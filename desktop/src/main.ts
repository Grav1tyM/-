import { invoke } from "@tauri-apps/api/core";
import { openUrl } from "@tauri-apps/plugin-opener";
import { filterMatches, parseThreshold, type MatchRow, type Priority } from "./filter";

type Player = {
  accountId: string;
  name: string;
  avatarUrl: string | null;
};

type Cache = {
  accountId: string;
  matches: MatchRow[];
};

function required<T>(value: T | null): T {
  if (value == null) {
    throw new Error("页面结构不完整");
  }
  return value;
}

const lookupForm = required(document.querySelector<HTMLFormElement>("#lookup-form"));
const codeInput = required(document.querySelector<HTMLInputElement>("#friend-code"));
const lookupButton = required(document.querySelector<HTMLButtonElement>("#lookup-button"));
const playerCard = required(document.querySelector<HTMLElement>("#player"));
const filterForm = required(document.querySelector<HTMLFormElement>("#filter-form"));
const headshotInput = required(document.querySelector<HTMLInputElement>("#headshot-threshold"));
const accuracyInput = required(document.querySelector<HTMLInputElement>("#accuracy-threshold"));
const filterButton = required(document.querySelector<HTMLButtonElement>("#filter-button"));
const statusEl = required(document.querySelector<HTMLElement>("#status"));
const errorEl = required(document.querySelector<HTMLElement>("#error"));
const resultsEl = required(document.querySelector<HTMLElement>("#results"));
const summaryEl = required(document.querySelector<HTMLElement>("#summary"));
const theadEl = required(document.querySelector<HTMLElement>("#thead"));
const tbodyEl = required(document.querySelector<HTMLElement>("#tbody"));

let player: Player | null = null;
let cache: Cache | null = null;
let busy = false;

lookupForm.addEventListener("submit", (event) => {
  event.preventDefault();
  void findPlayer();
});

filterForm.addEventListener("submit", (event) => {
  event.preventDefault();
  void applyFilter();
});

async function findPlayer(): Promise<void> {
  if (busy) {
    return;
  }
  clearError();
  hideResults();
  setBusy(true);
  setStatus("正在查找玩家…");
  try {
    const next = await invoke<Player>("lookup_player", { code: codeInput.value });
    player = next;
    if (!cache || cache.accountId !== next.accountId) {
      cache = null;
    }
    renderPlayer(next);
    filterForm.hidden = false;
    setStatus(`已找到 ${next.name}。设置阈值后筛选比赛。`);
  } catch (error) {
    player = null;
    cache = null;
    playerCard.hidden = true;
    filterForm.hidden = true;
    showError(messageOf(error));
    setStatus("");
  } finally {
    setBusy(false);
  }
}

async function applyFilter(): Promise<void> {
  if (busy || !player) {
    return;
  }
  const headshot = parseThreshold(headshotInput.value);
  const accuracy = parseThreshold(accuracyInput.value);
  if (headshot == null || accuracy == null) {
    showError("请把两个阈值都填成 0 到 100 之间的数字。");
    return;
  }
  clearError();
  const priority = selectedPriority();
  setBusy(true);
  try {
    const matches = await ensureMatches(player.accountId);
    if (matches.length === 0) {
      hideResults();
      setStatus("");
      showError("这个玩家没有可显示的比赛记录。可能还没有 Deadlock 对局，或记录为空。");
      return;
    }
    const filtered = filterMatches(matches, headshot, accuracy, priority);
    renderResults(filtered.rows, filtered.missingStats, matches.length, priority);
    setStatus("");
  } catch (error) {
    hideResults();
    showError(messageOf(error));
    setStatus("");
  } finally {
    setBusy(false);
  }
}

async function ensureMatches(accountId: string): Promise<MatchRow[]> {
  if (cache?.accountId === accountId) {
    return cache.matches;
  }
  setStatus("正在从 Statlocker 读取全部比赛…");
  const matches = await invoke<MatchRow[]>("load_matches", { accountId });
  cache = { accountId, matches };
  return matches;
}

function renderPlayer(next: Player): void {
  playerCard.hidden = false;
  playerCard.replaceChildren();

  if (next.avatarUrl) {
    const image = document.createElement("img");
    image.className = "avatar";
    image.alt = "";
    image.src = next.avatarUrl;
    image.addEventListener("error", () => image.remove());
    playerCard.append(image);
  }

  const text = document.createElement("div");
  const name = document.createElement("p");
  name.className = "player-name";
  name.textContent = next.name;
  const meta = document.createElement("p");
  meta.className = "player-meta";
  meta.textContent = `账户 ID ${next.accountId}`;
  text.append(name, meta);
  playerCard.append(text);

  const link = document.createElement("button");
  link.type = "button";
  link.className = "text-button";
  link.textContent = "打开 Statlocker 主页";
  link.addEventListener("click", () => {
    void openExternal(`https://statlocker.gg/profile/${next.accountId}`);
  });
  playerCard.append(link);
}

function renderResults(
  rows: MatchRow[],
  missingStats: number,
  total: number,
  priority: Priority,
): void {
  resultsEl.hidden = false;
  summaryEl.replaceChildren();
  const summary = document.createElement("p");
  summary.textContent = `已读取 ${total} 场，高于阈值 ${rows.length} 场。`;
  summaryEl.append(summary);
  if (missingStats > 0) {
    const note = document.createElement("p");
    note.className = "note";
    note.textContent = `另有 ${missingStats} 场缺少爆头率或命中率，未列入。`;
    summaryEl.append(note);
  }
  if (rows.length === 0) {
    const empty = document.createElement("p");
    empty.className = "note";
    empty.textContent = "没有同时高于两个阈值的比赛。";
    summaryEl.append(empty);
  }

  const headshotFirst = priority === "headshot";
  theadEl.replaceChildren();
  const headRow = document.createElement("tr");
  for (const label of [
    "比赛代码",
    headshotFirst ? "爆头率" : "命中率",
    headshotFirst ? "命中率" : "爆头率",
    "时间",
    "K / D / A",
  ]) {
    const cell = document.createElement("th");
    cell.scope = "col";
    cell.textContent = label;
    if (label === "爆头率" || label === "命中率") {
      cell.className = label.startsWith(headshotFirst ? "爆头" : "命中") ? "priority" : "";
    }
    headRow.append(cell);
  }
  theadEl.append(headRow);

  tbodyEl.replaceChildren();
  for (const match of rows) {
    const row = document.createElement("tr");
    row.append(matchCodeCell(match.matchId));
    const primary = headshotFirst ? match.headshotAccuracy : match.accuracy;
    const secondary = headshotFirst ? match.accuracy : match.headshotAccuracy;
    row.append(percentCell(primary, true), percentCell(secondary, false));
    const time = document.createElement("td");
    time.textContent = formatTime(match.startTimeMs);
    const kda = document.createElement("td");
    kda.textContent = formatKda(match);
    row.append(time, kda);
    tbodyEl.append(row);
  }
}

function matchCodeCell(matchId: string): HTMLTableCellElement {
  const cell = document.createElement("td");
  const button = document.createElement("button");
  button.type = "button";
  button.className = "match-code";
  button.textContent = matchId;
  button.title = "在浏览器中打开这场比赛";
  button.addEventListener("click", () => {
    if (!/^\d+$/.test(matchId)) {
      return;
    }
    void openExternal(`https://statlocker.gg/match/${matchId}`);
  });
  cell.append(button);
  return cell;
}

function percentCell(value: number | null, priority: boolean): HTMLTableCellElement {
  const cell = document.createElement("td");
  cell.textContent = value == null ? "—" : `${(value * 100).toFixed(1)}%`;
  if (priority) {
    cell.className = "priority";
  }
  return cell;
}

function formatKda(match: MatchRow): string {
  if (match.kills == null || match.deaths == null || match.assists == null) {
    return "—";
  }
  return `${match.kills} / ${match.deaths} / ${match.assists}`;
}

function formatTime(ms: number | null): string {
  if (ms == null) {
    return "—";
  }
  const date = new Date(ms);
  if (Number.isNaN(date.getTime())) {
    return "—";
  }
  const pad = (value: number) => String(value).padStart(2, "0");
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

function selectedPriority(): Priority {
  const picked = filterForm.querySelector<HTMLInputElement>('input[name="priority"]:checked');
  return picked?.value === "accuracy" ? "accuracy" : "headshot";
}

async function openExternal(url: string): Promise<void> {
  try {
    await openUrl(url);
  } catch {
    showError("无法打开浏览器。请稍后重试。");
  }
}

function messageOf(error: unknown): string {
  if (typeof error === "string" && error.trim()) {
    return error;
  }
  if (error instanceof Error && error.message.trim()) {
    return error.message;
  }
  return "查询失败，请检查网络后重试。";
}

function showError(message: string): void {
  errorEl.hidden = false;
  errorEl.textContent = message;
}

function clearError(): void {
  errorEl.hidden = true;
  errorEl.textContent = "";
}

function setStatus(message: string): void {
  statusEl.textContent = message;
}

function hideResults(): void {
  resultsEl.hidden = true;
  summaryEl.replaceChildren();
  theadEl.replaceChildren();
  tbodyEl.replaceChildren();
}

function setBusy(next: boolean): void {
  busy = next;
  lookupButton.disabled = next;
  filterButton.disabled = next;
  codeInput.disabled = next;
}

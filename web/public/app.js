const form = document.querySelector("#query-form");
const statusEl = document.querySelector("#status");
const choicesEl = document.querySelector("#choices");
const playerEl = document.querySelector("#player");
const resultsEl = document.querySelector("#results");
const submit = document.querySelector("#submit");

let busy = false;

function setStatus(kind, text) {
  if (!text) {
    statusEl.hidden = true;
    statusEl.textContent = "";
    statusEl.className = "status";
    return;
  }
  statusEl.hidden = false;
  statusEl.className = kind === "error" ? "status error" : "status";
  statusEl.textContent = text;
}

function formatWhen(ms) {
  if (!Number.isFinite(ms)) return "—";
  return new Intl.DateTimeFormat("zh-CN", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
    hourCycle: "h23",
  }).format(new Date(ms));
}

function formatPct(value) {
  return `${value.toFixed(1)}%`;
}

async function post(path, payload) {
  const response = await fetch(path, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });
  const body = await response.json().catch(() => ({}));
  if (!response.ok) throw new Error(body.error || `请求失败（${response.status}）`);
  return body;
}

function selectedScope() {
  return new FormData(form).get("scope") === "all" ? "all" : "recent";
}

function thresholds() {
  return {
    headshotAbove: Number(document.querySelector("#headshot").value),
    accuracyAbove: Number(document.querySelector("#accuracy").value),
  };
}

function renderChoices(players) {
  choicesEl.hidden = false;
  choicesEl.replaceChildren();
  const title = document.createElement("p");
  title.className = "hint";
  title.textContent = "找到多个玩家，请选择一个。";
  choicesEl.append(title);
  for (const player of players) {
    const button = document.createElement("button");
    button.type = "button";
    button.className = "choice";
    if (player.avatarUrl) {
      const img = document.createElement("img");
      img.alt = "";
      img.src = player.avatarUrl;
      button.append(img);
    }
    const text = document.createElement("span");
    text.textContent = `${player.name} · ${player.accountId}`;
    button.append(text);
    button.addEventListener("click", () => loadMatches(player.accountId));
    choicesEl.append(button);
  }
}

function renderPlayer(player) {
  playerEl.hidden = false;
  playerEl.replaceChildren();
  if (player.avatarUrl) {
    const img = document.createElement("img");
    img.alt = "";
    img.src = player.avatarUrl;
    playerEl.append(img);
  }
  const copy = document.createElement("div");
  const name = document.createElement("h2");
  name.textContent = player.name;
  const meta = document.createElement("p");
  const bits = [`账号 ${player.accountId}`];
  if (player.region) bits.push(`地区 ${player.region}`);
  meta.textContent = bits.join(" · ");
  const link = document.createElement("a");
  link.href = player.profileUrl;
  link.target = "_blank";
  link.rel = "noopener noreferrer";
  link.textContent = "在 Statlocker 打开资料";
  const line = document.createElement("p");
  line.append(link);
  copy.append(name, meta, line);
  playerEl.append(copy);
}

function renderResults(data) {
  resultsEl.hidden = false;
  resultsEl.replaceChildren();
  const summary = document.createElement("p");
  summary.className = "summary";
  const scopeText = data.scope === "all" ? "全部对局" : "最近完整记录";
  const total =
    data.totalMatches != null && data.totalMatches !== data.scanned
      ? `资料中约 ${data.totalMatches} 场，`
      : "";
  const missing = data.missingAim
    ? ` ${data.missingAim} 场缺少爆头率或命中率，已跳过。`
    : "";
  summary.textContent = `${scopeText}：${total}本次扫描 ${data.scanned} 场，${data.matches.length} 场两项都高于阈值。${missing}`;
  resultsEl.append(summary);

  if (data.matches.length === 0) {
    const empty = document.createElement("p");
    empty.className = "empty";
    empty.textContent = "这批对局里，没有爆头率和命中率都高于阈值的场次。";
    resultsEl.append(empty);
    return;
  }

  const scroll = document.createElement("div");
  scroll.className = "table-scroll";
  const table = document.createElement("table");
  const head = document.createElement("thead");
  const headRow = document.createElement("tr");
  for (const [label, numeric] of [
    ["日期", false],
    ["英雄", false],
    ["爆头率", true],
    ["命中率", true],
    ["对局", false],
  ]) {
    const cell = document.createElement("th");
    cell.textContent = label;
    if (numeric) cell.className = "num";
    headRow.append(cell);
  }
  head.append(headRow);
  const body = document.createElement("tbody");
  for (const match of data.matches) {
    const row = document.createElement("tr");
    const date = document.createElement("td");
    date.textContent = formatWhen(match.startTimeMs);
    const hero = document.createElement("td");
    hero.textContent = match.heroName || (match.heroId != null ? `英雄 #${match.heroId}` : "—");
    const headshot = document.createElement("td");
    headshot.className = "num";
    headshot.textContent = formatPct(match.headshot);
    const accuracy = document.createElement("td");
    accuracy.className = "num";
    accuracy.textContent = formatPct(match.accuracy);
    const linkCell = document.createElement("td");
    if (match.url) {
      const link = document.createElement("a");
      link.href = match.url;
      link.target = "_blank";
      link.rel = "noopener noreferrer";
      link.textContent = "打开";
      linkCell.append(link);
    } else {
      linkCell.textContent = "—";
    }
    row.append(date, hero, headshot, accuracy, linkCell);
    body.append(row);
  }
  table.append(head, body);
  scroll.append(table);
  resultsEl.append(scroll);
}

async function loadMatches(accountId) {
  if (busy) return;
  busy = true;
  submit.disabled = true;
  choicesEl.hidden = true;
  resultsEl.hidden = true;
  setStatus("loading", "正在查询 Statlocker…");
  try {
    const data = await post("/api/matches", {
      accountId,
      scope: selectedScope(),
      ...thresholds(),
    });
    setStatus(null, "");
    renderPlayer(data.player);
    renderResults(data);
  } catch (error) {
    setStatus("error", error.message || "查询失败");
  } finally {
    busy = false;
    submit.disabled = false;
  }
}

form.addEventListener("submit", async (event) => {
  event.preventDefault();
  if (busy) return;
  busy = true;
  submit.disabled = true;
  choicesEl.hidden = true;
  playerEl.hidden = true;
  resultsEl.hidden = true;
  setStatus("loading", "正在查找玩家…");
  try {
    const query = document.querySelector("#friend-code").value;
    const { players } = await post("/api/resolve", { query });
    if (!players.length) {
      setStatus("error", "没有找到这个好友代码对应的玩家。");
      return;
    }
    if (players.length > 1) {
      setStatus(null, "");
      renderChoices(players);
      return;
    }
    busy = false;
    submit.disabled = false;
    await loadMatches(players[0].accountId);
    return;
  } catch (error) {
    setStatus("error", error.message || "查询失败");
  } finally {
    busy = false;
    submit.disabled = false;
  }
});

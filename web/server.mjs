import { createServer } from "node:http";
import { readFile } from "node:fs/promises";
import { extname, resolve, sep } from "node:path";
import { fileURLToPath } from "node:url";
import { filterMatches } from "./filter.mjs";

const PORT = Number(process.env.PORT || 8787);
const ROOT = fileURLToPath(new URL("./public/", import.meta.url));
const ORIGIN = "https://statlocker.gg";
const UA =
  "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36";

const TYPES = {
  ".html": "text/html; charset=utf-8",
  ".css": "text/css; charset=utf-8",
  ".js": "text/javascript; charset=utf-8",
  ".svg": "image/svg+xml",
};

let heroNames = null;
let heroNamesAt = 0;

function send(res, status, body, type = "application/json; charset=utf-8") {
  const payload = Buffer.isBuffer(body) || typeof body === "string" ? body : JSON.stringify(body);
  res.writeHead(status, {
    "Content-Type": type,
    "Cache-Control": "no-store",
  });
  res.end(payload);
}

function readJson(req) {
  return new Promise((resolve, reject) => {
    const chunks = [];
    let size = 0;
    req.on("data", (chunk) => {
      size += chunk.length;
      if (size > 20_000) {
        reject(Object.assign(new Error("请求体过大"), { status: 413 }));
        req.destroy();
        return;
      }
      chunks.push(chunk);
    });
    req.on("end", () => {
      if (chunks.length === 0) {
        resolve({});
        return;
      }
      try {
        resolve(JSON.parse(Buffer.concat(chunks).toString("utf8")));
      } catch {
        reject(Object.assign(new Error("请求不是合法的 JSON"), { status: 400 }));
      }
    });
    req.on("error", reject);
  });
}

// Same unauthenticated calls the Statlocker pages make. Without Origin the
// server answers 401 "Missing API key"; the website itself does not ask the
// visitor for a key. This is not a Steam login and not a private credential.
async function statlocker(path, timeoutMs) {
  const response = await fetch(ORIGIN + path, {
    headers: {
      Accept: "application/json",
      "User-Agent": UA,
      Origin: ORIGIN,
      Referer: ORIGIN + "/",
    },
    signal: AbortSignal.timeout(timeoutMs),
  });
  const text = await response.text();
  let body = null;
  try {
    body = text ? JSON.parse(text) : null;
  } catch {
    body = null;
  }
  if (!response.ok) {
    const message =
      (body && (body.message || body.error)) || `Statlocker 返回 HTTP ${response.status}`;
    throw Object.assign(new Error(message), { status: response.status === 404 ? 404 : 502 });
  }
  if (body === null) {
    throw Object.assign(new Error("Statlocker 返回了无法解析的内容"), { status: 502 });
  }
  return body;
}

function normalizeQuery(raw) {
  const text = String(raw ?? "").trim();
  const profile = text.match(/steamcommunity\.com\/profiles\/(\d+)/i);
  if (profile) return profile[1];
  const vanity = text.match(/steamcommunity\.com\/id\/([^/?#]+)/i);
  if (vanity) return decodeURIComponent(vanity[1]);
  return text;
}

function slimPlayer(profile) {
  if (!profile || profile.accountId == null) return null;
  const accountId = String(profile.accountId);
  if (!/^\d+$/.test(accountId)) return null;
  return {
    accountId,
    name: profile.name || "未知玩家",
    avatarUrl: profile.avatarUrl || null,
    region: profile.region || null,
    profileUrl: `${ORIGIN}/profile/${accountId}`,
  };
}

async function resolvePlayers(query) {
  const normalized = normalizeQuery(query);
  if (!normalized || normalized.length > 200) {
    throw Object.assign(new Error("请输入 Steam 好友代码"), { status: 400 });
  }
  const found = await statlocker(
    `/api/profile/search-profiles/${encodeURIComponent(normalized)}`,
    20_000,
  );
  let players = (Array.isArray(found) ? found : []).map(slimPlayer).filter(Boolean);
  // The site's search box, on Enter, opens a numeric id of 5+ digits even
  // when search returns nothing.
  if (players.length === 0 && /^\d{5,20}$/.test(normalized)) {
    try {
      const profile = await statlocker(`/api/profile/${normalized}`, 20_000);
      const player = slimPlayer(profile);
      if (player) players = [player];
    } catch (error) {
      if (error.status !== 404) throw error;
    }
  }
  return players;
}

function parseThreshold(value, label) {
  const number = typeof value === "number" ? value : Number(String(value ?? "").trim());
  if (!Number.isFinite(number) || number < 0 || number > 100) {
    throw Object.assign(new Error(`${label}请填写 0 到 100 之间的百分数`), { status: 400 });
  }
  return number;
}

async function loadHeroNames() {
  if (heroNames && Date.now() - heroNamesAt < 24 * 60 * 60 * 1000) return heroNames;
  try {
    const list = await statlocker("/api/info/heroes-full", 30_000);
    const map = new Map();
    if (Array.isArray(list)) {
      for (const hero of list) {
        if (hero && hero.id != null && hero.name) map.set(Number(hero.id), String(hero.name));
      }
    }
    if (map.size > 0) {
      heroNames = map;
      heroNamesAt = Date.now();
    }
  } catch (error) {
    console.error("hero catalog skipped:", error.message);
  }
  return heroNames;
}

async function loadMatches(accountId, scope) {
  const suffix = scope === "all" ? "true" : "full-recent";
  // gameMode=1 & version=2 is the profile page's default "All" request.
  const path = `/api/profile/data/matches/${accountId}/${suffix}?gameMode=1&version=2`;
  return statlocker(path, scope === "all" ? 60_000 : 30_000);
}

async function queryMatches(body) {
  const accountId = String(body.accountId ?? "");
  if (!/^\d{1,20}$/.test(accountId)) {
    throw Object.assign(new Error("缺少玩家 ID"), { status: 400 });
  }
  const scope = body.scope === "all" ? "all" : "recent";
  const headshotAbove = parseThreshold(body.headshotAbove, "爆头率");
  const accuracyAbove = parseThreshold(body.accuracyAbove, "命中率");

  const [profile, history, heroes] = await Promise.all([
    statlocker(`/api/profile/${accountId}`, 20_000),
    loadMatches(accountId, scope),
    loadHeroNames(),
  ]);
  const player = slimPlayer(profile);
  if (!player) throw Object.assign(new Error("没有找到这个玩家"), { status: 404 });

  const rows = Array.isArray(history.matchHistory) ? history.matchHistory : [];
  const { included, missingAim } = filterMatches(rows, headshotAbove, accuracyAbove);
  const totalMatches =
    typeof history.totalMatches === "number"
      ? history.totalMatches
      : history.profileAggregateStats && typeof history.profileAggregateStats.totalMatches === "number"
        ? history.profileAggregateStats.totalMatches
        : null;

  return {
    player,
    scope,
    scanned: rows.length,
    totalMatches,
    missingAim,
    thresholds: { headshotAbove, accuracyAbove },
    matches: included.map((match) => ({
      matchId: match.matchId,
      startTimeMs: match.startTimeMs,
      heroId: match.heroId ?? null,
      heroName: match.heroId != null && heroes ? heroes.get(Number(match.heroId)) || null : null,
      headshot: match.headshot,
      accuracy: match.accuracy,
      url:
        match.matchId != null
          ? `${ORIGIN}/profile/${accountId}/matches/${match.matchId}/summary`
          : null,
    })),
  };
}

async function serveStatic(res, url) {
  const requested = url.pathname === "/" ? "/index.html" : url.pathname;
  const root = resolve(ROOT);
  const path = resolve(root, `.${requested}`);
  if (path !== root && !path.startsWith(root + sep)) {
    send(res, 404, { error: "未找到页面" });
    return;
  }
  try {
    const file = await readFile(path);
    send(res, 200, file, TYPES[extname(path)] || "application/octet-stream");
  } catch {
    send(res, 404, { error: "未找到页面" });
  }
}

const server = createServer(async (req, res) => {
  try {
    const url = new URL(req.url || "/", `http://127.0.0.1:${PORT}`);
    if (req.method === "POST" && url.pathname === "/api/resolve") {
      const body = await readJson(req);
      const players = await resolvePlayers(body.query);
      send(res, 200, { players });
      return;
    }
    if (req.method === "POST" && url.pathname === "/api/matches") {
      const body = await readJson(req);
      send(res, 200, await queryMatches(body));
      return;
    }
    if (req.method === "GET") {
      await serveStatic(res, url);
      return;
    }
    send(res, 405, { error: "不支持的请求" });
  } catch (error) {
    const status = error.status || (error.name === "TimeoutError" ? 504 : 500);
    const message =
      error.name === "TimeoutError" ? "Statlocker 响应超时，请稍后再试" : error.message || "查询失败";
    if (status >= 500) console.error(error);
    send(res, status, { error: message });
  }
});

server.listen(PORT, "127.0.0.1", () => {
  console.log(`listening on http://127.0.0.1:${PORT}`);
});

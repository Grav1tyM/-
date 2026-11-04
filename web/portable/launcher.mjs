// Starts the existing server and opens the browser. The filter itself stays in server.mjs.
import { spawn } from "node:child_process";
import { createConnection } from "node:net";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const appDir = dirname(fileURLToPath(import.meta.url));

function portIsFree(port) {
  return new Promise((resolve) => {
    const socket = createConnection({ host: "127.0.0.1", port });
    const finish = (free) => {
      socket.removeAllListeners();
      socket.destroy();
      resolve(free);
    };
    socket.once("connect", () => finish(false));
    socket.once("error", () => finish(true));
  });
}

async function pickPort() {
  for (let port = 8787; port <= 8799; port += 1) {
    if (await portIsFree(port)) return port;
  }
  throw new Error("端口 8787–8799 都被占用了。关掉占用它们的程序后再试。");
}

function waitForPage(port) {
  const deadline = Date.now() + 20_000;
  return new Promise((resolve, reject) => {
    const tick = () => {
      fetch(`http://127.0.0.1:${port}/`)
        .then((response) => {
          if (response.ok) resolve();
          else retry();
        })
        .catch(retry);
    };
    const retry = () => {
      if (Date.now() > deadline) reject(new Error("页面没有在时限内打开。"));
      else setTimeout(tick, 200);
    };
    tick();
  });
}

function openBrowser(url) {
  if (process.platform === "win32") {
    spawn("cmd", ["/c", "start", "", url], {
      detached: true,
      stdio: "ignore",
      windowsHide: true,
    }).unref();
    return;
  }
  const command = process.platform === "darwin" ? "open" : "xdg-open";
  spawn(command, [url], { detached: true, stdio: "ignore" }).unref();
}

const port = await pickPort();
const child = spawn(process.execPath, [join(appDir, "server.mjs")], {
  cwd: appDir,
  env: { ...process.env, PORT: String(port) },
  stdio: "inherit",
});

let shuttingDown = false;
function shutdown() {
  if (shuttingDown) return;
  shuttingDown = true;
  if (!child.killed) child.kill();
}

process.on("SIGINT", () => {
  shutdown();
  process.exit(0);
});
process.on("SIGTERM", () => {
  shutdown();
  process.exit(0);
});
child.on("exit", (code) => {
  if (!shuttingDown) process.exit(code ?? 0);
});

try {
  await waitForPage(port);
} catch (error) {
  console.error(error.message);
  shutdown();
  process.exit(1);
}

const url = `http://127.0.0.1:${port}/`;
console.log("");
console.log("对局瞄准筛选已启动。");
console.log(url);
console.log("请不要关闭这个窗口。用完后关闭窗口即可退出。");
openBrowser(url);

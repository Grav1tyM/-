// Builds a Windows folder zip: unzip, double-click, no Node install.
// Usage: node scripts/build-windows-zip.mjs [output.zip]
import { execFile } from "node:child_process";
import { mkdir, cp, writeFile, rm, access } from "node:fs/promises";
import { dirname, isAbsolute, join, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { promisify } from "node:util";

const exec = promisify(execFile);
const webRoot = join(dirname(fileURLToPath(import.meta.url)), "..");
const nodeVersion = "v22.23.4";
const nodeFolder = `node-${nodeVersion}-win-x64`;
const nodeZipName = `${nodeFolder}.zip`;
const nodeUrl = `https://nodejs.org/dist/${nodeVersion}/${nodeZipName}`;
const defaultOut = join(webRoot, "dist", "statlocker-filter-windows.zip");

const output = process.argv[2]
  ? isAbsolute(process.argv[2])
    ? process.argv[2]
    : resolve(process.argv[2])
  : defaultOut;
const cache = join(webRoot, "dist", "cache");
const stage = join(webRoot, "dist", "stage", "statlocker-filter");

async function downloadNode() {
  await mkdir(cache, { recursive: true });
  const zipPath = join(cache, nodeZipName);
  try {
    await access(zipPath);
  } catch {
    console.log("downloading", nodeUrl);
    await exec("curl", ["-fsSL", "-o", zipPath, nodeUrl], { maxBuffer: 1024 * 1024 });
  }
  return zipPath;
}

async function main() {
  const nodeZip = await downloadNode();
  await rm(stage, { recursive: true, force: true });
  await mkdir(join(stage, "runtime"), { recursive: true });
  await mkdir(join(stage, "app", "public"), { recursive: true });

  await exec("unzip", ["-j", "-o", nodeZip, `${nodeFolder}/node.exe`, "-d", join(stage, "runtime")]);
  await exec("unzip", ["-j", "-o", nodeZip, `${nodeFolder}/LICENSE`, "-d", join(stage, "runtime")]);

  await cp(join(webRoot, "server.mjs"), join(stage, "app", "server.mjs"));
  await cp(join(webRoot, "filter.mjs"), join(stage, "app", "filter.mjs"));
  await cp(join(webRoot, "portable", "launcher.mjs"), join(stage, "app", "launcher.mjs"));
  await cp(join(webRoot, "public"), join(stage, "app", "public"), { recursive: true });
  await cp(join(webRoot, "portable", "Start.bat"), join(stage, "Start.bat"));
  await cp(join(webRoot, "portable", "Start.bat"), join(stage, "打开筛选.bat"));
  await cp(join(webRoot, "portable", "使用说明.txt"), join(stage, "使用说明.txt"));

  await mkdir(dirname(output), { recursive: true });
  await writeZip(stage, output);
  console.log(output);
}

function writeZip(stageDir, dest) {
  const script = `
import os, sys, zipfile
stage, dest = sys.argv[1], sys.argv[2]
os.makedirs(os.path.dirname(dest) or ".", exist_ok=True)
if os.path.exists(dest):
    os.remove(dest)
with zipfile.ZipFile(dest, "w", compression=zipfile.ZIP_DEFLATED, compresslevel=6) as z:
    for dirpath, _, files in os.walk(stage):
        for name in files:
            full = os.path.join(dirpath, name)
            rel = os.path.relpath(full, stage).replace(os.sep, "/")
            info = zipfile.ZipInfo("statlocker-filter/" + rel)
            info.flag_bits |= 0x800
            info.compress_type = zipfile.ZIP_DEFLATED
            info.create_system = 0
            info.date_time = (2026, 11, 4, 12, 0, 0)
            with open(full, "rb") as handle:
                z.writestr(info, handle.read())
print(os.path.getsize(dest))
`;
  return exec("python3", ["-c", script, stageDir, dest], { maxBuffer: 1024 * 1024 });
}

main().catch((error) => {
  console.error(error);
  process.exit(1);
});

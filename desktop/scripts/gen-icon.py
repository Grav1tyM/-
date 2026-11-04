"""Draw a simple green mark and write the Tauri icon set."""

import struct
import zlib
from pathlib import Path


def png(width: int, height: int, pixel) -> bytes:
    raw = bytearray()
    for y in range(height):
        raw.append(0)
        for x in range(width):
            raw.extend(pixel(x, y, width, height))

    def chunk(tag: bytes, data: bytes) -> bytes:
        return (
            struct.pack(">I", len(data))
            + tag
            + data
            + struct.pack(">I", zlib.crc32(tag + data) & 0xFFFFFFFF)
        )

    ihdr = struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)
    return (
        b"\x89PNG\r\n\x1a\n"
        + chunk(b"IHDR", ihdr)
        + chunk(b"IDAT", zlib.compress(bytes(raw), 9))
        + chunk(b"IEND", b"")
    )


def icon(x: int, y: int, width: int, height: int) -> tuple[int, int, int, int]:
    nx = (x + 0.5) / width * 2 - 1
    ny = (y + 0.5) / height * 2 - 1
    ax = abs(nx)
    ay = abs(ny)
    if ax > 0.82 or ay > 0.82:
        return (0, 0, 0, 0)
    corner = max(ax - 0.58, 0) ** 2 + max(ay - 0.58, 0) ** 2
    if corner > 0.07:
        return (0, 0, 0, 0)
    on_cross = abs(nx) < 0.09 or abs(ny) < 0.09
    ring = 0.28 < (nx * nx + ny * ny) ** 0.5 < 0.42
    if on_cross or ring:
        return (255, 250, 242, 255)
    return (11, 107, 74, 255)


def ico(png_bytes: bytes, size: int) -> bytes:
    header = struct.pack("<HHH", 0, 1, 1)
    entry = struct.pack(
        "<BBBBHHII",
        size if size < 256 else 0,
        size if size < 256 else 0,
        0,
        0,
        1,
        32,
        len(png_bytes),
        22,
    )
    return header + entry + png_bytes


def main() -> None:
    out = Path(__file__).resolve().parents[1] / "src-tauri" / "icons"
    out.mkdir(parents=True, exist_ok=True)
    for name, size in (("32x32.png", 32), ("128x128.png", 128), ("128x128@2x.png", 256)):
        (out / name).write_bytes(png(size, size, icon))
    big = png(256, 256, icon)
    (out / "icon.ico").write_bytes(ico(big, 256))


if __name__ == "__main__":
    main()

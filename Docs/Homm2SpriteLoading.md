# HoMM2 Sprite Loading

`PixelHeroes.Homm2` can load HoMM2 sprites without depending on Godot, `System.Drawing.Bitmap`, SDL or any other rendering API.

Supported resource formats:

- `HEROES2.AGG` archive entries.
- `KB.PAL` palette files.
- `*.ICN` sprite files with normal and monochrome RLE frames.

Public entry point:

- `Homm2SpriteLoader.LoadFromAgg(aggPath, icnName, paletteName = "KB.PAL")`
- `Homm2SpriteLoader.LoadFromFiles(icnPath, palettePath)`

Decoded frame data:

- `Width`, `Height`.
- `OffsetX`, `OffsetY`.
- `PivotX`, `PivotY`, derived from the negative ICN offsets.
- `CenterX`, `CenterY`, derived from the frame size.
- `PaletteIndexes`: one byte per pixel, already unpacked from ICN RLE.
- `Alpha`: one byte per pixel.

Alpha values:

- `0`: transparent pixel.
- `64`: HoMM2 shadow / semi-transparent pixel.
- `255`: opaque palette-indexed pixel.

The separate alpha array is intentional. ICN transparency is encoded by RLE commands, not by a normal color index, so a single byte-per-pixel palette index buffer cannot distinguish a transparent pixel from an opaque pixel using palette index `0`.

Palette data:

- `Homm2Palette.Red`, `Green`, `Blue`: 256-byte channels.
- `KB.PAL` stores 6-bit color components in the original game data, so the loader scales them to 8-bit channels.
- `ToRgbaBytes()` can produce a flat 256-color RGBA palette, but sprite-specific transparency and shadow still come from each frame's `Alpha` array.

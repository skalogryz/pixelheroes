# PixelHeroes.Homm2.TileDumper

Console tool for exporting HoMM2 adventure-map terrain tiles from `HEROES2.AGG`.

Default mode writes one atlas PNG:

```powershell
PixelHeroes.Homm2.TileDumper.exe --agg "C:\Games\HEROES2\DATA\HEROES2.AGG" --out homm2_tiles.png
```

Export one PNG per tile:

```powershell
PixelHeroes.Homm2.TileDumper.exe --agg "C:\Games\HEROES2\DATA\HEROES2.AGG" --separate --out homm2_tiles
```

Defaults:

- Tile resource: `GROUND32.TIL`
- Palette resource: `KB.PAL`
- Atlas columns: `32`

# HoMM2 Resource Cache

The Godot project loads HoMM2 map terrain textures through `Homm2ResourceCache`.

When a HoMM2 map is selected, `MainMenu` reads the map first and then asks the cache to load resources for that map path. The cache searches for `HEROES2.AGG` in the map directory, parent directories, and common `DATA` / `data` subdirectories.

Only the first successful load creates textures. Later map loads reuse the already decoded `GROUND32.TIL` textures and do not read `HEROES2.AGG` again.

`MapTile.Sprite` stores the HoMM2 `TerrainImageIndex` and terrain flip flags as a string:

```text
terrain:<TerrainImageIndex>:<v|->:<h|->
```

HoMM2 terrain flags are interpreted as:

- bit `1`: vertical flip.
- bit `2`: horizontal flip.

The renderer uses the index to find the base `GROUND32.TIL` texture and lazily creates flipped texture variants only when a map tile asks for them. If resources are missing, rendering falls back to colored terrain cells.

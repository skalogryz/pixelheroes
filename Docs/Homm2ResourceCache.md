# HoMM2 Resource Cache

The Godot project loads HoMM2 map terrain textures through `Homm2ResourceCache`.

When a HoMM2 map is selected, `MainMenu` reads the map first and then asks the cache to load resources for that map path. The cache searches for `HEROES2.AGG` in the map directory, parent directories, and common `DATA` / `data` subdirectories.

Only the first successful load creates textures. Later map loads reuse the already decoded `GROUND32.TIL` textures and do not read `HEROES2.AGG` again.

`MapTile.Sprite` stores the HoMM2 `TerrainImageIndex` as a string. The map renderer uses that string to find the corresponding terrain texture by index. If resources are missing, rendering falls back to colored terrain cells.

# Godot HoMM2 Texture Conversion

The main Godot project contains `Homm2SpriteTextureConverter`.

It converts HoMM2 sprite data from `PixelHeroes.Homm2` into Godot runtime textures:

- `Homm2SpriteFrame` + `Homm2Palette` -> `Image`
- `Homm2SpriteFrame` + `Homm2Palette` -> `ImageTexture`
- `Homm2SpriteSet` -> list of converted frames
- `HEROES2.AGG` + ICN resource name -> list of converted frames

Example:

```csharp
var frames = Homm2SpriteTextureConverter.LoadFromAgg(@"C:\Games\HEROES2\DATA\HEROES2.AGG", "HEROES.ICN");
var sprite = new Sprite();
sprite.Texture = frames[0].Texture;
sprite.Offset = -frames[0].Pivot;
AddChild(sprite);
```

This converter intentionally lives in the Godot project, not in `PixelHeroes.Homm2`, because it depends on `Godot.Image`, `Godot.ImageTexture`, `Godot.Color` and `Godot.Vector2`.

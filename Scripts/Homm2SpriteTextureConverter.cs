using System;
using System.Collections.Generic;
using Godot;
using PixelHeroes.Homm2.Sprites;

public sealed class Homm2GodotSpriteFrame
{
    public Homm2GodotSpriteFrame(ImageTexture texture, Vector2 pivot, Vector2 center, Vector2 offset)
    {
        Texture = texture;
        Pivot = pivot;
        Center = center;
        Offset = offset;
    }

    public ImageTexture Texture { get; }
    public Vector2 Pivot { get; }
    public Vector2 Center { get; }
    public Vector2 Offset { get; }
}

public static class Homm2SpriteTextureConverter
{
    public static Homm2GodotSpriteFrame ConvertFrame(Homm2SpriteFrame frame, Homm2Palette palette)
    {
        if (frame == null)
        {
            throw new ArgumentNullException(nameof(frame));
        }

        var image = ConvertFrameToImage(frame, palette);
        var texture = new ImageTexture();
        texture.CreateFromImage(image, 0);

        return new Homm2GodotSpriteFrame(
            texture,
            new Vector2(frame.PivotX, frame.PivotY),
            new Vector2(frame.CenterX, frame.CenterY),
            new Vector2(frame.OffsetX, frame.OffsetY));
    }

    public static IReadOnlyList<Homm2GodotSpriteFrame> ConvertSpriteSet(Homm2SpriteSet spriteSet, Homm2Palette palette)
    {
        if (spriteSet == null)
        {
            throw new ArgumentNullException(nameof(spriteSet));
        }

        var frames = new List<Homm2GodotSpriteFrame>(spriteSet.Frames.Count);
        foreach (var frame in spriteSet.Frames)
        {
            frames.Add(ConvertFrame(frame, palette));
        }

        return frames;
    }

    public static IReadOnlyList<Homm2GodotSpriteFrame> ConvertResource(Homm2SpriteResource resource)
    {
        if (resource == null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        return ConvertSpriteSet(resource.SpriteSet, resource.Palette);
    }

    public static IReadOnlyList<Homm2GodotSpriteFrame> LoadFromAgg(string aggPath, string icnName, string paletteName = "KB.PAL")
    {
        var resource = new Homm2SpriteLoader().LoadFromAgg(aggPath, icnName, paletteName);
        return ConvertResource(resource);
    }

    public static IReadOnlyList<Homm2GodotSpriteFrame> LoadFromFiles(string icnPath, string palettePath)
    {
        var resource = new Homm2SpriteLoader().LoadFromFiles(icnPath, palettePath);
        return ConvertResource(resource);
    }

    public static Homm2GodotSpriteFrame CreateFlippedFrame(Homm2GodotSpriteFrame source, bool verticalFlip, bool horizontalFlip)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var image = source.Texture.GetData();
        if (horizontalFlip)
        {
            image.FlipX();
        }

        if (verticalFlip)
        {
            image.FlipY();
        }

        var texture = new ImageTexture();
        texture.CreateFromImage(image, 0);
        return new Homm2GodotSpriteFrame(texture, source.Pivot, source.Center, source.Offset);
    }

    public static Image ConvertFrameToImage(Homm2SpriteFrame frame, Homm2Palette palette)
    {
        if (frame == null)
        {
            throw new ArgumentNullException(nameof(frame));
        }

        if (palette == null)
        {
            throw new ArgumentNullException(nameof(palette));
        }

        var image = new Image();
        image.Create(frame.Width, frame.Height, false, Image.Format.Rgba8);
        image.Lock();

        for (var y = 0; y < frame.Height; y++)
        {
            for (var x = 0; x < frame.Width; x++)
            {
                var pixelIndex = y * frame.Width + x;
                var paletteIndex = frame.PaletteIndexes[pixelIndex];
                var alpha = frame.Alpha[pixelIndex] / 255.0f;
                var color = new Color(
                    palette.Red[paletteIndex] / 255.0f,
                    palette.Green[paletteIndex] / 255.0f,
                    palette.Blue[paletteIndex] / 255.0f,
                    alpha);

                image.SetPixel(x, y, color);
            }
        }

        image.Unlock();
        return image;
    }
}

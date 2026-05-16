using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PixelHeroes.Homm2.Sprites;

namespace PixelHeroes.Homm2.TileDumper
{
    internal static class Program
    {
        private const string DefaultTileSetName = "GROUND32.TIL";
        private const string DefaultPaletteName = "KB.PAL";

        private static int Main(string[] args)
        {
            try
            {
                var options = DumpOptions.Parse(args);
                if (options.ShowHelp)
                {
                    PrintUsage();
                    return 0;
                }

                var resource = new Homm2SpriteLoader().LoadTileSetFromAgg(options.AggPath, options.TileSetName, options.PaletteName);
                if (options.SeparateFiles)
                {
                    DumpSeparateFiles(resource, options.OutputPath);
                }
                else
                {
                    DumpAtlas(resource, options.OutputPath, options.Columns);
                }

                Console.WriteLine($"Exported {resource.SpriteSet.Frames.Count} tile(s).");
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                Console.Error.WriteLine();
                PrintUsage();
                return 1;
            }
        }

        private static void DumpAtlas(Homm2SpriteResource resource, string outputPath, int columns)
        {
            var frames = resource.SpriteSet.Frames;
            if (frames.Count == 0)
            {
                throw new InvalidOperationException("Tile set has no frames.");
            }

            var tileWidth = frames[0].Width;
            var tileHeight = frames[0].Height;
            var rows = (int)Math.Ceiling(frames.Count / (double)columns);
            var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
            if (!string.IsNullOrEmpty(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (var atlas = new Bitmap(tileWidth * columns, tileHeight * rows, PixelFormat.Format32bppArgb))
            using (var graphics = Graphics.FromImage(atlas))
            {
                graphics.Clear(Color.Transparent);
                for (var i = 0; i < frames.Count; i++)
                {
                    using (var tile = CreateBitmap(frames[i], resource.Palette))
                    {
                        var x = i % columns * tileWidth;
                        var y = i / columns * tileHeight;
                        graphics.DrawImageUnscaled(tile, x, y);
                    }
                }

                atlas.Save(outputPath, ImageFormat.Png);
            }

            Console.WriteLine($"Atlas: {outputPath}");
        }

        private static void DumpSeparateFiles(Homm2SpriteResource resource, string outputDirectory)
        {
            Directory.CreateDirectory(outputDirectory);

            var frames = resource.SpriteSet.Frames;
            var digits = Math.Max(4, frames.Count.ToString().Length);
            for (var i = 0; i < frames.Count; i++)
            {
                var filePath = Path.Combine(outputDirectory, "tile_" + i.ToString("D" + digits) + ".png");
                using (var bitmap = CreateBitmap(frames[i], resource.Palette))
                {
                    bitmap.Save(filePath, ImageFormat.Png);
                }
            }

            Console.WriteLine($"Directory: {outputDirectory}");
        }

        private static Bitmap CreateBitmap(Homm2SpriteFrame frame, Homm2Palette palette)
        {
            var bitmap = new Bitmap(frame.Width, frame.Height, PixelFormat.Format32bppArgb);
            for (var y = 0; y < frame.Height; y++)
            {
                for (var x = 0; x < frame.Width; x++)
                {
                    var pixelIndex = y * frame.Width + x;
                    var paletteIndex = frame.PaletteIndexes[pixelIndex];
                    var alpha = frame.Alpha[pixelIndex];
                    var color = Color.FromArgb(alpha, palette.Red[paletteIndex], palette.Green[paletteIndex], palette.Blue[paletteIndex]);
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("PixelHeroes.Homm2.TileDumper");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  PixelHeroes.Homm2.TileDumper.exe --agg <path-to-HEROES2.AGG> [--out <atlas.png>]");
            Console.WriteLine("  PixelHeroes.Homm2.TileDumper.exe --agg <path-to-HEROES2.AGG> --separate --out <directory>");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --agg <path>       Required. Path to HEROES2.AGG.");
            Console.WriteLine("  --out <path>       Output atlas PNG or output directory. Default: homm2_tiles.png.");
            Console.WriteLine("  --separate         Export one PNG per tile instead of one atlas PNG.");
            Console.WriteLine("  --columns <count>  Atlas columns. Default: 32.");
            Console.WriteLine("  --til <name>       AGG tile resource. Default: " + DefaultTileSetName + ".");
            Console.WriteLine("  --pal <name>       AGG palette resource. Default: " + DefaultPaletteName + ".");
        }

        private sealed class DumpOptions
        {
            public string AggPath { get; private set; }
            public string OutputPath { get; private set; } = "homm2_tiles.png";
            public string TileSetName { get; private set; } = DefaultTileSetName;
            public string PaletteName { get; private set; } = DefaultPaletteName;
            public bool SeparateFiles { get; private set; }
            public bool ShowHelp { get; private set; }
            public int Columns { get; private set; } = 32;

            public static DumpOptions Parse(string[] args)
            {
                var options = new DumpOptions();
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i];
                    switch (arg)
                    {
                        case "-h":
                        case "--help":
                            options.ShowHelp = true;
                            break;
                        case "--agg":
                            options.AggPath = RequireValue(args, ref i, arg);
                            break;
                        case "--out":
                            options.OutputPath = RequireValue(args, ref i, arg);
                            break;
                        case "--separate":
                            options.SeparateFiles = true;
                            if (options.OutputPath == "homm2_tiles.png")
                            {
                                options.OutputPath = "homm2_tiles";
                            }
                            break;
                        case "--columns":
                            options.Columns = int.Parse(RequireValue(args, ref i, arg));
                            break;
                        case "--til":
                            options.TileSetName = RequireValue(args, ref i, arg);
                            break;
                        case "--pal":
                            options.PaletteName = RequireValue(args, ref i, arg);
                            break;
                        default:
                            throw new ArgumentException("Unknown argument: " + arg);
                    }
                }

                if (!options.ShowHelp && string.IsNullOrWhiteSpace(options.AggPath))
                {
                    throw new ArgumentException("--agg is required.");
                }

                if (options.Columns <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(options.Columns));
                }

                return options;
            }

            private static string RequireValue(string[] args, ref int index, string optionName)
            {
                if (index + 1 >= args.Length)
                {
                    throw new ArgumentException(optionName + " requires a value.");
                }

                index++;
                return args[index];
            }
        }
    }
}

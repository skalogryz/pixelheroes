using PixelHeroes.Core.World;

public static class GameSession
{
    public static AdventureMap CurrentMap { get; private set; }

    public static void UseMap(AdventureMap map)
    {
        CurrentMap = map;
    }

    public static void UseDefaultMap()
    {
        CurrentMap = SampleMapFactory.CreateStartingMap();
    }

    public static AdventureMap TakeMapOrDefault()
    {
        if (CurrentMap != null)
        {
            return CurrentMap;
        }

        CurrentMap = SampleMapFactory.CreateStartingMap();
        return CurrentMap;
    }
}

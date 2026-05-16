# HoMM2 Map Data Not Represented In PixelHeroes.Core Yet

This list tracks HoMM2 MP2/MX2 data that is present in the source format but is not modeled in `AdventureMap`, `MapTile`, `MapItem`, `MapUnit`, or `SpecialLocation` yet. The importer intentionally does not add new domain properties for this information.

Sources used:

- fheroes2 MP2 header parsing: `readMP2Map` checks magic `0x5C000000`, reads difficulty, width, height, player availability, victory/loss metadata, map name and description.
- fheroes2 MP2 tile format: MP2 header size is 428 bytes; each tile is 20 bytes and contains terrain image index, object layer data, quantity fields, terrain flags, object type, add-on link and object UIDs.
- fheroes2 object type list: MP2 stores many concrete adventure-map object types beyond the current PixelHeroes categories.

Not represented yet:

- Map difficulty.
- Map description.
- Available player colors, human/computer player permissions, player races, alliances.
- Victory and loss conditions and their parameters.
- Whether the game starts with a hero in the first castle.
- Expansion/version distinction: `.mp2` Succession Wars vs `.mx2` Price of Loyalty.
- Original terrain families that do not have dedicated `TerrainType` values yet: desert, snow, swamp, wasteland/cracked terrain, lava, beach.
- Terrain image index and terrain shape/flags.
- Road overlays and road direction details.
- Per-tile object layer information: bottom/top ICN sets, sprite indices, layer type, animation/overlay flags.
- Add-on chunks chained from a tile by `nextAddonIndex`.
- Object UIDs for multi-tile objects.
- Exact HoMM2 object subtype metadata for many locations, artifacts, spells, signs, events, riddles, rumors, castles and heroes.
- Exact monster type, exact monster count semantics and monster disposition.
- Exact resource type/amount semantics for all pickup objects.
- Object ownership/player color for castles, mines and other capturable locations.
- Hero identity, army, artifacts, spells, portrait/class and custom text.
- Castle/town internals: faction, buildings, garrison and custom settings.

# Dungeon Crawler Project Memory

## Project Overview
- Unity 6 (6000.2.6f2) 2.5D dungeon crawler
- Top-down orthographic camera
- Procedural level generation with story level save/load
- Converting from 3D to 2D/2.5D

## Technical Stack
- Unity 6 with URP (Universal Render Pipeline)
- URP Asset: PC_RPAsset in Assets/Settings/
- Input System Package (New) — InputSystem_Actions.inputactions
- 2D Tilemap system for level rendering
- Cinemachine installed but not yet implemented
- 2D Pixel Perfect installed
- 2D Tilemap Extras installed
- AI Navigation package installed (legacy, being phased out)

## Project Structure
Assets/
├── 2D_Assets/
│   ├── Sprites/        — Hyptosis tile art batches 1-5 (32x32px)
│   ├── Tiles/
│   │   └── Dungeon/    — DungeonFloor and DungeonWall Rule Tiles
│   ├── Animations/     — future character animations
│   └── Decorations/    — future decoration tiles
├── _3D_Archive/        — archived 3D assets, to be deleted later
├── LevelConfigs/       — RoomLevelLayoutConfiguration ScriptableObjects
├── Materials/
├── procedural_levels/  — NavMesh assets (legacy)
├── Scenes/
├── Scripts/
│   ├── Level Decorator/
│   └── Utilities/
│       └── Editor/
├── Settings/           — URP pipeline assets
├── SpecialRoomConfigs/ — RoomTemplate ScriptableObjects
├── TileSets/           — Tileset ScriptableObjects
└── _3D_Archive/        — archived 3D assets

## Scene Hierarchy
procedural_levels (scene)
├── FollowCamera
│   └── Main Camera (Orthographic, Size:10, Z:-10)
├── SharedLevelData
├── Directional Light
├── LevelLayout         — black/white layout preview display
├── LevelBuilder        — main generation controller
├── LayoutGeneratorRooms
├── LevelGeometryGeneration
├── RoomDecorator
├── LevelGeometry       — MarchingSquares component
├── Grid
│   └── DungeonTilemap  — main tilemap with collision
└── Player              — Rigidbody2D, DirectedAgent, SpriteRenderer

## Key Scripts
### DO NOT MODIFY (core generation logic, dimension-agnostic)
- LayoutGeneratorRooms.cs — procedural room/hallway generation
- SharedLevelData.cs — seed management, scale=1
- RoomDecorator.cs — decoration placement
- PatternMatchingDecoratorRule.cs — decoration rules
- BaseDecoratorRule.cs — decoration base class
- TileType.cs — tile type enum
- Level.cs, Room.cs, Hallway.cs, RoomTemplate.cs — data classes

### Modified for 2D conversion
- MarchingSquares.cs — reads Texture2D, writes to Tilemap
  - Uses TextureBasedLevel to read level data
  - Places tiles at Vector3Int(x, y, 0) — full level size, no border trim
  - Y axis matches layout preview orientation exactly
  - Sets tilemap.tileAnchor = (0.5, 0.5, 0) at runtime
  - Tilemap reference: DungeonTilemap
- LevelBuilder.cs — orchestrates generation
  - Removed NavMesh, uses 2D player spawn
  - Has floorTile and wallTile SerializeField references
  - Spawns player using spiral search for floor tile (tile == floorTile)
  - FindFloorSpawn uses raw roomCenter coords (no offset — tiles placed at (x,y,0) directly)
  - SpawnPlayerDelayed coroutine: sets Kinematic, positions player, waits 0.5s, restores Dynamic
  - GetStartRoomRect has safety clamps: room size capped at levelConfig/4, available range Min(1,...)
- DirectedAgent.cs — 2D player controller
  - Rigidbody2D movement, no NavMeshAgent
  - Uses InputSystem_Actions for input
  - Freeze Rotation Z enabled in Awake
  - Stops on input deadzone < 0.1
- FollowCamera.cs — Lerp-based camera follow in LateUpdate
- TileVariant.cs — stores TileBase[] instead of GameObject[]
- Tileset.cs — GameTile() returns TileBase instead of GameObject

### New scripts (2D conversion)
- LevelSaveData.cs — serializable save data (levelId, seed, levelType, placedItems)
- ItemSaveState.cs — item position save data (itemId, gridPosition)
- LevelSaveManager.cs — JSON save/load to Application.persistentDataPath
  - Uses reflection to read/write SharedLevelData.seed (private field)

## Tilemap Configuration (DungeonTilemap)
- Tile Anchor: X:0.5, Y:0.5, Z:0 (set at runtime by MarchingSquares)
- Tilemap Collider 2D:
  - Composite Operation: Merge
  - Offset: X:0, Y:0
  - Use Delaunay Mesh: false
- Composite Collider 2D:
  - Geometry Type: Polygons
  - Generation Type: Synchronous
- Rigidbody2D: Static
- Sorting Layer: Ground, Order: 0
- Chunk Culling Bounds: Manual, X:100, Y:100

## Tile Assets
- DungeonFloor (Rule Tile)
  - Sprite: hyptosis_tile-art-batch-1_13
  - Default Collider: None
  - Tiling Rules Collider: None
  - Assigned to Tileset indices 0-14
- DungeonWall (Rule Tile)
  - Sprite: hyptosis_tile-art-batch-1_17
  - Default Collider: Grid
  - Tiling Rules Collider: Grid
  - Assigned to Tileset index 15

## Player Configuration
- Tag: Player
- Sprite Renderer: Knob (placeholder), Sorting Layer: Default, Order: 5
- Rigidbody2D: Dynamic, Gravity Scale: 0, Freeze Rotation Z
- Capsule Collider 2D
- DirectedAgent script, Move Speed: 5

## Level Config (Level 0 1 - active config)
- Width: 64, Length: 64
- Max Room Count: 25
- Door Distance From Edge: 1
- Hallway Length Min: 1, Max: 10
- Room Templates: SmallRoom(10), LargeRoom(3),
  SpecialRoom1-4 with layout textures

## Sprite Sheets
- Hyptosis tile art batches 1-5
- All imported as 32x32 grid sliced sprites
- Pixels Per Unit: 32
- Filter Mode: Point (no filter)
- Compression: None

## Known Issues / TODO
- Player sprite is placeholder (white Knob)
- No character animations yet
- Rule Tile neighbor rules not configured (walls use default sprite only)
- _3D_Archive folder needs cleanup when 3D assets confirmed unused
- DontDestroyOnLoad object in scene needs investigation
- NavMesh components may still exist on some GameObjects
- Directional Light should be replaced with 2D lights eventually
- AI Navigation package can be removed once confirmed unused
- ~~Layout map and tilemap orientation mismatch~~ — RESOLVED: Y axis no longer flipped, both match

## Next Steps (Priority Order)
1. Player character sprite and animations
2. Rule Tile wall polish (corner/edge variants)
3. Enemy system
4. Combat system
5. Save/load story levels
6. UI/HUD (health, minimap, inventory)
7. Multiple dungeon tile themes
8. Sound effects and music

## Packages Installed
- Universal RP (URP)
- Input System
- AI Navigation (legacy)
- Cinemachine
- 2D Sprite
- 2D Tilemap Extras
- 2D Pixel Perfect
- Custom NUnit
- Test Framework
- Visual Studio Editor
- Multiplayer Center

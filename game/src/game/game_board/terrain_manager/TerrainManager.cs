namespace HOB;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Collections.Generic;
using HexGridMap;
using HOB.GameEntity;
using RaycastSystem;

[GlobalClass, Tool]
public partial class TerrainManager : Node3D {
  [Export] private Material TerrainMaterial { get; set; } = default!;
  [Export] private Material WaterMaterial { get; set; } = default!;

  private Image TerrainData { get; set; } = default!;
  private Image HighlightData { get; set; } = default!;
  private ImageTexture HighlightDataTexture { get; set; } = default!;
  private Texture2DArray HexTextures { get; set; } = default!;
  private Image TextureLookup { get; set; } = default!;

  private Vector2I ChunkCount { get; set; }
  private Vector2I ChunkSize { get; set; }
  private Chunk[] Chunks { get; set; }
  private GameGrid Grid { get; set; }

  private System.Collections.Generic.Dictionary<PropSetting, List<(Mesh Mesh, Material Material, Transform3D Transform)>> _multiMeshCache = new();
  private System.Collections.Generic.Dictionary<Chunk, System.Collections.Generic.Dictionary<(Mesh Mesh, Material Material), List<Transform3D>>> _chunkMeshGroups = new();

  public override void _PhysicsProcess(double delta) {
    var position = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World)?.Position;
    if (position != null) {
      TerrainMaterial.Set("shader_parameter/mouse_world_pos", new Vector2(position.Value.X, position.Value.Z));
    }
  }

  public void CreateData(GameGrid grid, MissionData mission) {
    Grid = grid;

    ChunkSize = new(16, 16);
    var cols = grid.MapData.Cols;
    var rows = grid.MapData.Rows;

    if (ChunkSize.X > cols) {
      ChunkSize = new(cols, ChunkSize.Y);
    }

    if (ChunkSize.Y > rows) {
      ChunkSize = new(ChunkSize.X, rows);
    }

    while (cols % ChunkSize.X != 0 || rows % ChunkSize.Y != 0) {
      if (cols % ChunkSize.X != 0) {
        ChunkSize = new(ChunkSize.X + 1, ChunkSize.Y);
      }

      if (rows % ChunkSize.Y != 0) {
        ChunkSize = new(ChunkSize.X, ChunkSize.Y + 1);
      }
    }

    ChunkCount = new(cols / ChunkSize.X, rows / ChunkSize.Y);

    Chunks = new Chunk[ChunkCount.X * ChunkCount.Y];
    for (var i = 0; i < Chunks.Length; i++) {
      var chunk = new Chunk(i, ChunkSize, TerrainMaterial, WaterMaterial, TerrainMaterial, grid);
      Chunks[i] = chunk;
      AddChild(chunk);
    }

    InitializeImageData(cols, rows);
    InitializeTextures(grid);
    UpdateTerrainTextureData();

    HighlightDataTexture = ImageTexture.CreateFromImage(HighlightData);
    UpdateHighlights();

    TerrainMaterial.Set("shader_parameter/grid_size", new Vector2I(grid.MapData.Cols, grid.MapData.Rows));
    TerrainMaterial.Set("shader_parameter/hex_textures", HexTextures);


    // TODO: place props after the chunk has generated and add visibility ranges for them, they take most of the frame time
    PlaceProps(mission);
    FinalizeChunkMultiMeshes();
  }

  private void InitializeImageData(int cols, int rows) {
    TerrainData = Image.CreateEmpty(cols, rows, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(cols, rows, false, Image.Format.Rgba8);
    TextureLookup = Image.CreateEmpty(cols, rows, false, Image.Format.R8);

    TextureLookup.Fill(Color.Color8(0, 0, 0, 0));
    TerrainData.Fill(Colors.Transparent);
  }

  private void InitializeTextures(GameGrid grid) {
    var settings = grid.MapData.Settings.CellSettings;
    var textureSize = new Vector2I(1080, 1080);
    var empty = Image.CreateEmpty(textureSize.X, textureSize.Y, true, Image.Format.Rgb8);
    empty.Fill(Colors.White);

    var textures = new Godot.Collections.Array<Image>();
    foreach (var setting in settings) {
      if (setting.Texture != null) {
        var img = setting.Texture.GetImage();
        textures.Add(img);
      }
      else {
        textures.Add(empty);
      }
    }

    HexTextures = new();
    HexTextures.CreateFromImages(textures);

    foreach (var hex in grid.GetCells()) {
      var setting = hex.GetSetting();
      var index = System.Array.IndexOf(settings.ToArray(), setting) + 1;
      if (setting.Texture == null) {
        index = 0;
      }

      TextureLookup.SetPixel(hex.OffsetCoord.Col, hex.OffsetCoord.Row, Color.Color8((byte)index, 0, 0, 255));
      SetTerrainPixel(new(hex.OffsetCoord.Col, hex.OffsetCoord.Row), setting.Color);
    }

    var lookupTexture = ImageTexture.CreateFromImage(TextureLookup);
    TerrainMaterial.Set("shader_parameter/hex_texture_lookup", lookupTexture);
  }

  public void SetMouseHighlight(bool value) {
    TerrainMaterial.Set("shader_parameter/show_mouse_highlight", value);
  }

  public void SetHighlight(GameCell cell, Color color) {
    SetHighlighPixel(cell.OffsetCoord, color);
  }

  public void UpdateHighlights() {
    UpdateHighlightTextureData();
  }

  public void ClearHighlights() {
    HighlightData.Fill(Colors.Transparent);
  }

  public (Chunk chunk, OffsetCoord localCoord) OffsetToChunk(OffsetCoord coord) {
    var chunkX = coord.Col / ChunkSize.X;
    var chunkY = coord.Row / ChunkSize.Y;
    var localX = coord.Col - chunkX * ChunkSize.X;
    var localY = coord.Row - chunkY * ChunkSize.Y;
    return (Chunks[chunkX + (chunkY * ChunkCount.X)], new(localX, localY));
  }

  public void AddCellToChunk(GameCell cell) {
    var (chunk, localCoord) = OffsetToChunk(cell.OffsetCoord);
    chunk.AddCell(localCoord.Col + localCoord.Row * ChunkSize.X, Grid.GetCellIndex(cell));
  }

  private void SetHighlighPixel(OffsetCoord offset, Color color) {
    if (offset.Col >= 0 && offset.Col < HighlightData.GetSize().X &&
        offset.Row >= 0 && offset.Row < HighlightData.GetSize().Y) {
      HighlightData.SetPixel(offset.Col, offset.Row, color);
    }
  }

  private void SetTerrainPixel(OffsetCoord offset, Color color) {
    if (offset.Col >= 0 && offset.Col < HighlightData.GetSize().X &&
        offset.Row >= 0 && offset.Row < HighlightData.GetSize().Y) {
      TerrainData.SetPixel(offset.Col, offset.Row, color);
    }
  }

  private void UpdateTerrainTextureData() {
    var texture = ImageTexture.CreateFromImage(TerrainData);
    TerrainMaterial.Set("shader_parameter/terrain_data_texture", texture);
  }

  private void UpdateHighlightTextureData() {
    HighlightDataTexture.Update(HighlightData);
    TerrainMaterial.Set("shader_parameter/highlight_data_texture", HighlightDataTexture);
  }

  private void PlaceProps(MissionData mission) {
    var rng = new RandomNumberGenerator();
    var settingsGroups = GroupCellsByPropSettings();
    var occupiedCells = GetOccupiedCells(mission);

    InitializeChunkMeshGroups();

    foreach (var kvp in settingsGroups) {
      var propSetting = kvp.Key;
      var cells = kvp.Value;
      var meshDataList = GetCachedMultiMeshData(propSetting);

      foreach (var cell in cells) {
        if (occupiedCells.Contains(cell.OffsetCoord) || rng.Randf() * 100 > propSetting.Chance) {
          continue;
        }

        var (chunk, _) = OffsetToChunk(cell.OffsetCoord);
        if (!_chunkMeshGroups.TryGetValue(chunk, out var chunkMeshGroups)) {
          continue;
        }

        PlacePropsInCell(cell, propSetting, meshDataList, chunkMeshGroups, rng);
      }
    }
  }

  private void InitializeChunkMeshGroups() {
    foreach (var chunk in Chunks) {
      _chunkMeshGroups[chunk] = new System.Collections.Generic.Dictionary<(Mesh Mesh, Material Material), List<Transform3D>>();
    }
  }

  private void PlacePropsInCell(GameCell cell, PropSetting propSetting,
      List<(Mesh Mesh, Material Material, Transform3D Transform)> meshDataList,
      System.Collections.Generic.Dictionary<(Mesh Mesh, Material Material), List<Transform3D>> chunkMeshGroups,
      RandomNumberGenerator rng) {
    for (var i = 0; i < rng.RandiRange(propSetting.AmountRange.X, propSetting.AmountRange.Y); i++) {
      var worldPos = cell.GetRealPosition() +
               new Vector3(
                   (float)rng.RandfRange(-1f, 1f),
                   0,
                   (float)rng.RandfRange(-1f, 1f)
               );

      var scale = Vector3.One * rng.RandfRange(propSetting.ScaleRange.X, propSetting.ScaleRange.Y);
      var rotation = rng.RandfRange(Mathf.DegToRad(0), Mathf.DegToRad(360));

      foreach (var (mesh, material, initialTransform) in meshDataList) {
        if (!chunkMeshGroups.TryGetValue((mesh, material), out var transforms)) {
          transforms = new List<Transform3D>();
          chunkMeshGroups[(mesh, material)] = transforms;
        }

        var finalTransform = Transform3D.Identity
                  .Rotated(Vector3.Up, rotation)
                  .Scaled(scale)
                  .Translated(worldPos)
                  * initialTransform;

        transforms.Add(finalTransform);
      }
    }
  }

  private void FinalizeChunkMultiMeshes() {
    foreach (var chunk in Chunks) {
      if (!_chunkMeshGroups.TryGetValue(chunk, out var meshGroups)) {
        continue;
      }

      foreach (var kvp in meshGroups) {
        if (kvp.Value.Count == 0) {
          continue;
        }

        var multimesh = new MultiMesh {
          Mesh = kvp.Key.Mesh,
          TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
          InstanceCount = kvp.Value.Count
        };

        if (kvp.Key.Material != null) {
          multimesh.Mesh.SurfaceSetMaterial(0, kvp.Key.Material);
        }

        for (var i = 0; i < kvp.Value.Count; i++) {
          multimesh.SetInstanceTransform(i, kvp.Value[i]);
        }


        chunk.AddChild(new MultiMeshInstance3D { Multimesh = multimesh, Position = chunk.GetCenter() with { Y = 0 } });
      }
      GD.Print("Center: " + chunk.GetCenter());
    }

    _chunkMeshGroups.Clear();
  }

  private HashSet<OffsetCoord> GetOccupiedCells(MissionData mission) {
    var occupiedCells = new HashSet<OffsetCoord>();

    foreach (var playerData in mission.PlayerSpawnDatas) {
      foreach (var entity in playerData.SpawnedEntities) {
        foreach (var spawnAt in entity.SpawnAt) {
          occupiedCells.Add(new(spawnAt.X, spawnAt.Y));
        }
      }
    }

    return occupiedCells;
  }

  private List<(Mesh Mesh, Material Material, Transform3D Transform)> GetCachedMultiMeshData(PropSetting setting) {
    if (_multiMeshCache.TryGetValue(setting, out var cached)) {
      return cached;
    }

    using var scene = setting.PropScene.Instantiate<Node3D>();
    var meshInstances = GetAllMeshInstances(scene);
    var result = new List<(Mesh, Material, Transform3D)>();

    foreach (var meshInstance in meshInstances) {
      var transform = CalculateFullTransform(meshInstance, scene);
      result.Add((
          meshInstance.Mesh,
          meshInstance.GetActiveMaterial(0),
          transform
      ));
    }

    _multiMeshCache[setting] = result;
    return result;
  }

  private List<MeshInstance3D> GetAllMeshInstances(Node root) {
    var instances = new List<MeshInstance3D>();
    var queue = new Queue<Node>();
    queue.Enqueue(root);

    while (queue.Count > 0) {
      var node = queue.Dequeue();
      if (node is MeshInstance3D meshInstance) {
        instances.Add(meshInstance);
      }

      foreach (var child in node.GetChildren()) {
        queue.Enqueue(child);
      }
    }

    return instances;
  }

  private Transform3D CalculateFullTransform(Node3D node, Node3D root) {
    var transform = node.Transform;
    var current = node.GetParent();

    while (current != null && current != root) {
      if (current is Node3D node3d) {
        transform = node3d.Transform * transform;
      }
      current = current.GetParent();
    }

    return transform;
  }

  private System.Collections.Generic.Dictionary<PropSetting, List<GameCell>> GroupCellsByPropSettings() {
    var settingsDict = new System.Collections.Generic.Dictionary<PropSetting, List<GameCell>>();

    foreach (var cell in Grid.GetCells()) {
      foreach (var propSetting in cell.GetSetting().Props) {
        if (!settingsDict.TryGetValue(propSetting, out var cells)) {
          cells = new List<GameCell>();
          settingsDict[propSetting] = cells;
        }
        cells.Add(cell);
      }
    }

    return settingsDict;
  }
}

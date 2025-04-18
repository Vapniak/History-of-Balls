namespace HOB;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using HexGridMap;
using HOB.GameEntity;
using RaycastSystem;


[GlobalClass, Tool]
public partial class TerrainManager : Node3D {
  [Export] private Material TerrainMaterial { get; set; }
  [Export] private Material WaterMaterial { get; set; }

  private Image TerrainData { get; set; }
  private Image HighlightData { get; set; }
  private Texture2DArray HexTextures { get; set; }
  private Image TextureLookup { get; set; }


  private Vector2I ChunkCount { get; set; }
  private Vector2I ChunkSize { get; set; }

  private Chunk[] Chunks { get; set; }

  private GameGrid Grid { get; set; }


  public override void _PhysicsProcess(double delta) {
    var position = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World)?.Position;
    if (position != null) {
      TerrainMaterial.Set("shader_parameter/mouse_world_pos", new Vector2(position.Value.X, position.Value.Z));
    }
  }

  public void CreateData(GameGrid grid) {
    Grid = grid;

    ChunkSize = new(16, 16);
    // TODO: add chunk buffering loading and unloading when player moves
    var cols = grid.MapData.Cols;
    var rows = grid.MapData.Rows;

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
      var chunk = new Chunk(i, ChunkSize, TerrainMaterial, WaterMaterial, grid);
      Chunks[i] = chunk;
      AddChild(chunk);
    }

    TerrainData = Image.CreateEmpty(cols, rows, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(cols, rows, false, Image.Format.Rgba8);
    TextureLookup = Image.CreateEmpty(cols, rows, false, Image.Format.R8);
    TextureLookup.Fill(Color.Color8(0, 0, 0, 0));


    TerrainData.Fill(Colors.Transparent);

    var settings = grid.MapData.Settings.CellSettings;

    var textureSize = new Vector2I(1080, 1080);
    var empty = Image.CreateEmpty(textureSize.X, textureSize.Y, false, Image.Format.Rgb8);
    empty.Fill(Colors.White);
    var textures = new Array<Image>();
    foreach (var setting in settings) {
      if (setting.Texture != null) {
        var img = setting.Texture.GetImage();
        if (img.GetSize() != textureSize) {
          // img.Resize(textureSize.X, textureSize.Y);
        }
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

    UpdateTerrainTextureData();

    UpdateHighlights();

    TerrainMaterial.Set("shader_parameter/grid_size", new Vector2I(grid.MapData.Cols, grid.MapData.Rows));

    var lookupTexture = ImageTexture.CreateFromImage(TextureLookup);
    TerrainMaterial.Set("shader_parameter/hex_texture_lookup", lookupTexture);

    TerrainMaterial.Set("shader_parameter/hex_textures", HexTextures);
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
    if (offset.Col >= 0 && offset.Col < HighlightData.GetSize().X && offset.Row >= 0 && offset.Row < HighlightData.GetSize().Y) {
      HighlightData.SetPixel(offset.Col, offset.Row, color);
    }
  }

  private void SetTerrainPixel(OffsetCoord offset, Color color) {
    if (offset.Col >= 0 && offset.Col < HighlightData.GetSize().X && offset.Row >= 0 && offset.Row < HighlightData.GetSize().Y) {
      TerrainData.SetPixel(offset.Col, offset.Row, color);
    }
  }

  private void UpdateTerrainTextureData() {
    var texture = ImageTexture.CreateFromImage(TerrainData);
    TerrainMaterial.Set("shader_parameter/terrain_data_texture", texture);
  }

  private void UpdateHighlightTextureData() {
    var texture = ImageTexture.CreateFromImage(HighlightData);
    TerrainMaterial.Set("shader_parameter/highlight_data_texture", texture);
  }
}

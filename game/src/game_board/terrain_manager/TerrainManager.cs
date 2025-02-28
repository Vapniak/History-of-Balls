namespace HOB;

using Godot;
using HexGridMap;
using RaycastSystem;


[GlobalClass]
public partial class TerrainManager : Node3D {
  [Export] private Material TerrainMaterial { get; set; }

  private Image TerrainData { get; set; }
  private Image HighlightData { get; set; }

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
      var chunk = new Chunk(i, ChunkSize, TerrainMaterial, grid);
      Chunks[i] = chunk;
      AddChild(chunk);
    }

    TerrainData = Image.CreateEmpty(cols, rows, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(cols, rows, false, Image.Format.Rgba8);

    TerrainData.Fill(Colors.Transparent);

    foreach (var hex in grid.MapData.GetCells()) {
      var setting = grid.MapData.Settings.CellSettings[hex.Id];
      SetTerrainPixel(new(hex.Col, hex.Row), setting.Color);
    }

    UpdateTerrainTextureData();

    UpdateHighlights();

    TerrainMaterial.Set("shader_parameter/grid_size", new Vector2I(grid.MapData.Cols, grid.MapData.Rows));
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

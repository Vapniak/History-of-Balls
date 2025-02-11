namespace HOB;

using Godot;
using HexGridMap;
using RaycastSystem;

[GlobalClass]
public partial class TerrainManager : Node3D {
  [Export] private MeshInstance3D TerrainMesh { get; set; }
  private Material TerrainMaterial { get; set; }

  private Image TerrainData { get; set; }
  private Image HighlightData { get; set; }

  public override void _PhysicsProcess(double delta) {
    var position = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World)?.Position;
    if (position != null) {
      TerrainMaterial.Set("shader_parameter/mouse_world_pos", new Vector2(position.Value.X, position.Value.Z));
    }
  }


  // TODO: divide the terrain to chunks and only update the texture in chunk
  public void CreateData(MapData mapData, Vector2 realMapSize) {
    TerrainData = Image.CreateEmpty(mapData.Cols, mapData.Rows, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(mapData.Cols, mapData.Rows, false, Image.Format.Rgba8);

    TerrainData.Fill(Colors.Transparent);

    foreach (var hex in mapData.GetCells()) {
      var setting = mapData.Settings.CellSettings[hex.Id];
      SetTerrainPixel(new(hex.Col, hex.Row), setting.Color);
    }

    TerrainMaterial = TerrainMesh.GetActiveMaterial(0);

    ((PlaneMesh)TerrainMesh.Mesh).Size = realMapSize * 10;

    UpdateTerrainTextureData();

    UpdateHighlights();
  }

  public void SetMouseHighlight(bool value) {
    TerrainMaterial.Set("shader_parameter/show_mouse_highlight", value);
  }

  // FIXME: clicking lags game on large map
  public void SetHighlight(GameCell cell, Color color) {
    SetHighlighPixel(cell.OffsetCoord, color);
  }

  public void UpdateHighlights() {
    UpdateHighlightTextureData();
  }

  public void ClearHighlights() {
    HighlightData.Fill(Colors.Transparent);
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

namespace HOB;

using System.Runtime.InteropServices;
using Godot;
using HexGridMap;

[GlobalClass]
public partial class TerrainManager : Node {
  public enum EdgeType {
    Flat, // ELEVATION DIFF 0
    Slope, // ELEVATION DIFF 1
    Hill // ELEVATION DIFF > 1
  }
  [Signal] public delegate void TerrainDataTextureChangedEventHandler(ImageTexture texture);
  [Signal] public delegate void HighlightDataTextureChangedEventHandler(ImageTexture texture);

  private ImageTexture TerrainDataTexture { get; set; }
  private Image TerrainData { get; set; }

  private ImageTexture HighlightDataTexture { get; set; }
  private Image HighlightData { get; set; }

  // TODO: divide the terrain to chunks and only update the texture in chunk
  public void CreateData(MapData mapData) {
    TerrainData = Image.CreateEmpty(mapData.Cols, mapData.Rows, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(mapData.Cols, mapData.Rows, false, Image.Format.Rgba8);

    // TODO: offset all coords so they fit in texture and start from 0 offset

    TerrainData.Fill(Colors.Transparent);

    foreach (var hex in mapData.GetCells()) {
      var setting = mapData.Settings.CellSettings[hex.Id];
      SetTerrainPixel(new(hex.Col, hex.Row), setting.Color);
    }

    UpdateTerrainTextureData();

    UpdateHighlights();
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
    TerrainDataTexture = ImageTexture.CreateFromImage(TerrainData);
    EmitSignal(SignalName.TerrainDataTextureChanged, TerrainDataTexture);
  }

  private void UpdateHighlightTextureData() {
    HighlightDataTexture = ImageTexture.CreateFromImage(HighlightData);
    EmitSignal(SignalName.HighlightDataTextureChanged, HighlightDataTexture);
  }
}

namespace HOB;

using Godot;
using HexGridMap;

public partial class TerrainManager : Node {
  [Signal] public delegate void TerrainDataTextureChangedEventHandler(ImageTexture texture);
  [Signal] public delegate void HighlightDataTextureChangedEventHandler(ImageTexture texture);

  private ImageTexture TerrainDataTexture { get; set; }
  private Image TerrainData { get; set; }


  private ImageTexture HighlightDataTexture { get; set; }
  private Image HighlightData { get; set; }

  public void CreateData(int width, int height) {
    TerrainData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);

    TerrainData.Fill(Colors.DarkOliveGreen);

    UpdateHighlightTextureData();
    UpdateTerrainTextureData();
  }

  // TODO: add colors to cells
  public void ClearHighlights() {
    HighlightData.Fill(Colors.Transparent);
  }

  public void AddHighlightToCells(HexCell[] cells) {
    if (cells != null) {
      foreach (var cell in cells) {
        SetHighlighPixel(cell.GetOffsetCoord(), Colors.White);
      }
    }
  }

  public void RemoveHighlightFromCells(HexCell[] cells) {
    if (cells != null) {
      foreach (var cell in cells) {
        SetHighlighPixel(cell.GetOffsetCoord(), Colors.Transparent);
      }
    }
  }

  public void UpdateHighlights() {
    UpdateHighlightTextureData();
  }

  private void SetHighlighPixel(OffsetCoord offset, Color color) {
    if (offset.Col >= 0 && offset.Col < HighlightData.GetSize().X && offset.Row >= 0 && offset.Row < HighlightData.GetSize().Y) {
      HighlightData.SetPixel(offset.Col, offset.Row, color);
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

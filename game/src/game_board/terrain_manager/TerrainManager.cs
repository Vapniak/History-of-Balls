namespace HOB;

using System.Data;
using Godot;
using HexGridMap;

public partial class TerrainManager : Node {
  [Signal] public delegate void TerrainDataTextureChangedEventHandler(ImageTexture texture);
  [Signal] public delegate void HighlightDataTextureChangedEventHandler(ImageTexture texture);

  private ImageTexture TerrainDataTexture { get; set; }
  private Image TerrainData { get; set; }


  private ImageTexture HighlightDataTexture { get; set; }
  private Image HighlightData { get; set; }

  public void CreateData(int width, int height, GameCell[] cells) {
    TerrainData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);

    //TerrainData.Fill(Colors.Transparent);

    // TODO: offset all coords so they fit in texture and start from 0 offset
    // foreach (var cell in cells) {
    //   TerrainData.SetPixel(cell.OffsetCoord().Col, cell.OffsetCoord().Row, Colors.DarkOliveGreen);
    // }

    UpdateHighlightTextureData();
    //UpdateTerrainTextureData();
  }

  // TODO: add colors to cells
  public void ClearHighlights() {
    HighlightData.Fill(Colors.Transparent);
  }

  public void AddHighlightToCells(GameCell[] cells) {
    if (cells != null) {
      foreach (var cell in cells) {
        SetHighlighPixel(cell.OffsetCoord, Colors.White);
      }
    }
  }

  public void RemoveHighlightFromCells(GameCell[] cells) {
    if (cells != null) {
      foreach (var cell in cells) {
        SetHighlighPixel(cell.OffsetCoord, Colors.Transparent);
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

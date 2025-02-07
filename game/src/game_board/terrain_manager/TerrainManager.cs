namespace HOB;

using System;
using Godot;
using Godot.Collections;
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

  private GameCell[] Cells { get; set; }

  public void CreateData(int width, int height) {
    TerrainData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);

    // TODO: offset all coords so they fit in texture and start from 0 offset
  }

  public void UpdateData(GameCell[] cells) {
    TerrainData.Fill(Colors.Transparent);
    Cells = cells;

    foreach (var cell in cells) {
      SetTerrainPixel(cell.OffsetCoord, cell.TerrainColor);
    }

    UpdateTerrainTextureData();

    UpdateHighlights();
  }

  public void UpdateHighlights() {
    foreach (var cell in Cells) {
      SetHighlighPixel(cell.OffsetCoord, cell.HighlightColor);
    }

    UpdateHighlightTextureData();
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

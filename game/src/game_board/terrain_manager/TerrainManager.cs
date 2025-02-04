namespace HOB;

using System;
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

  private GameCell[] Cells { get; set; }

  public void CreateData(int width, int height, GameCell[] cells) {
    TerrainData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);


    Cells = cells;
    TerrainData.Fill(Colors.Transparent);

    // TODO: offset all coords so they fit in texture and start from 0 offset

    Random rnd = new();
    foreach (var cell in cells) {
      cell.MoveCost = rnd.Next(0, 3);
      TerrainData.SetPixel(cell.OffsetCoord.Col, cell.OffsetCoord.Row, new Color(cell.MoveCost == 0 ? 0 : 1f / cell.MoveCost, 0, 0));
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

  private void UpdateTerrainTextureData() {
    TerrainDataTexture = ImageTexture.CreateFromImage(TerrainData);
    EmitSignal(SignalName.TerrainDataTextureChanged, TerrainDataTexture);
  }

  private void UpdateHighlightTextureData() {
    HighlightDataTexture = ImageTexture.CreateFromImage(HighlightData);
    EmitSignal(SignalName.HighlightDataTextureChanged, HighlightDataTexture);
  }
}

namespace HOB;

using System;
using Godot;
using Godot.Collections;
using HexGridMap;

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

  //TODO: we should not update them but read from map data file
  public void UpdateData(GameCell[] cells, MapData mapData) {
    TerrainData.Fill(Colors.Transparent);
    Cells = cells;

    var i = 0;
    foreach (var cell in cells) {
      TerrainData.SetPixel(cell.OffsetCoord.Col, cell.OffsetCoord.Row, mapData.HexList[i].Color);
      i++;
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

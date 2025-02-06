namespace HOB;

using System;
using System.Data;
using Godot;
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

  private readonly Random _rnd;

  public TerrainManager() {
    _rnd = new();
  }
  public void CreateData(int width, int height) {
    TerrainData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);

    // TODO: offset all coords so they fit in texture and start from 0 offset
  }

  //TODO: we should not update them but read from map data file
  public void UpdateData(GameCell[] cells) {
    TerrainData.Fill(Colors.Transparent);
    Cells = cells;
    foreach (var cell in cells) {
      TerrainData.SetPixel(cell.OffsetCoord.Col, cell.OffsetCoord.Row, new Color(cell.MoveCost == 0 ? 0 : 1f / cell.MoveCost, 0f, 0f));
    }

    UpdateTerrainTextureData();

    UpdateHighlights();
  }
  public GameCell CreateCell(CubeCoord coord, HexLayout layout) {
    var cell = new GameCell(coord, layout) {
      // TODO: get from data
      MoveCost = _rnd.Next(0, 3)
    };

    return cell;
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

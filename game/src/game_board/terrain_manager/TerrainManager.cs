namespace HOB;

using Godot;
using HexGridMap;
using System;

public partial class TerrainManager : Node {
  public GameBoard GameBoard { get; set; }

  public ImageTexture TerrainDataTexture { get; private set; }
  private Image TerrainData { get; set; }


  public void CreateData(int width, int height) {
    TerrainData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);

    TerrainData.SetPixel(0, 0, Colors.White);
    UpdateTextureData();
  }
  public void HighlightCells(HexOffsetCoordinates[] coords) {

  }

  private void UpdateTextureData() {
    TerrainDataTexture = ImageTexture.CreateFromImage(TerrainData);
  }
}

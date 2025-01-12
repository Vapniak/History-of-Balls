namespace HOB;

using Godot;
using HexGridMap;

public partial class TerrainManager : Node {
  [Signal] public delegate void TerrainDataTextureChangedEventHandler(ImageTexture texture);
  [Signal] public delegate void HighlightDataTextureChangedEventHandler(ImageTexture texture);
  public GameBoard GameBoard { get; set; }

  public ImageTexture TerrainDataTexture { get; private set; }
  private Image TerrainData { get; set; }


  public ImageTexture HighlightDataTexture { get; private set; }
  private Image HighlightData { get; set; }

  public void CreateData(int width, int height) {
    TerrainData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
    HighlightData = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);

    TerrainData.Fill(Colors.LawnGreen);


    UpdateHighlightTextureData();
    UpdateTerrainTextureData();
  }
  public void HighlightCells(HexCoordinates[] coords) {
    HighlightData.Fill(Colors.Transparent);
    foreach (var coord in coords) {
      var offset = coord.Roffset(Offset.Even);
      if (offset.Col >= 0 && offset.Col < HighlightData.GetSize().X && offset.Row >= 0 && offset.Row < HighlightData.GetSize().Y) {
        HighlightData.SetPixel(offset.Col, offset.Row, Colors.White);
      }
    }

    UpdateHighlightTextureData();
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

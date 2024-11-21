namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class ImageMapShape : MapShape {
  [Export] public Texture2D HeightMap { get; private set; }
  public ImageMapShape() {

  }

  public ImageMapShape(Texture2D texture) {
    HeightMap = texture;
  }
  public override void BuildMap(HexGridMap gridMap) {
    var image = HeightMap.GetImage();
    image.Decompress();

    var left = 0;
    var right = image.GetWidth() / 10;
    var top = 0;
    var bottom = image.GetHeight() / 10;

    var iteration = 100;
    for (var r = top; r <= bottom; r++) {
      var rOffset = r >> 1;
      for (var q = left - rOffset; q <= right - rOffset; q++) {
        var color = image.GetPixel(iteration % image.GetWidth(), iteration / image.GetWidth());
        gridMap.AddHex(new(new(q, r, -q - r), color.Luminance));
        //GD.Print(iteration % image.GetWidth(), " ", iteration / image.GetWidth());
        iteration++;
      }
    }
  }
}

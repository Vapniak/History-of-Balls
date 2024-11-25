namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class ImageMapShape : MapShape {
  [Export] public Texture2D HeightMap { get; private set; }
  [Export] public int Size { get; private set; } = 100;
  public ImageMapShape() {

  }

  public ImageMapShape(Texture2D texture) {
    HeightMap = texture;
  }
  public override void BuildMap(HexGridMap gridMap) {
    var image = HeightMap.GetImage();
    image.Decompress();

    image.Resize(Size, Size);


    var left = 0;
    var right = image.GetWidth() - 1;
    var top = 0;
    var bottom = image.GetHeight() - 1;

    var iteration = 0;
    for (var r = top; r <= bottom; r++) {
      var rOffset = r >> 1;
      for (var q = left - rOffset; q <= right - rOffset; q++) {
        var color = image.GetPixel(iteration % image.GetWidth(), iteration / image.GetWidth());
        //FIXME: temp elevation
        gridMap.AddHex(new(new(q, r, -q - r), (int)(color.Luminance * 10)));
        //GD.Print(iteration % image.GetWidth(), " ", iteration / image.GetWidth());
        iteration++;
      }
    }
  }

}

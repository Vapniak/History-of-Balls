namespace HOB;

using Godot;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class MapData : Resource {
  [Export] public string Title { get; set; }
  [Export] public string Description { get; set; }
  [Export] public int Cols { get; set; }
  [Export] public int Rows { get; set; }
  [Export] public Hex[] HexList { get; set; }
  [Export] public MapSettings Settings { get; set; }

  public void ParseMap(Json json) {
    var data = json.Data.AsGodotDictionary();

    Title = data["title"].AsString();
    Description = data["description"].AsString();
    Cols = data["cols"].AsInt32();
    Rows = data["rows"].AsInt32();

    var hexList = new List<Hex>();

    var createMapSettings = Settings == null;

    if (createMapSettings) {
      Settings = new();
    }

    foreach (var item in data["hexMap"].AsGodotArray()) {
      var hex = item.AsGodotDictionary();

      var hexToAdd = new Hex() {
        Col = hex["q"].AsInt32(),
        Row = hex["r"].AsInt32(),
        Color = Color.FromHtml(hex["color"].AsString())
      };

      hexList.Add(hexToAdd);

      Settings.HexSettings.TryAdd(hexToAdd.Color, new() { MoveCost = 1, Visualizaiton = hexToAdd.Color });
    }

    HexList = hexList.ToArray();
  }
};

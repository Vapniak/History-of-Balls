namespace HOB;

using Godot;
using Godot.Collections;
using HexGridMap;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class MapGridShape : GridShape {
  [Export(PropertyHint.File, "*.json")] private string MapFile { get; set; }

  public Dictionary MapData { get; set; }
  public override T[] CreateCells<T>(Func<CubeCoord, T> createCell, HexLayout layout) {
    var json = ResourceLoader.Load<Json>(MapFile);

    var data = (Dictionary)json.Data;
    MapData = data;

    GD.Print(data["title"]);

    var cells = new List<T>();
    foreach (var item in data["hexMap"].AsGodotArray()) {
      var hex = item.AsGodotDictionary();
      cells.Add(createCell(new CubeCoord(hex["q"].AsInt32(), hex["r"].AsInt32())));
    }

    return cells.ToArray();
  }
  public override Vector2I GetRectSize() {
    return new(40, MapData["rows"].AsInt32());
  }
}

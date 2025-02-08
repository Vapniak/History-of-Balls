namespace HOB;

using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class MapData : Resource {
  [Export] public string Title { get; set; }
  [Export] public string Description { get; set; }
  [Export] public int Cols { get; set; }
  [Export] public int Rows { get; set; }
  [Export] public Cell[] Cells { get; set; }
  [Export] public MapSettings Settings { get; set; }

  public void ParseMap(Json json) {
    var data = json.Data.AsGodotDictionary();

    Title = data["title"].AsString();
    Description = data["description"].AsString();
    Cols = data["cols"].AsInt32();
    Rows = data["rows"].AsInt32();


    Settings ??= new();

    var cells = new Array<CellDefinition>();
    foreach (var item in data["cellDefinitions"].AsGodotArray()) {
      var cell = item.AsGodotDictionary();

      var cellToAdd = new CellDefinition() {
        Name = cell["name"].AsString(),
        Color = Color.FromHtml(cell["color"].AsString()),
        MoveCost = 1,
      };

      cells.Add(cellToAdd);
    }

    Settings.CellDefinitions = cells;


    var cellList = new List<Cell>();
    foreach (var item in data["cells"].AsGodotArray()) {
      var hex = item.AsGodotDictionary();

      var hexToAdd = new Cell() {
        Col = hex["col"].AsInt32(),
        Row = hex["row"].AsInt32(),
        Id = hex["id"].AsInt32(),
        ObjectId = hex["objectId"].AsInt32()
      };

      cellList.Add(hexToAdd);
    }

    Cells = cellList.ToArray();
  }
};

namespace HOB;

using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

[GlobalClass, Tool]
public partial class MapData : Resource {
  [Export] private Json MapFile { get; set; }
  [Export]
  private bool Generate {
    get => false; set => ParseMap();
  }
  [Export] public string Title { get; set; }
  [Export] public string Description { get; set; }
  [Export] public int Cols { get; set; }
  [Export] public int Rows { get; set; }
  [Export] public MapSettings Settings { get; set; }

  private Dictionary _mapData;
  private Array<Cell> Cells { get; set; }

  public void ParseMap() {
    var data = MapFile.Data.AsGodotDictionary();

    Title = data["title"].AsString();
    Description = data["description"].AsString();
    Cols = data["cols"].AsInt32();
    Rows = data["rows"].AsInt32();


    Settings ??= new();

    var cells = new Array<CellSetting>();
    foreach (var item in data["cellDefinitions"].AsGodotArray()) {
      var cell = item.AsGodotDictionary();

      var cellToAdd = new CellSetting() {
        Name = cell["name"].AsString(),
        Color = Color.FromHtml(cell["color"].AsString()),
        MoveCost = 1,
      };

      cells.Add(cellToAdd);
    }

    if (Settings.CellSettings == null) {
      Settings.CellSettings = cells;
    }
  }

  public Array<Cell> GetCells() {
    if (Cells != null) {
      return Cells;
    }

    Cells = new();

    foreach (var item in MapFile.Data.AsGodotDictionary()["cells"].AsGodotArray()) {
      var hex = item.AsGodotDictionary();

      var hexToAdd = new Cell() {
        Col = hex["col"].AsInt32(),
        Row = hex["row"].AsInt32(),
        Id = hex["id"].AsInt32(),
        ObjectId = hex["objectId"].AsInt32()
      };

      Cells.Add(hexToAdd);
    }

    return Cells;
  }

  public Cell GetCell(int col, int row) {
    return Cells.FirstOrDefault(cell => cell.Col == col && cell.Row == row);
  }
};

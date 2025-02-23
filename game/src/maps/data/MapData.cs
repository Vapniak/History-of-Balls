namespace HOB;

using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.IO;

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
  private Array<Cell> Cells { get; set; } = new();

  private void ParseMap() {
    if (MapFile == null || MapFile.Data.VariantType == Variant.Type.Nil) {
      return;
    }

    var data = MapFile.Data.AsGodotDictionary();
    Title = data["title"].AsString();
    Description = data["description"].AsString();
    Cols = data["cols"].AsInt32();
    Rows = data["rows"].AsInt32();
  }

  public Array<Cell> GetCells() {
    if (Cells.Count > 0) {
      return Cells;
    }

    Cells.Clear();
    if (MapFile == null || MapFile.Data.VariantType == Variant.Type.Nil) {
      return Cells;
    }

    var mapData = MapFile.Data.AsGodotDictionary();
    if (!mapData.ContainsKey("cells") || mapData["cells"].VariantType == Variant.Type.Nil) {
      return Cells;
    }

    var cellSettingsDict = new Godot.Collections.Dictionary<int, CellSetting>();
    foreach (var item in mapData["cellDefinitions"].AsGodotArray()) {
      var cellDef = item.AsGodotDictionary();
      var cellSetting = new CellSetting {
        Name = cellDef["name"].AsString(),
        Color = Color.FromHtml(cellDef["color"].AsString()),
        MoveCost = cellDef.ContainsKey("moveCost") ? cellDef["moveCost"].AsInt32() : 1
      };
      cellSettingsDict[cellDef["id"].AsInt32()] = cellSetting;
    }

    foreach (var item in mapData["cells"].AsGodotArray()) {
      var hex = item.AsGodotDictionary();
      var cellId = hex["id"].AsInt32();
      var cellToAdd = new Cell() {
        Col = hex["col"].AsInt32(),
        Row = hex["row"].AsInt32(),
        Id = cellId,
        ObjectId = hex["objectId"].AsInt32()
      };

      if (cellSettingsDict.ContainsKey(cellId)) {
        var cellSetting = cellSettingsDict[cellId];
        if (Settings == null) {
          Settings = new MapSettings { CellSettings = new Godot.Collections.Array<CellSetting>() };
        }
        if (!Settings.CellSettings.Contains(cellSetting)) {
          Settings.CellSettings.Add(cellSetting);
        }
      }

      Cells.Add(cellToAdd);
    }

    return Cells;
  }

  private void SetCells(Array<Cell> newCells) => Cells = newCells;

  private void SaveMapData(string path) {
    if (Cells == null || Cells.Count == 0) {
      GD.PrintErr("Brak danych do zapisania!");
      return;
    }

    var data = new Dictionary {
            { "title", Title },
            { "description", Description },
            { "cols", Cols },
            { "rows", Rows },
            { "cells", new Array() }
        };

    foreach (var cell in Cells) {
      var cellData = new Dictionary {
                { "col", cell.Col },
                { "row", cell.Row },
                { "id", cell.Id },
                { "objectId", cell.ObjectId }
            };
      ((Array)data["cells"]).Add(cellData);
    }

    var jsonString = Json.Stringify(data, "\t");

    File.WriteAllText(path, jsonString);
    GD.Print("Mapa zapisana do pliku: " + path);
  }
}

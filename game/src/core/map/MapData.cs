namespace HOB;

using Godot;
using Godot.Collections;
using System.Linq;
using System.IO;
using System.Collections.Generic;

[GlobalClass, Tool]
public partial class MapData : Resource {
  [Export] private Json? MapFile { get; set; }
  [Export]
  private bool Generate {
    get => false;
    set => ParseMap();
  }
  [Export] public string Title { get; set; } = "";
  [Export] public int Cols { get; set; }
  [Export] public int Rows { get; set; }

  [Export] public MapSettings? Settings { get; private set; }
  [Export] public NationsSettings? NationsSettings { get; private set; }

  private Dictionary? _mapData;
  private Array<Cell>? Cells { get; set; }

  private void ParseMap() {
    if (MapFile == null || MapFile.Data.VariantType == Variant.Type.Nil) {
      return;
    }

    var data = MapFile.Data.AsGodotDictionary();
    Title = data.ContainsKey("title") ? data["title"].AsString() : "";
    //Description = data.ContainsKey("description") ? data["description"].AsString() : "";
    Cols = data.ContainsKey("cols") ? data["cols"].AsInt32() : 0;
    Rows = data.ContainsKey("rows") ? data["rows"].AsInt32() : 0;

    Settings ??= new MapSettings();
    NationsSettings ??= new();

    if (data.ContainsKey("cellDefinitions") && data["cellDefinitions"].VariantType != Variant.Type.Nil) {
      var cellSettings = new Array<CellSetting>();
      foreach (var item in data["cellDefinitions"].AsGodotArray()) {
        var cellDef = item.AsGodotDictionary();
        if (!cellDef.ContainsKey("id") || !cellDef.ContainsKey("name") || !cellDef.ContainsKey("color")) {
          continue;
        }

        var cellSetting = new CellSetting {
          Name = cellDef["name"].AsString(),
          Color = Color.FromHtml(cellDef["color"].AsString()),
          MoveCost = cellDef.ContainsKey("moveCost") ? cellDef["moveCost"].AsInt32() : 1
        };
        cellSettings.Add(cellSetting);
      }
      if (Settings.CellSettings == null) {
        Settings.CellSettings = cellSettings;
      }
    }

    if (data.ContainsKey("nationDefinitions")) {
      var nationSettings = new NationsSettings();

      foreach (var item in data["nationDefinitions"].AsGodotArray()) {
        var nationDef = item.AsGodotDictionary();
        var nation = new NationSettings(nationDef["name"].ToString());
        if (data.ContainsKey("objectDefinitions")) {
          foreach (var objectDef in data["objectDefinitions"].AsGodotArray()) {
            var @object = objectDef.AsGodotDictionary();
            var obj = new ObjectSetting() {
              Name = @object["name"].ToString(),
            };

            nation.ObjectSettings.Add(obj);
          }
        }

        nationSettings.Settings.Add(nation);
      }

      NationsSettings = nationSettings;
    }
  }

  public Array<Cell> GetCells() {
    if (Cells != null) {
      return Cells;
    }
    Cells = new Array<Cell>();

    if (MapFile == null || MapFile.Data.VariantType == Variant.Type.Nil) {
      return Cells;
    }

    var data = MapFile.Data.AsGodotDictionary();
    if (!data.ContainsKey("cells") || data["cells"].VariantType == Variant.Type.Nil) {
      return Cells;
    }

    var cellSettingsDict = new Godot.Collections.Dictionary<int, CellSetting>();
    if (data.ContainsKey("cellDefinitions") && data["cellDefinitions"].VariantType != Variant.Type.Nil) {
      foreach (var item in data["cellDefinitions"].AsGodotArray()) {
        var cellDef = item.AsGodotDictionary();
        if (!cellDef.ContainsKey("id") || !cellDef.ContainsKey("name") || !cellDef.ContainsKey("color")) {
          continue;
        }

        var id = cellDef["id"].AsInt32();
        var name = cellDef["name"].AsString();

        CellSetting setting = null;
        if (Settings != null && Settings.CellSettings != null) {
          setting = Settings.CellSettings.FirstOrDefault(cs => cs.Name == name);
        }

        if (setting == null) {
          setting = new CellSetting {
            Name = name,
            Color = Color.FromHtml(cellDef["color"].AsString()),
            MoveCost = cellDef.ContainsKey("moveCost") ? cellDef["moveCost"].AsInt32() : 1
          };
          if (Settings == null) {
            Settings = new MapSettings { CellSettings = new Array<CellSetting>() };
          }

          Settings.CellSettings.Add(setting);
        }
        cellSettingsDict[id] = setting;
      }
    }

    foreach (var item in data["cells"].AsGodotArray()) {
      var hex = item.AsGodotDictionary();

      if (!hex.ContainsKey("id") || !hex.ContainsKey("col") || !hex.ContainsKey("row")) {
        GD.PrintErr("Brak klucza 'id', 'col' lub 'row' w danych komórki");
        continue;
      }

      var cellId = hex["id"].AsInt32();
      var col = hex["col"].AsInt32();
      var row = hex["row"].AsInt32();
      var objectId = hex.ContainsKey("objectId") ? hex["objectId"].AsInt32() : 0;
      var nationId = hex["nationId"].AsInt32();

      var cell = new Cell() {
        Col = col,
        Row = row,
        Id = cellId,
        ObjectId = objectId,
        NationId = nationId,
      };

      if (cellSettingsDict.ContainsKey(cellId)) {
        var cellSetting = cellSettingsDict[cellId];
        if (Settings == null) {
          Settings = new MapSettings { CellSettings = new Array<CellSetting>() };
        }

        if (!Settings.CellSettings.Contains(cellSetting)) {
          Settings.CellSettings.Add(cellSetting);
        }
      }

      Cells.Add(cell);
    }
    return Cells;
  }

  private void SaveMapData(string path) {
    if (Cells == null || Cells.Count == 0) {
      GD.PrintErr("Brak danych do zapisania!");
      return;
    }

    var data = new Dictionary {
            { "title", Title },
            //{ "description", Description },
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

  public Cell GetCell(int col, int row) {
    return Cells.FirstOrDefault(cell => cell.Col == col && cell.Row == row);
  }
}

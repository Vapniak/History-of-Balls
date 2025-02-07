namespace HOB;

using Godot;
using System;


// TODO: make map generator plugin for in editor generation
[Tool]
public partial class MapGenerator : Node {
  [Export(PropertyHint.File, "*.json")] private string MapPath { get; set; }
  [Export] private MapData GeneratedMapData { get; set; }
  [Export]
  private bool GenerateMap {
    get => false;
    set {
      if (MapPath != null) {
        var json = ResourceLoader.Load<Json>(MapPath);
        if (json != null) {
          GeneratedMapData ??= new();
          GeneratedMapData.ParseMap(json);
        }
      }
    }
  }
}

namespace HexGridMap;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class HexGrid : Node3D {
  [Export] public HexLayout Layout { get; private set; } = new();
  [Export] public MapShape MapShape { get; private set; }
  private readonly HashSet<HexCell> _map = new();

  public HexGrid() {

  }
  public void BuildMap() {
    _map.Clear();
    MapShape.BuildMap(this);
  }

  public void AddHex(HexCell hex) {
    _map.Add(hex);
  }

  public int Size() {
    return _map.Count;
  }

  public void RemoveHex(HexCell hex) {
    _map.Remove(hex);
  }

  public HashSet<HexCell>.Enumerator GetMapEnumerator() {
    return _map.GetEnumerator();
  }
}

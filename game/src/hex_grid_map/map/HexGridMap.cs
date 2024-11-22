namespace HexGridMap;

using System.Collections.Generic;
using Godot;

// https://www.redblobgames.com/grids/hexagons/implementation.html

[GlobalClass]
public partial class HexGridMap : Resource {
  [Export] public Layout Layout { get; private set; }
  [Export] private MapShape _mapShape;
  private readonly HashSet<Hex> _map = new();

  public HexGridMap() {

  }
  public void BuildMap() {
    _map.Clear();
    _mapShape.BuildMap(this);
  }

  public void AddHex(Hex hex) {
    _map.Add(hex);
  }

  public int Size() {
    return _map.Count;
  }

  public void RemoveHex(Hex hex) {
    _map.Remove(hex);
  }

  public HashSet<Hex>.Enumerator GetMapEnumerator() {
    return _map.GetEnumerator();
  }
}

namespace HexGridMap;

using Godot;
using System;

public partial class HexGrid : Node {

  [Export] private int _seed;

  public int CellCountX { get; private set; }
  public int CellCountZ { get; private set; }
}

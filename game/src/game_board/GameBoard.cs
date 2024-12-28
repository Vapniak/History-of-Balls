namespace HOB;

using Godot;
using HexGridMap;
using System;

public partial class GameBoard : Node3D {

  [Export] public HexGrid Grid { get; private set; }

  [Export] private PackedScene _debugMesh;

  public Vector3[] CellPositions { get; private set; }
  public override void _Ready() {
    Grid.CreateGrid();

    var cells = Grid.GetCells();
    var count = cells.Length;
    CellPositions = new Vector3[count];

    for (var i = 0; i < count; i++) {
      var cell = cells[i];
      var point = Grid.Layout.HexCoordinatesToPoint(cell.Coordinates);
      var mesh = _debugMesh.Instantiate<Node3D>();

      CellPositions[i] = new(point.X, 0, point.Y);

      mesh.Position = new(point.X, 0, point.Y);
      mesh.Scale = Vector3.One * Grid.Layout.HexCellScale;
      AddChild(mesh);
    }
  }
}

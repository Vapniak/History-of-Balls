namespace HOB;

using System.Collections.Generic;
using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;

/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();
  [Export] private HexGrid Grid { get; set; }
  [Export] private PackedScene _debugMesh;

  public EntityManager EntityManager { get; private set; }

  private Aabb _combinedAabb;
  public override void _Ready() {
    EntityManager = new() {
      GameBoard = this
    };
    AddChild(EntityManager);

    _combinedAabb = new();

    // TODO: procedural map generation and divide it into chunks and optimalize
    // TODO: chunk loading of nearest visible nodes
    // TODO: option to load map from external file

    // TODO: work on shader for hex cell
    foreach (var coord in Grid.CreateGridShape()) {
      var point = Grid.GetLayout().HexToPoint(coord);
      var mesh = _debugMesh.Instantiate<MeshInstance3D>();

      mesh.Position = new(point.X, 0, point.Y);
      AddChild(mesh);

      _combinedAabb = _combinedAabb.Merge(mesh.GlobalTransform * mesh.GetAabb());
    }

    EmitSignal(SignalName.GridCreated);
  }
  public Aabb GetAabb() => _combinedAabb;

  public HexCoordinates GetHexCoordinates(Vector3 point) {
    return Grid.GetLayout().PointToHex(new(point.X, point.Z));
  }

  public Vector3 GetPoint(HexCoordinates coordinates) {
    var point = Grid.GetLayout().HexToPoint(coordinates);
    return new(point.X, GetAabb().GetCenter().Y + (GetAabb().Size.Y / 2), point.Y);
  }
}

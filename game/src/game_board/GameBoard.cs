namespace HOB;

using System.Collections.Generic;
using System.Linq;
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


  private Dictionary<PlayerState, List<Entity>> PlayerEntities { get; set; }

  private Aabb _combinedAabb;
  public override void _Ready() {
    PlayerEntities = new();

    _combinedAabb = new();

    // TODO: procedural map generation and divide it into chunks and optimalize
    // TODO: chunk loading of nearest visible nodes
    // TODO: option to load map from external file
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

  public void AddEntity(PlayerState owner, Entity entity, HexCoordinates coords) {
    entity.Ready += () => {
      entity.GlobalPosition = GetPoint(coords);
    };

    entity.Coordinates = coords;
    entity.OwnerState = owner;

    AddChild(entity);

    if (!PlayerEntities.TryGetValue(owner, out var entities)) {
      entities = new();
      PlayerEntities.Add(owner, entities);
    }

    entities.Add(entity);
  }

  public List<Entity> GetEntitiesOnCoords(PlayerState playerState, HexCoordinates coordinates) {
    var entities = GetEntitiesByOwner(playerState).Where(e => e.Coordinates == coordinates).ToList();
    return entities;
  }

  public List<Entity> GetEntitiesByOwner(PlayerState ownerState) {
    PlayerEntities.TryGetValue(ownerState, out var value);
    return value;
  }
}

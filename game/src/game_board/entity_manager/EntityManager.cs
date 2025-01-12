namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;
using System.Collections.Generic;
using System.Linq;

public partial class EntityManager : Node {
  public GameBoard GameBoard { get; set; }

  private List<Entity> Entities { get; set; }
  public override void _Ready() {
    Entities = new();
  }

  public void AddEntity(Entity entity, HexCoordinates coords, IMatchController controller = null) {
    entity.Ready += () => {
      entity.GlobalPosition = GameBoard.GetPoint(coords);
    };

    entity.Coordinates = coords;

    AddChild(entity);

    controller?.OwnedEntities.Add(entity);
    Entities.Add(entity);
  }

  public List<Entity> GetOwnedEntitiesOnCoords(IMatchController owner, HexCoordinates coords) {
    var entities = owner.OwnedEntities.Where(e => e.Coordinates == coords).ToList();
    return entities;
  }

  public List<Entity> GetEntitiesOnCoords(HexCoordinates coords) {
    return Entities.Where(e => e.Coordinates == coords).ToList();
  }
}

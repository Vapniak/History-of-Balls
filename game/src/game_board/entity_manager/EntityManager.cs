namespace HOB;

using Godot;
using HexGridMap;
using HOB.GameEntity;
using System.Collections.Generic;
using System.Linq;

public partial class EntityManager : Node {
  private List<Entity> Entities { get; set; }
  public override void _Ready() {
    Entities = new();
  }

  public void AddEntity(Entity entity, CubeCoord coord, Vector3 position, IMatchController controller = null) {
    entity.Ready += () => {
      entity.GlobalPosition = position;
    };

    entity.coords = coord;

    AddChild(entity);

    controller?.OwnedEntities.Add(entity);
    Entities.Add(entity);
  }

  public Entity[] GetOwnedEntitiesOnCoords(IMatchController owner, CubeCoord coord) {
    var entities = owner.OwnedEntities.Where(e => e.coords == coord).ToList();
    return entities.ToArray();
  }

  public Entity[] GetEntitiesOnCoords(CubeCoord coord) {
    return Entities.Where(e => e.coords == coord).ToArray();
  }
}

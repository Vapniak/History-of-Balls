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

  public void AddEntity(Entity entity, HexCell cell, Vector3 position, IMatchController controller = null) {
    entity.Ready += () => {
      entity.GlobalPosition = position;
    };

    entity.Cell = cell;

    AddChild(entity);

    controller?.OwnedEntities.Add(entity);
    Entities.Add(entity);
  }

  public Entity[] GetOwnedEntitiesOnCell(IMatchController owner, HexCell cell) {
    var entities = owner.OwnedEntities.Where(e => e.Cell == cell).ToList();
    return entities.ToArray();
  }

  public Entity[] GetEntitiesOnCell(HexCell cell) {
    return Entities.Where(e => e.Cell == cell).ToArray();
  }
}

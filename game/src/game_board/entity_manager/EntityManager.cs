namespace HOB;
using Godot;
using HOB.GameEntity;
using System.Collections.Generic;
using System.Linq;

public partial class EntityManager : Node {
  private List<Entity> Entities { get; set; }
  private Dictionary<IMatchController, List<Entity>> OwnedEntities { get; set; }

  public override void _Ready() {
    Entities = new();
    OwnedEntities = new();
  }

  public void AddEntity(Entity entity, GameCell cell, Vector3 position, IMatchController controller) {
    entity.Ready += () => {
      entity.GlobalPosition = position;
    };

    entity.Cell = cell;
    entity.OwnerController = controller;

    AddChild(entity);


    if (OwnedEntities.TryGetValue(controller, out var entites)) {
      entites.Add(entity);
    }
    else {
      OwnedEntities.Add(controller, new() { entity });
    }

    Entities.Add(entity);
  }

  public void RemoveEntity(Entity entity) {
    entity.QueueFree();
    Entities.Remove(entity);
    Entities.Remove(entity);
  }

  public Entity[] GetOwnedEntitiesOnCell(IMatchController owner, GameCell cell) {
    return GetOwnedEntites(owner).Where(e => e.Cell == cell).ToArray();
  }

  public Entity[] GetOwnedEntites(IMatchController owner) {
    return OwnedEntities.GetValueOrDefault(owner).ToArray();
  }

  public Entity[] GetEntitiesOnCell(GameCell cell) {
    return Entities.Where(e => e.Cell == cell).ToArray();
  }
}

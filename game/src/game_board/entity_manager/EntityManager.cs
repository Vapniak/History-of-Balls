namespace HOB;
using Godot;
using HOB.GameEntity;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class EntityManager : Node {
  [Signal] public delegate void EntityRemovedEventHandler(Entity entity);

  private List<Entity> Entities { get; set; }
  private Dictionary<IMatchController, List<Entity>> OwnedEntities { get; set; }

  public override void _Ready() {
    Entities = new();
    OwnedEntities = new();
  }

  public void AddEntity(Entity entity) {
    entity.TreeExiting += () => RemoveEntity(entity);

    AddChild(entity);

    if (entity.TryGetOwner(out var owner)) {
      if (OwnedEntities.TryGetValue(owner, out var entites)) {
        entites.Add(entity);
      }
      else {
        OwnedEntities.Add(owner, new() { entity });
      }
    }

    Entities.Add(entity);
  }

  public void RemoveEntity(Entity entity) {
    entity.QueueFree();
    Entities.Remove(entity);

    if (entity.TryGetOwner(out var owner)) {
      if (OwnedEntities.TryGetValue(owner, out var entites)) {
        entites.Remove(entity);
      }
    }

    EmitSignal(SignalName.EntityRemoved, entity);
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

  public Entity[] GetEnemyEntities(IMatchController controller) {
    return Entities.Except(OwnedEntities[controller]).ToArray();
  }

  public Entity[] GetEntities() => Entities.ToArray();
}

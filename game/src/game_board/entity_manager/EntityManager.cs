namespace HOB;
using Godot;
using HOB.GameEntity;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class EntityManager : Node {
  [Export] private Material EntityOwnedMaterial { get; set; }
  [Export] private Material EntityEnemyMaterial { get; set; }
  [Export] private Material EntityNotOwnedMaterial { get; set; }


  public GameGrid Grid { get; set; }

  private List<Entity> Entities { get; set; }


  // TODO: make specific ui for each entity type, structure, unit have different UI in 3D
  private string _entityUISceneUID = "uid://ka4lyslghbk";

  public override void _Ready() {
    Entities = new();
  }


  public void AddEntity(Entity entity) {
    entity.TreeExiting += () => RemoveEntity(entity);

    entity.OwnerControllerChanged += () => {
      if (entity.TryGetOwner(out var owner)) {
        SetEntityMaterialBasedOnOwnership(entity.GetOwnershipType(owner), entity);
      }
    };

    if (entity.TryGetOwner(out var owner)) {
      SetEntityMaterialBasedOnOwnership(entity.GetOwnershipType(owner), entity);
    }

    AddChild(entity);

    var entityUIScene = ResourceLoader.Load<PackedScene>(_entityUISceneUID).Instantiate<EntityUi3D>();
    entityUIScene.SetNameLabel(entity.GetEntityName());
    entity.Body.AddChild(entityUIScene);

    Entities.Add(entity);
  }

  public void RemoveEntity(Entity entity) {
    entity.QueueFree();
    Entities.Remove(entity);
  }

  public void SetEntityMaterialBasedOnOwnership(Entity.OwnershipType ownership, Entity entity) {
    switch (ownership) {
      case Entity.OwnershipType.Owned:
        entity.SetMaterial(EntityOwnedMaterial);
        break;
      case Entity.OwnershipType.Enemy:
        entity.SetMaterial(EntityEnemyMaterial);
        break;
      case Entity.OwnershipType.NotOwned:
        entity.SetMaterial(EntityNotOwnedMaterial);
        break;
      default:
        break;
    }
  }
  public Entity[] GetOwnedEntitiesOnCell(IMatchController owner, GameCell cell) {
    return GetOwnedEntites(owner).Where(e => e.Cell == cell).ToArray();
  }

  public Entity[] GetOwnedEntites(IMatchController owner) {
    return Entities.Where(e => e.GetOwnershipType(owner) == Entity.OwnershipType.Owned).ToArray();
  }


  public Entity[] GetNotOwnedEntities() {
    return Entities.Where(e => !e.TryGetOwner(out _)).ToArray();
  }

  public Entity[] GetEntitiesOnCell(GameCell cell) {
    return Entities.Where(e => e.Cell == cell).ToArray();
  }

  public Entity[] GetEnemyEntities(IMatchController controller) {
    return Entities.Where(e => e.GetOwnershipType(controller) == Entity.OwnershipType.Enemy).ToArray();
  }

  public Entity[] GetEntities() => Entities.ToArray();
}

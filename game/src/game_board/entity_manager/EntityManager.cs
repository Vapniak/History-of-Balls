namespace HOB;
using Godot;
using HOB.GameEntity;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class EntityManager : Node {
  [Export] private Material EntityMaterial { get; set; }

  public GameGrid Grid { get; set; }

  private List<Entity> Entities { get; set; }

  private Dictionary<Team, Material> TeamMaterials { get; set; }


  private string _entityUISceneUID = "uid://ka4lyslghbk";

  public override void _Ready() {
    Entities = new();
    TeamMaterials = new();
  }


  public void AddEntity(Entity entity) {
    entity.TreeExiting += () => RemoveEntity(entity);

    var entityUIScene = ResourceLoader.Load<PackedScene>(_entityUISceneUID).Instantiate<EntityUi3D>();

    entity.OwnerControllerChanged += () => {
      if (entity.TryGetOwner(out var owner)) {
        if (!TeamMaterials.TryGetValue(owner.Team, out var value)) {
          value = EntityMaterial.Duplicate() as Material;
          value.Set("shader_parameter/emission_color", owner.Team.Color);
        }

        entityUIScene.SetColor(owner.Team.Color);
        entity.SetMaterial(value);
      }
    };

    AddChild(entity);

    entity.Body.AddChild(entityUIScene);

    Entities.Add(entity);
  }

  public void RemoveEntity(Entity entity) {
    entity.QueueFree();
    Entities.Remove(entity);

    // TODO: remove team materials
  }

  public Entity[] GetOwnedEntitiesOnCell(IMatchController owner, GameCell cell) {
    return GetOwnedEntites(owner).Where(e => e.Cell == cell).ToArray();
  }

  public Entity[] GetOwnedEntites(IMatchController owner) {
    return Entities.Where(e => e.TryGetOwner(out var o) && o == owner).ToArray();
  }


  public Entity[] GetNotOwnedEntities() {
    return Entities.Where(e => !e.TryGetOwner(out _)).ToArray();
  }

  public Entity[] GetEntitiesOnCell(GameCell cell) {
    return Entities.Where(e => e.Cell == cell).ToArray();
  }

  public Entity[] GetEnemyEntities(IMatchController controller) {
    return Entities.Where(e => e.TryGetOwner(out var owner) && owner != controller).ToArray();
  }

  public Entity[] GetEntities() => Entities.ToArray();
}

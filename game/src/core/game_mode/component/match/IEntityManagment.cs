namespace HOB;

using System;
using Godot;
using HexGridMap;
using HOB.GameEntity;

public interface IEntityManagment {
  public event Action<Entity> EntityAdded;
  public event Action<Entity> EntityRemoved;

  public bool TryAddEntityOnCell(EntityData data, GameCell cell, IMatchController owner, Vector3 rotation = new());
  public Entity[] GetOwnedEntitiesOnCell(IMatchController owner, GameCell cell);
  public Entity[] GetOwnedEntites(IMatchController owner);
  public Entity[] GetNotOwnedEntities();
  public Entity[] GetEntitiesOnCell(GameCell cell);
  public Entity[] GetEnemyEntities(IMatchController controller);
  public Entity[] GetEntities();
}

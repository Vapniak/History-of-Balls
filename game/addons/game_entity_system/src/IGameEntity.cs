namespace GameEntitySystem;

using Godot.Collections;

public interface IGameEntity {
  public IGameEntityData GameEntityData { get; }
  public Array<GameEntityComponent> EntityComponents { get; }

  public virtual void SetupComponents() {
    foreach (var component in EntityComponents) {
      component.SetData(GameEntityData);
    }
  }
}

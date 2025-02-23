namespace HOB.GameEntity;

using Godot;


[GlobalClass]
public abstract partial class Trait : Node {
  public Entity Entity { get; set; }

  public override void _Ready() {
    Entity.OwnerControllerChanging += OnOwnerChanging;
    Entity.OwnerControllerChanged += OnOwnerChanged;
  }

  protected virtual void OnOwnerChanging() {

  }

  protected virtual void OnOwnerChanged() {

  }
  protected T GetStat<T>() where T : BaseStat {
    return Entity.GetStat<T>();
  }
}

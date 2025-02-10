namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public abstract partial class Trait : Node {
  public Entity Entity { get; set; }

  protected T GetStat<T>() where T : BaseStat {
    return Entity.GetStat<T>();
  }
}

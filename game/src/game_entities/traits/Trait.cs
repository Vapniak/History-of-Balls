namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public abstract partial class Trait : Node {
  public Entity Entity { get; set; }
}

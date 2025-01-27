namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public abstract partial class Command : Resource {
  [Export] public string Name { get; private set; } = "Command";

  public abstract bool IsAvailable();
}


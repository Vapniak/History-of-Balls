namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public abstract partial class Trait : Node {
  public Entity GetEntity() {
    var entity = GetOwnerOrNull<Entity>();
    if (entity == null) {
      GD.Print("Trait is not owned by entity.");
    }

    return entity;
  }
}

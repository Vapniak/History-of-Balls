namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ClaimableTrait : Trait {
  public void ClaimBy(IMatchController controller) {
    Entity.ChangeOwner(controller);
  }
}

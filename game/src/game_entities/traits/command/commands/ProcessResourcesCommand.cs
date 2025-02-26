namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ProcessResourcesCommand : Command {
  [Export] public FactoryTrait FactoryTrait { get; private set; }

  public override bool IsAvailable() {
    if (FactoryTrait.ProcessingRoundsLeft > 0) {
      return false;
    }

    if (GetEntity().TryGetOwner(out var owner)) {
      if (GetEntity().TryGetStat<FactoryStats>(out var stats)) {
        if (owner.GetPlayerState().GetResourceType(stats.InputType).Value >= stats.InputValue) {
          return base.IsAvailable();
        }
      }
    }

    return false;
  }

  public override void OnTurnStarted() {
    base.OnTurnStarted();

    if (!IsOwnerCurrentTurn()) {
      return;
    }

    if (IsAvailable()) {
      Use();
      FactoryTrait.StartProcessing();
      Finish();
    }
  }
}

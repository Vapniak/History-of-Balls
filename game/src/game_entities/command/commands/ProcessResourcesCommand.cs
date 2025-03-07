namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ProcessResourcesCommand : Command {
  [Export] public FactoryTrait FactoryTrait { get; private set; }

  public override bool CanBeUsed(IMatchController caller) {
    if (FactoryTrait.ProcessingRoundsLeft > 0) {
      return false;
    }

    if (GetEntity().TryGetOwner(out var owner)) {
      if (GetEntity().TryGetStat<FactoryStats>(out var stats)) {
        if (owner.GetPlayerState().GetResourceType(stats.ProcessedResource).Value >= stats.ProcessedValue) {
          return base.CanBeUsed(caller);
        }
      }
    }

    return false;
  }

  public override void OnTurnStarted() {
    base.OnTurnStarted();

    if (GetEntity().TryGetOwner(out var owner)) {
      if (CanBeUsed(owner)) {
        Use();
        FactoryTrait.StartProcessing();
        Finish();
      }
    }
  }
}

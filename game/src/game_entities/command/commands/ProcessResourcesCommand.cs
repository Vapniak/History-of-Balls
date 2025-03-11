namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ProcessResourcesCommand : Command {
  [Export] public FactoryTrait FactoryTrait { get; private set; }

  public override void _Ready() {
    base._Ready();

    GetEntity().OwnerControllerChanging += () => {
      if (GetEntity().TryGetOwner(out var owner)) {
        var stats = GetEntity().GetStat<FactoryStats>();
        owner.GetPlayerState().GetResourceType(stats.ProcessedResource).ValueChanged -= TryUse;
      }
    };

    GetEntity().OwnerControllerChanged += () => {
      if (GetEntity().TryGetOwner(out var owner)) {
        var stats = GetEntity().GetStat<FactoryStats>();
        owner.GetPlayerState().GetResourceType(stats.ProcessedResource).ValueChanged += TryUse;
      }
    };
  }
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


  private void TryUse() {
    if (GetEntity().TryGetOwner(out var owner)) {
      if (CanBeUsed(owner)) {
        Use();
        FactoryTrait.StartProcessing();
        Finish();
      }
    }
  }
}

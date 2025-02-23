namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class FactoryTrait : Trait {
  public uint ProcessingTurnsLeft { get; private set; }

  public bool IsProcessingResource() => ProcessingTurnsLeft > 0;
  public void StartProcessing() {
    if (ProcessingTurnsLeft > 0) {
      return;
    }

    if (Entity.TryGetOwner(out var owner)) {
      if (Entity.TryGetStat<FactoryStats>(out var stats)) {
        ProcessingTurnsLeft = stats.ProcessingTurns;
        owner.GetPlayerState().GetResourceType(stats.InputType).Value -= stats.InputValue;
      }
    }
  }

  protected override void OnOwnerChanging() {
    base.OnOwnerChanging();

    if (Entity.TryGetOwner(out var owner)) {
      owner.GetGameState().TurnStartedEvent -= OnTurnStarted;
    }
  }

  protected override void OnOwnerChanged() {
    base.OnOwnerChanged();

    if (Entity.TryGetOwner(out var owner)) {
      owner.GetGameState().TurnStartedEvent += OnTurnStarted;
    }
  }

  private void OnTurnStarted() {
    if (IsProcessingResource()) {
      ProcessingTurnsLeft--;

      if (ProcessingTurnsLeft == 0) {
        if (Entity.TryGetOwner(out var owner)) {
          if (Entity.TryGetStat<FactoryStats>(out var stats)) {
            owner.GetPlayerState().GetResourceType(stats.OutputType).Value += stats.OutputValue;
          }
        }
      }
    }
  }
}

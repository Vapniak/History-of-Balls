namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class FactoryTrait : Trait {
  [Signal] public delegate void ProcessingFinishedEventHandler();
  public uint ProcessingRoundsLeft { get; private set; }

  private bool _startedProcessingThisTurn;

  public void StartProcessing() {
    if (Entity.TryGetOwner(out var owner)) {
      if (Entity.TryGetStat<FactoryStats>(out var stats)) {
        ProcessingRoundsLeft = stats.ProcessingTurns;
        owner.GetPlayerState().GetResourceType(stats.ProcessedResource).Value -= stats.ProcessedValue;
      }
    }
  }

  protected override void OnOwnerChanging() {
    base.OnOwnerChanging();

    if (Entity.TryGetOwner(out var owner)) {
      owner.GetGameMode().GetMatchEvents().TurnStarted -= OnTurnStarted;
    }
  }

  protected override void OnOwnerChanged() {
    base.OnOwnerChanged();

    if (Entity.TryGetOwner(out var owner)) {
      owner.GetGameMode().GetMatchEvents().TurnStarted += OnTurnStarted;
    }
  }

  private void OnTurnStarted() {
    if (Entity.TryGetOwner(out var owner) && owner.IsCurrentTurn()) {
      if (ProcessingRoundsLeft > 0) {
        ProcessingRoundsLeft--;
        if (ProcessingRoundsLeft == 0) {
          if (Entity.TryGetStat<FactoryStats>(out var stats)) {
            owner.GetPlayerState().GetResourceType(stats.ProducedResource).Value += stats.ProducedValue;
            EmitSignal(SignalName.ProcessingFinished);
          }
        }
      }
    }
  }
}

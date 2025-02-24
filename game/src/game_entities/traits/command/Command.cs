namespace HOB.GameEntity;

using GameplayFramework;
using Godot;

[GlobalClass]
public abstract partial class Command : Node {
  [Signal] public delegate void StartedEventHandler();
  [Signal] public delegate void FinishedEventHandler();

  [Export] public string CommandName { get; private set; } = "Command";
  [Export] public bool ShowInUI { get; private set; } = true;
  [Export] private uint CooldownRounds { get; set; } = 1;
  public uint CooldownRoundsLeft { get; private set; }
  public bool UsedThisRound { get; private set; }

  public CommandTrait CommandTrait { get; set; }

  private uint _cooldown;

  protected virtual void Use() {
    UsedThisRound = true;
    CooldownRoundsLeft = CooldownRounds;
    EmitSignal(SignalName.Started);
  }
  protected virtual void Finish() {
    EmitSignal(SignalName.Finished);
  }

  public virtual bool IsAvailable() {
    return CooldownRoundsLeft == 0 && !UsedThisRound;
  }

  public virtual void OnTurnStarted() {

  }
  public void OnTurnChanged() {
    if (GetEntity().TryGetOwner(out var owner)) {
      if (!owner.IsCurrentTurn()) {
        return;
      }
    }
    else {
      return;
    }

    UsedThisRound = false;

    if (CooldownRoundsLeft > 0) {
      CooldownRoundsLeft--;
    }
  }

  public virtual void OnTurnEnded() {

  }

  public Entity GetEntity() => CommandTrait.Entity;
}


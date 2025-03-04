namespace HOB.GameEntity;

using GameplayFramework;
using Godot;


// TODO: transform it into resource
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

  public virtual bool CanBeUsed(IMatchController caller) {
    return CooldownRoundsLeft == 0 && !UsedThisRound && GetEntity().TryGetOwner(out var owner) && owner.IsCurrentTurn() && owner == caller;
  }

  public virtual void OnTurnStarted() {

  }
  public void OnTurnChanged() {
    if (!IsOwnerCurrentTurn()) {
      return;
    }

    if (CooldownRoundsLeft > 0) {
      CooldownRoundsLeft--;
      if (CooldownRoundsLeft == 0) {
        UsedThisRound = false;
      }
    }
  }

  public virtual void OnTurnEnded() {

  }

  public virtual void OnRoundStarted() {

  }

  public Entity GetEntity() => CommandTrait.Entity;

  public bool IsOwnerCurrentTurn() {
    if (GetEntity().TryGetOwner(out var owner) && owner.IsCurrentTurn()) {
      return true;
    }

    return false;
  }
}


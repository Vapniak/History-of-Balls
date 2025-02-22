namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public abstract partial class Command : Node {
  [Signal] public delegate void StartedEventHandler();
  [Signal] public delegate void FinishedEventHandler();

  [Export] public string CommandName { get; private set; } = "Command";
  [Export] public bool UseOnRoundStart { get; private set; } = false;
  [Export] public bool ShowInUI { get; private set; } = true;
  [Export] private uint CooldownRounds { get; set; } = 1;
  public uint CooldownRoundsLeft { get; private set; }
  public bool UsedThisRound { get; private set; }

  public CommandTrait CommandTrait { get; set; }

  private uint _cooldown;

  public override void _Ready() {
    if (GetEntity().TryGetOwner(out var owner)) {
      owner.GetGameState().TurnChangedEvent += OnTurnChanged;
    }
  }

  protected virtual void Use() {
    UsedThisRound = true;
    CooldownRoundsLeft = CooldownRounds;
    EmitSignal(SignalName.Started);
  }
  protected virtual void Finish() {
    EmitSignal(SignalName.Finished);
  }

  public virtual bool IsAvailable() {
    return CooldownRoundsLeft == 0 && !UsedThisRound && GetEntity().TryGetOwner(out var owner) && owner.IsCurrentTurn();
  }

  public void OnTurnChanged(int roundNumber) {
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

    if (IsAvailable() && UseOnRoundStart) {
      Use();
      Finish();
    }
  }

  public Entity GetEntity() => CommandTrait.Entity;
}


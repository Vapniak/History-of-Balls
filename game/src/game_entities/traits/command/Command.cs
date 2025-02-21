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

  protected bool IsExecuting { get; private set; }

  private uint _cooldown;

  public override void _Ready() {
    if (GetEntity().OwnerController != null) {
      GetEntity().OwnerController.GetGameState().RoundStartedEvent += OnRoundStarted;
    }
  }

  protected virtual void Use() {
    IsExecuting = true;
    UsedThisRound = true;
    CooldownRoundsLeft = CooldownRounds;
    EmitSignal(SignalName.Started);
  }
  protected virtual void Finish() {
    IsExecuting = false;
    EmitSignal(SignalName.Finished);
  }

  public virtual bool IsAvailable() {
    return CooldownRoundsLeft == 0 && !UsedThisRound && GetEntity().OwnerController.IsCurrentTurn();
  }
  public void OnRoundStarted(int roundNumber) {
    if (!GetEntity().OwnerController.IsCurrentTurn()) {
      return;
    }

    UsedThisRound = false;
    if (CooldownRoundsLeft > 0) {
      CooldownRoundsLeft--;
    }

    if (UseOnRoundStart && IsAvailable()) {
      Use();
      Finish();
    }
  }

  public Entity GetEntity() => CommandTrait.Entity;
}


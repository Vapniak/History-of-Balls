namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public abstract partial class Command : Node {
  [Signal] public delegate void StartedEventHandler();
  [Signal] public delegate void FinishedEventHandler();

  [Export] public string CommandName { get; private set; } = "Command";
  [Export] private uint CooldownRounds { get; set; } = 1;
  public uint CooldownRoundsLeft { get; private set; }
  public bool UsedThisRound { get; private set; }

  public CommandTrait CommandTrait { get; set; }

  protected bool IsExecuting { get; private set; }

  private uint _cooldown;

  public override void _Ready() {
    GetEntity().OwnerController.GetGameState().RoundEndedEvent += OnRoundChanged;
  }

  protected void Use() {
    IsExecuting = true;
    UsedThisRound = true;
    CooldownRoundsLeft = CooldownRounds;
    EmitSignal(SignalName.Started);
  }
  protected void Finish() {
    IsExecuting = false;
    EmitSignal(SignalName.Finished);
  }

  public virtual bool IsAvailable() {
    return CooldownRoundsLeft == 0 && !UsedThisRound;
  }
  public virtual void OnRoundChanged(int roundNumber) {
    UsedThisRound = false;
    if (CooldownRoundsLeft > 0) {
      CooldownRoundsLeft--;
    }
  }

  public Entity GetEntity() => CommandTrait.Entity;
}


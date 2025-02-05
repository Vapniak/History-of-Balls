namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public abstract partial class Command : Node {
  [Signal] public delegate void StartedEventHandler();
  [Signal] public delegate void FinishedEventHandler();

  [Export] public string CommandName { get; private set; } = "Command";

  public CommandTrait CommandTrait { get; set; }

  protected bool IsExecuting { get; private set; }

  public override void _Ready() {
    GetEntity().OwnerController.GetGameState().RoundEndedEvent += OnRoundChanged;
  }

  public void Start() {
    IsExecuting = true;
    EmitSignal(SignalName.Started);
  }
  public void Finish() {
    IsExecuting = false;
    EmitSignal(SignalName.Finished);
  }
  public virtual bool IsAvailable() {
    return CommandTrait.CurrentExecutedCommand == null;
  }
  public virtual void OnRoundChanged(int roundNumber) {

  }

  public Entity GetEntity() => CommandTrait.Entity;
}


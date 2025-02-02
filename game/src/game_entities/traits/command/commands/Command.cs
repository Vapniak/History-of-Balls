namespace HOB.GameEntity;

using Godot;
using Godot.Collections;

// TODO: make commands be nodes
[GlobalClass]
public abstract partial class Command : Node {
  [Export] public string CommandName { get; private set; } = "Command";

  public CommandTrait CommandTrait { get; set; }

  public override void _Ready() {
    GetEntity().OwnerController.GetGameState().RoundEndedEvent += OnRoundChanged;
  }
  public abstract bool IsAvailable();
  public virtual void OnRoundChanged(int roundNumber) {

  }

  public Entity GetEntity() => CommandTrait.Entity;
}


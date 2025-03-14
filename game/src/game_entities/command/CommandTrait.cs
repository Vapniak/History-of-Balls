namespace HOB.GameEntity;

using System.Collections.Generic;
using System.Linq;
using Godot;

// TODO: fix command trait, because you cant have 2 command traits and when you will have unit traits and for example structure traits and both will have command trait it will bug out
[GlobalClass]
public partial class CommandTrait : Trait {
  [Signal] public delegate void CommandStartedEventHandler(Command command);
  [Signal] public delegate void CommandFinishedEventHandler(Command command);

  public Command CurrentExecutedCommand { get; private set; }

  // AAAAHHHH, the commands were shared between units and I spend 3 hours figuring out why all commands get changed when I change only one
  private List<Command> Commands { get; set; }
  public override void _EnterTree() {
    base._EnterTree();

    Commands = new();

    foreach (var child in GetChildren()) {
      if (child is Command command) {
        Commands.Add(command);
        command.CommandTrait = this;

        command.Started += () => {
          CurrentExecutedCommand = command;
          EmitSignal(SignalName.CommandStarted, command);
        };

        command.Finished += () => {
          CurrentExecutedCommand = null;
          EmitSignal(SignalName.CommandFinished, command);
        };
      }
    }
  }

  public Command[] GetCommands() => Commands.ToArray();
  public bool TryGetCommand<T>(out T command) where T : Command {
    command = Commands.OfType<T>().FirstOrDefault();
    if (command != null) {
      return true;
    }

    return false;
  }

  protected override void OnOwnerChanging() {
    base.OnOwnerChanging();

    foreach (var command in GetCommands()) {
      if (Entity.TryGetOwner(out var owner)) {
        owner.GetGameMode().GetMatchEvents().TurnStarted -= command.OnTurnStarted;
        owner.GetGameMode().GetMatchEvents().TurnChanged -= command.OnTurnChanged;
        owner.GetGameMode().GetMatchEvents().TurnEnded -= command.OnTurnEnded;
        owner.GetGameMode().GetMatchEvents().RoundStarted -= command.OnRoundStarted;
      }
    }
  }

  protected override void OnOwnerChanged() {
    base.OnOwnerChanged();

    foreach (var command in GetCommands()) {
      if (Entity.TryGetOwner(out var owner)) {
        owner.GetGameMode().GetMatchEvents().TurnStarted += command.OnTurnStarted;
        owner.GetGameMode().GetMatchEvents().TurnChanged += command.OnTurnChanged;
        owner.GetGameMode().GetMatchEvents().TurnEnded += command.OnTurnEnded;
        owner.GetGameMode().GetMatchEvents().RoundStarted += command.OnRoundStarted;
      }
    }
  }
}

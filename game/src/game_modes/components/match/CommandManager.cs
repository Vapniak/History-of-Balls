namespace HOB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HOB.GameEntity;

public class CommandManager {
  private Dictionary<Type, IGameCommand> _commands = new();

  public CommandManager() {
    RegisterCommands();
  }

  private void RegisterCommands() {
    var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(GameCommand<>)));

    foreach (var type in commandTypes) {
      var instance = Activator.CreateInstance(type) as IGameCommand;
      _commands[type] = instance;
    }
  }

  public IEnumerable<IGameCommand> GetAllCommands() {
    return _commands.Values;
  }

  public IEnumerable<IGameCommand> GetAvailableCommands(Entity entity) {
    return _commands.Values.Where(c => c.IsAvailable(entity));
  }
}

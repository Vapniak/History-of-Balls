namespace HOB.GameEntity;

using System;
using System.Linq;

public abstract class GameCommand<T> : IGameCommand where T : struct, ICommandParameters {
  private static readonly Type[] _requiredTraits;
  static GameCommand() {
    _requiredTraits = Attribute.GetCustomAttributes(typeof(GameCommand<T>))
            .Where(attr => attr.GetType().IsGenericType &&
                   attr.GetType().GetGenericTypeDefinition() == typeof(RequiresTraitAttribute<>))
            .Select(attr => attr.GetType().GetGenericArguments()[0])
            .ToArray();
  }

  public bool IsAvailable(Entity entity) {
    return _requiredTraits.All(t => entity.HasTrait(t));
  }

  public abstract void Execute(Entity entity, T parameters);
  public abstract bool CanExecute(Entity entity, T parameters);
}

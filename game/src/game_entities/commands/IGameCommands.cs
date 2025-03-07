namespace HOB.GameEntity;

public interface IGameCommand {
  public bool IsAvailable(Entity entity);
}

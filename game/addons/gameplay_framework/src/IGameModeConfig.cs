namespace GameplayFramework;

public interface IGameModeConfig<out T> where T : GameMode, IGameMode {
  public T CreateGameMode();
}
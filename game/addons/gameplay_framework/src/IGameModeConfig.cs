namespace GameplayFramework;

public interface IGameModeConfig<T> where T : GameMode, IGameMode {
  public T CreateGameMode();
  public void ConfigureGameMode(T gameMode);
}
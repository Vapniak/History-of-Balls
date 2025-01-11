namespace GameplayFramework;


public interface IController {
  void SetPlayerState(PlayerState playerState);
  PlayerState GetPlayerState();
  T GetPlayerState<T>() where T : PlayerState;
}

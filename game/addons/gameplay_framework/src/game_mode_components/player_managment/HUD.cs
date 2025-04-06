namespace GameplayFramework;

using Godot;

[GlobalClass]
public partial class HUD : CanvasLayer {
  private PlayerController? PlayerController { get; set; }

  public void SetPlayerController(PlayerController playerController) {
    PlayerController = playerController;
  }

  public PlayerController GetPlayerController() => PlayerController!;
  public T? GetPlayerController<T>() where T : class, IController => GetPlayerController() as T;
}

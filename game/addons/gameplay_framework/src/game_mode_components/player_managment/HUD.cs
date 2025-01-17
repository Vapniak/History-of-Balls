namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public partial class HUD : CanvasLayer {
  private PlayerController PlayerController { get; set; }

  public void SetPlayerController(PlayerController playerController) {
    PlayerController = playerController;
  }

  public PlayerController GetPlayerController() => PlayerController;
  public T GetPlayerController<T>() where T : PlayerController => GetPlayerController() as T;

  public void ShowHUD() {
    Visible = !Visible;
  }
}

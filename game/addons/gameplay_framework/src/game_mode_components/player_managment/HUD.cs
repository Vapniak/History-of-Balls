namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public partial class HUD : CanvasLayer {
  private PlayerController PlayerController { get; set; }

  public void SetPlayerController(PlayerController playerController) {
    PlayerController = playerController;
  }


  // TODO: interface for player controller
  public PlayerController GetPlayerController() => PlayerController;
  public T GetPlayerController<T>() where T : class, IController => GetPlayerController() as T;
}

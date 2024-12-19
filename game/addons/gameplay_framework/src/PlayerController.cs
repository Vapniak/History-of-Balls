namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public partial class PlayerController : Controller {
  public HUD HUD { get; private set; }

  public void SpawnHUD() {
    HUD ??= new();

    Game.GetWorld().AddChild(HUD);
  }

  public void SetHUD(HUD hud) {
    if (hud is null) {
      return;
    }

    HUD = hud;
    hud.PlayerOwner = this;
  }
  public T GetHUD<T>() where T : HUD {
    return HUD as T;
  }
}

namespace GameplayFramework;

using Godot;
using System;

/// <summary>
/// <inheritdoc/>
/// </summary>
[GlobalClass]
public partial class PlayerController : Controller {
  private HUD HUD { get; set; }
  private IPlayerControllable Controllable { get; set; }

  public void SpawnHUD() {
    HUD ??= new();
    HUD.Name = "HUD";

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

  public void SetControllable(IPlayerControllable controllable) => Controllable = controllable;
  public T GetCharacter<T>() where T : Node => Controllable.GetCharacter<T>();
}

namespace GameplayFramework;

using Godot;
using HOB;
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

    AddChild(HUD);
  }

  public void SetHUD(HUD hud) {
    if (hud is null) {
      return;
    }

    HUD = hud;
    hud.SetPlayerController(this);
  }

  public virtual HUD GetHUD() => HUD;
  public T GetHUD<T>() where T : HUD {
    return GetHUD() as T;
  }

  public void SetControllable(IPlayerControllable controllable) => Controllable = controllable;
  public T GetCharacter<T>() where T : Node => Controllable.GetCharacter<T>();
}

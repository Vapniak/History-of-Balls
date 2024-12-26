namespace GameplayFramework;

using Godot;
using System;

[GlobalClass]
public partial class HUD : Control {
  public PlayerController PlayerOwner { get; internal set; }

  public void ShowHUD() {
    Visible = !Visible;
  }
}

namespace HOB;

using GameplayFramework;
using Godot;
using System;

[GlobalClass]
public partial class TestPlayerManagmentComponent : PlayerManagmentComponent {
  protected override PlayerState CreatePlayerState() => new TestPlayerState();
}

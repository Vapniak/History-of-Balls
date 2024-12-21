namespace HOB;

using GameplayFramework;
using Godot;
using System;

[GlobalClass]
public partial class TestPlayerController : PlayerController {

  private PlayerCharacter _character;
  public override void _Ready() {
    base._Ready();

    _character = GetCharacter<PlayerCharacter>();
  }
  public override void _Process(double delta) {
    base._Process(delta);

    // TODO: character movement
  }
}

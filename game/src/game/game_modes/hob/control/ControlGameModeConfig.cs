namespace HOB;

using Godot;
using HOB;
using System;

[GlobalClass]
public partial class ControlGameModeConfig : HOBGameModeConfig {
  public override ControlGameMode CreateGameMode() {
    return ResourceLoader.Load<PackedScene>("uid://bivsyw3lakjxn").Instantiate<ControlGameMode>();
  }
}

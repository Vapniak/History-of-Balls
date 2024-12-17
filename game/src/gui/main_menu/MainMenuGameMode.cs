namespace HOB;

using Godot;
using HOB.Core;

public partial class MainMenuGameMode : GameMode {
  [Export] public Splash Splash { get; private set; }
  [Export] public ColorRect BlankScreen { get; private set; }
  [Export] public AnimationPlayer AnimationPlayer { get; private set; }

  // TODO: managment of menu logic
}

namespace HOB;

using GameplayFramework;
using Godot;
using System;

[GlobalClass]
public partial class HOBLevel : Level {
  // TODO: more centralised approach like level transition manager inside game instance scene
  [Export] private PackedScene LoadLevelTransitionScene { get; set; }
  [Export] private PackedScene UnLoadLevelTransitionScene { get; set; }

  public override void Load() {
    base.Load();

    var loadTransition = LoadLevelTransitionScene?.InstantiateOrNull<LevelTransition>();
    if (loadTransition != null) {
      loadTransition.Finished += () => {
        loadTransition.QueueFree();
      };
      AddChild(loadTransition);
    }
  }

  public override void UnLoad() {
    var unloadTransition = UnLoadLevelTransitionScene?.InstantiateOrNull<LevelTransition>();

    if (unloadTransition != null) {
      unloadTransition.Finished += () => {
        base.UnLoad();
      };

      AddChild(unloadTransition);
    }
    else {
      base.UnLoad();
    }
  }
}

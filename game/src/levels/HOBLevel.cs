namespace HOB;

using GameplayFramework;
using Godot;
using System;

[GlobalClass]
public partial class HOBLevel : Level {
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
        QueueFree();
      };

      AddChild(unloadTransition);
    }
    else {
      QueueFree();
    }
  }
}

namespace HOB;

using GameplayFramework;
using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class HOBLevel : Level {
  // TODO: more centralised approach like level transition manager inside game instance scene
  [Export] private PackedScene LoadLevelTransitionScene { get; set; }
  [Export] private PackedScene UnLoadLevelTransitionScene { get; set; }

  public override async Task Load() {
    var loadTransition = LoadLevelTransitionScene?.InstantiateOrNull<LevelTransition>();
    if (loadTransition != null) {
      await base.Load();

      var tcs = new TaskCompletionSource<bool>();

      loadTransition.Finished += () => {
        tcs.TrySetResult(true);
      };

      AddChild(loadTransition);

      await tcs.Task;

      loadTransition.QueueFree();
    }
    else {
      await base.Load();
    }
  }
  public override async Task UnLoad() {
    var unloadTransition = UnLoadLevelTransitionScene?.InstantiateOrNull<LevelTransition>();
    if (unloadTransition != null) {
      var tcs = new TaskCompletionSource<bool>();

      unloadTransition.Finished += () => {
        tcs.TrySetResult(true);
      };

      AddChild(unloadTransition);

      await tcs.Task;

      await base.UnLoad();
    }
    else {
      await base.UnLoad();
    }
  }
}

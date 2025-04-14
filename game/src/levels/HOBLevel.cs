namespace HOB;

using GameplayFramework;
using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class HOBLevel : Level {
  [Export] private PackedScene? LoadLevelTransitionScene { get; set; }
  [Export] private PackedScene? UnLoadLevelTransitionScene { get; set; }

  public override async Task Load() {
    var loadTransition = LoadLevelTransitionScene?.InstantiateOrNull<LevelTransition>();

    // TODO: level loading
    await base.Load();

    if (loadTransition != null) {
      var tcs = new TaskCompletionSource<bool>();

      loadTransition.Finished += () => tcs.TrySetResult(true);

      AddChild(loadTransition);

      await tcs.Task;

      loadTransition.QueueFree();
    }
  }
  public override async Task UnLoad() {
    var unloadTransition = UnLoadLevelTransitionScene?.InstantiateOrNull<LevelTransition>();
    if (unloadTransition != null) {
      var tcs = new TaskCompletionSource<bool>();

      unloadTransition.Finished += () => tcs.TrySetResult(true);

      AddChild(unloadTransition);

      await tcs.Task;
    }

    await base.UnLoad();
  }
}

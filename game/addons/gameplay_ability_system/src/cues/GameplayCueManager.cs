namespace GameplayAbilitySystem;

using System.Collections.Generic;
using GameplayTags;
using Godot;

public partial class GameplayCueManager : Node {
  [Export(PropertyHint.Dir)] private string GameplayCuesPath { get; set; } = "gameplay_cues/";
  public static GameplayCueManager Instance { get; private set; } = new();

  private HashSet<GameplayCue> GameplayCues { get; set; } = new();
  public override void _EnterTree() {
    Instance = this;

    using var dir = DirAccess.Open(GameplayCuesPath);
    if (dir != null) {
      dir.IncludeHidden = false;
      dir.IncludeNavigational = false;

      foreach (var fileName in dir.GetFiles()) {
        if (fileName.EndsWith(".tscn")) {
          var scenePath = GameplayCuesPath.PathJoin(fileName);
          var scene = GD.Load<PackedScene>(scenePath);

          if (scene != null) {
            var instance = scene.InstantiateOrNull<GameplayCue>();
            if (instance != null) {
              GameplayCues.Add(instance);
            }
          }
        }
      }
    }
    else {
      GD.PrintErr($"Failed to open directory: {GameplayCuesPath}");
    }
  }

  public void HandleGameplayCue(Node target, Tag gameplayCueTag, GameplayCueEventType eventType, GameplayCueParameters parameters) {
    foreach (var cue in GameplayCues) {
      if (cue.CueTag.IsExact(gameplayCueTag)) {
        cue.HandleGameplayCue(target, eventType, parameters);
      }
    }
  }

  public override void _ExitTree() {
    Instance = null;
  }
}

namespace GameplayAbilitySystem;

using System.Collections.Generic;
using GameplayTags;
using Godot;

public partial class GameplayCueManager : Node {
  [Export(PropertyHint.Dir)] private string GameplayCuesPath { get; set; } = "gameplay_cues/";
  public static GameplayCueManager Instance { get; private set; } = new();

  private List<GameplayCue> GameplayCues { get; set; } = new();
  public override void _EnterTree() {
    Instance = this;

    LoadAllScenesRecursively(GameplayCuesPath);
  }

  public void HandleGameplayCue(Tag gameplayCueTag, GameplayCueEventType eventType, GameplayCueParameters parameters) {
    foreach (var cue in GameplayCues) {
      if (cue.CueTag.IsExact(gameplayCueTag)) {
        var c = (cue.Duplicate((int)DuplicateFlags.UseInstantiation) as GameplayCue)!;
        c.Ready += () => c.HandleGameplayCue(eventType, parameters);
        AddChild(c);
      }
    }
  }

  public override void _ExitTree() {
    Instance = null;
  }

  public void LoadAllScenesRecursively(string directoryPath) {
    var scenes = new List<GameplayCue>();
    ScanDirectoryRecursive(directoryPath, scenes);
    GameplayCues.AddRange(scenes);
  }

  private static void ScanDirectoryRecursive(string currentPath, List<GameplayCue> result) {
    using var dir = DirAccess.Open(currentPath);
    if (dir == null) {
      GD.PushError($"Failed to open directory: {currentPath}");
      return;
    }

    dir.IncludeHidden = false;
    dir.IncludeNavigational = false;

    foreach (var fileName in dir.GetFiles()) {
      if (fileName.EndsWith(".tscn")) {
        var fullPath = System.IO.Path.Combine(currentPath, fileName);
        var scene = GD.Load<PackedScene>(fullPath);

        if (scene?.InstantiateOrNull<GameplayCue>() is GameplayCue instance) {
          result.Add(instance);
        }
      }
    }

    foreach (var dirName in dir.GetDirectories()) {
      ScanDirectoryRecursive(System.IO.Path.Combine(currentPath, dirName), result);
    }
  }
}

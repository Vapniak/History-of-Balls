#if TOOLS
namespace GameplayAbilitySystem;

using Godot;
using System;

[Tool]
public partial class Plugin : EditorPlugin {
  public override void _EnterTree() {
    // Initialization of the plugin goes here.
    AddAutoloadSingleton("GameplayCueManager", "src/cues/gameplay_cue_manager.tscn");
  }

  public override void _ExitTree() {
    // Clean-up of the plugin goes here.
    RemoveAutoloadSingleton("GameplayCueManager");
  }
}
#endif

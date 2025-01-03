#if TOOLS

namespace RaycastSystem;

using Godot;

[Tool]
public partial class Plugin : EditorPlugin {

  public override void _EnterTree() {
    // Initialization of the plugin goes here.
    AddAutoloadSingleton(nameof(RaycastSystem), "/src/globals/RaycastSystem.cs");
  }

  public override void _ExitTree() {
    // Clean-up of the plugin goes here.
    RemoveAutoloadSingleton(nameof(RaycastSystem));
  }
}
#endif

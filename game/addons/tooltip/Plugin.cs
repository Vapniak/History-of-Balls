#if TOOLS
namespace Tooltip;

using Godot;
using System;

[Tool]
public partial class Plugin : EditorPlugin {
  public override void _EnterTree() {
    // Initialization of the plugin goes here.
    AddAutoloadSingleton("TooltipManager", "src/tooltip_manager.tscn");
  }

  public override void _ExitTree() {
    // Clean-up of the plugin goes here.
    RemoveAutoloadSingleton("TooltipManager");
  }
}
#endif

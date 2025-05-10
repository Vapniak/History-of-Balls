#if TOOLS
namespace Console;

using Godot;
using System;

[Tool]
public partial class Plugin : EditorPlugin {
  public override void _EnterTree() {
    // Initialization of the plugin goes here.
    AddAutoloadSingleton("ConsoleAutoload", "src/Console.tscn");
  }

  public override void _ExitTree() {
    // Clean-up of the plugin goes here.
    RemoveAutoloadSingleton("ConsoleAutoload");
  }
}
#endif

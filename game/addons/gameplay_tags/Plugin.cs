#if TOOLS
namespace GameplayTags;
using Godot;


[Tool]
public partial class Plugin : EditorPlugin {
  private TagEditorPlugin _tagPropertyInspector;

  public override void _EnterTree() {
    // Initialize the custom property inspector
    _tagPropertyInspector = new TagEditorPlugin();

    AddToolMenuItem("Reinitialize TagRegistry", Callable.From(TagManager.Initialize));

    // Register the inspector for Tag properties
    AddInspectorPlugin(_tagPropertyInspector);
  }

  public override void _ExitTree() {
    // Clean up when the plugin is disabled
    RemoveInspectorPlugin(_tagPropertyInspector);
    RemoveToolMenuItem("Reinitialize TagRegistry");
  }
}
#endif

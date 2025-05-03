namespace HOB;

using Godot;

[GlobalClass]
public partial class TutorialGameModeConfig : HOBGameModeConfig {
  public override TutorialGameMode CreateGameMode() {
    return ResourceLoader.Load<PackedScene>("uid://bwhksvkei510f").Instantiate<TutorialGameMode>();
  }
}

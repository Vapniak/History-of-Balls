namespace HOB;

using System.Threading.Tasks;
using GameplayFramework;
using Godot;
using WidgetSystem;

[GlobalClass]
public partial class MainMenuWidget : Widget, IWidgetFactory<MainMenuWidget> {
  private static void OnStartGamePressed() {
    _ = GameInstance.GetGameMode<MainMenuGameMode>()?.StartGame();
  }
  private void OnSettingsPressed() {
    WidgetManager.PushWidget<SettingsMenu>();
  }

  private void OnTutorialPressed() {
    WidgetManager.PushWidget<TutorialMenuWidget>();
  }
  private static void OnQuitButtonPressed() {
    GameInstance.QuitGame();
  }

  static MainMenuWidget IWidgetFactory<MainMenuWidget>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://d4knun30ujmci").Instantiate<MainMenuWidget>();
  }
}

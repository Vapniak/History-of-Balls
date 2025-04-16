namespace HOB;

using System.Threading.Tasks;
using GameplayFramework;
using Godot;
using WidgetSystem;

[GlobalClass]
public partial class MainMenuWidget : Widget, IWidgetFactory<MainMenuWidget> {
  private static readonly string _menu = "uid://d4knun30ujmci";

  private void OnStartGamePressed() {
    _ = GameInstance.GetGameMode<MainMenuGameMode>().StartGame();
  }
  private void OnSettingsPressed() {
    WidgetManager.PushWidget<SettingsMenu>();
  }

  private void OnTutorialPressed() {
    WidgetManager.PushWidget<TutorialMenuWidget>();
  }
  private void OnQuitButtonPressed() {
    GameInstance.QuitGame();
  }

  static MainMenuWidget IWidgetFactory<MainMenuWidget>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>(_menu).Instantiate<MainMenuWidget>();
  }
}

namespace HOB;

using System.Threading.Tasks;
using GameplayFramework;
using Godot;
using WidgetSystem;

public partial class MainMenu : Widget, IWidget<MainMenu> {
  private static readonly string _menu = "uid://d4knun30ujmci";

  private void OnStartGamePressed() {
    _ = GameInstance.GetGameMode<MainMenuGameMode>().StartGame();
  }
  private void OnSettingsPressed() {
    WidgetManager.PushWidget<SettingsMenu>();
  }

  private void OnTutorialPressed() {
    WidgetManager.PushWidget<TutorialPanel>();
  }
  private void OnQuitButtonPressed() {
    GameInstance.QuitGame();
  }

  static MainMenu IWidget<MainMenu>.Create() {
    return ResourceLoader.Load<PackedScene>(_menu).Instantiate<MainMenu>();
  }
}

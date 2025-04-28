namespace HOB;

using System.Threading.Tasks;
using GameplayFramework;
using Godot;
using WidgetSystem;

[GlobalClass]
public partial class MainMenuWidget : HOBWidget, IWidgetFactory<MainMenuWidget> {

  private SelectCampaignWidget SelectCampaignWidget;

  public override void _EnterTree() {
    base._EnterTree();

    SelectCampaignWidget = SelectCampaignWidget.CreateWidget();
  }
  private void OnSelectMissionPressed() {
    //_ = GameInstance.GetGameMode<MainMenuGameMode>()?.StartGame();
    WidgetManager.PushWidget(SelectCampaignWidget);
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

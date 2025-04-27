namespace HOB;

using Godot;
using GameplayFramework;
using AudioManager;
using WidgetSystem;
using System.Threading.Tasks;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  [Export] private PackedScene _loadingScreenScene = default!;
  [Export] private SurviveGameModeConfig Config;

  protected override GameState CreateGameState() => new MainMenuGameState();

  public override void _Ready() {
    base._Ready();
    WidgetManager.Instance.PushWidget<MainMenuWidget>();
    MusicManager.Instance.Play("music", "main_menu", 0, true);
  }

  public override void _ExitTree() {
    WidgetManager.Instance.PopAllWidgets();
  }

  public void StartMission(MissionData missionData) {
    _ = GameInstance.GetWorld().OpenLevelThreaded("mission_level", _loadingScreenScene, missionData.GetGameModeConfig());
  }

  // public async Task StartGame() {
  //   await GameInstance.GetWorld().OpenLevelThreaded("test_level", _loadingScreenScene, Config);
  // }

  public void Quit() => GameInstance.QuitGame();
}

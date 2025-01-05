namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class TestGameMode : GameMode {
  [Export] private PackedScene TestEntity { get; set; }


  private PauseComponent PauseComponent { get; set; }
  private TestPlayerManagmentComponent PlayerManagmentComponent { get; set; }
  private MatchComponent MatchComponent { get; set; }

  public override void Init() {
    base.Init();

    GetGameState<TestGameState>().GameBoard = Game.GetWorld().CurrentLevel.GetChildByType<GameBoard>();

    PauseComponent = GetGameModeComponent<PauseComponent>();
    PauseComponent.GetPauseMenu().Resume += OnResume;
    PauseComponent.GetPauseMenu().MainMenu += OnMainMenu;
    PauseComponent.GetPauseMenu().Quit += OnQuit;

    PlayerManagmentComponent = GetGameModeComponent<TestPlayerManagmentComponent>();

    MatchComponent = GetGameModeComponent<MatchComponent>();


    // TODO: initialization of entities from map
    GetGameState<TestGameState>().GameBoard.GridCreated += () => {
      var entity = TestEntity.InstantiateOrNull<Entity>();
      MatchComponent.SpawnEntity(entity, new(0, 0));
    };
  }

  public override void _Process(double delta) {
    base._Process(delta);

    if (Input.IsActionJustPressed(BuiltinInputActions.UICancel)) {
      PauseComponent.Pause();
    }
  }

  protected override GameState CreateGameState() => new TestGameState();

  private void OnResume() => PauseComponent.Resume();
  private void OnMainMenu() {
    OnResume();
    Game.GetWorld().OpenLevel("main_menu_level");
  }

  private void OnQuit() => Game.QuitGame();
}

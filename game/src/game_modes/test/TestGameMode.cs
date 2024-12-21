namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;

[GlobalClass]
public partial class TestGameMode : GameMode {
  public override void _Ready() {
    base._Ready();

    GetGameState<TestGameState>().HexGrid = Game.GetWorld().CurrentLevel.GetChildByType<HexGrid>();
  }

  protected override GameState CreateGameState() => new TestGameState();
  protected override PlayerState CreatePlayerState() => new TestPlayerState();
}

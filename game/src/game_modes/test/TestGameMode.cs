namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;

[GlobalClass]
public partial class TestGameMode : GameMode {
  public override void _Ready() {
    base._Ready();

    GetGameState<TestGameState>().GameBoard = Game.GetWorld().CurrentLevel.GetChildByType<GameBoard>();
  }

  protected override GameState CreateGameState() => new TestGameState();
}

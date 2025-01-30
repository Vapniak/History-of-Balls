namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent {
  // TODO: handle current player turns
  [Export] private PackedScene TestEntity { get; set; }
  [Export] private PackedScene TestEntity2 { get; set; }

  private GameBoard GameBoard { get; set; }

  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(PlayerState playerState) {
    // TODO: spawning from map
    if (playerState.GetController() is PlayerController) {
      var entity = TestEntity.InstantiateOrNull<Entity>();
      var entity2 = TestEntity2.InstantiateOrNull<Entity>();
      GameBoard.AddEntity(entity, new(0, 0), playerState.GetController<IMatchController>());
      GameBoard.AddEntity(entity2, new(1, 0), playerState.GetController<IMatchController>());
    }
    else {
      var entity = TestEntity.InstantiateOrNull<Entity>();
      var entity2 = TestEntity2.InstantiateOrNull<Entity>();
      GameBoard.AddEntity(entity, new(5, 0), playerState.GetController<IMatchController>());
      GameBoard.AddEntity(entity2, new(6, 0), playerState.GetController<IMatchController>());
    }
  }
}

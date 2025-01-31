namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent {
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

    var controller = playerState.GetController<IMatchController>();
    controller.EndTurnEvent += () => OnEndTurn(controller);

    if (controller is PlayerController) {
      var entity = TestEntity.InstantiateOrNull<Entity>().Duplicate() as Entity;
      var entity2 = TestEntity2.InstantiateOrNull<Entity>().Duplicate() as Entity;
      var entity3 = TestEntity2.InstantiateOrNull<Entity>().Duplicate() as Entity;
      GameBoard.AddEntity(entity, new(0, 0), controller);
      GameBoard.AddEntity(entity2, new(1, 0), controller);
      GameBoard.AddEntity(entity3, new(2, 0), controller);
    }
    else {
      var entity = TestEntity.InstantiateOrNull<Entity>().Duplicate() as Entity;
      var entity2 = TestEntity2.InstantiateOrNull<Entity>().Duplicate() as Entity;
      GameBoard.AddEntity(entity, new(5, 0), controller);
      GameBoard.AddEntity(entity2, new(6, 0), controller);
    }
  }

  private void OnEndTurn(IMatchController controller) {
    if (GetGameState().IsCurrentTurn(controller)) {
      GetGameState().NextTurn();
    }
  }
}

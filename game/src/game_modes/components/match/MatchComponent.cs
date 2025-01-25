namespace HOB;

using System.Linq;
using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;


/// <summary>
/// Manages entities and turns of each player.
/// </summary>
[GlobalClass]
public partial class MatchComponent : GameModeComponent {
  // TODO: handle current player turns

  [Export] private PackedScene TestEntity { get; set; }
  [Export] private PackedScene TestEntity2 { get; set; }

  private IMatchController CurrentController { get; set; }
  private GameBoard GameBoard { get; set; }

  private Entity SelectedEntity { get; set; }
  private Action CurrentAction { get; set; }

  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public virtual void OnPlayerSpawned(PlayerState playerState) {
    // TODO: spawning from map
    var entity = TestEntity.InstantiateOrNull<Entity>();
    var entity2 = TestEntity2.InstantiateOrNull<Entity>();
    GameBoard.AddEntity(entity, new(0, 0), playerState.GetController<IMatchController>());
    GameBoard.AddEntity(entity2, new(1, 0), playerState.GetController<IMatchController>());

    playerState.GetController<IMatchController>().CoordClicked += (coord) => OnCoordClicked(playerState.GetController<IMatchController>(), coord);

    CurrentController = playerState.GetController<IMatchController>();
  }

  private void OnCoordClicked(IMatchController controller, CubeCoord coord) {
    if (CurrentController != controller) {
      return;
    }

    var cell = GameBoard.GetCell(coord);

    if (cell == null) {
      return;
    }

    SelectCell(cell);
  }

  // TODO: selection system and showing actions
  // TODO: state machine for state of current player ex. performing action, selecting unit
  private void SelectCell(GameCell cell) {
    var entities = GameBoard.GetOwnedEntitiesOnCell(CurrentController, cell);

    if (CurrentAction != null) {
      if (CurrentAction is MoveAction) {
        if (SelectedEntity.TryGetTrait<MoveTrait>(out var moveTrait)) {
          // TODO: check if in range
          if (entities.Length == 0 && GameBoard.GetCellsInRange(SelectedEntity.Cell.Coord, moveTrait.Data.MoveRange).Contains(cell)) {
            moveTrait.Move(cell);
            CurrentAction.Finish();
          }
          return;
        }
      }
    }



    if (entities.Length > 0) {
      if (entities[0] != SelectedEntity) {
        if (SelectedEntity != null) {
          DeselectEntity(SelectedEntity);
        }
        SelectedEntity = entities[0];
        SelectEntity(SelectedEntity);
      }
    }
    else if (SelectedEntity != null) {
      DeselectEntity(SelectedEntity);
      SelectedEntity = null;
    }

    GD.PrintS("Entities:", entities.Length, "Q:", cell.Coord.Q, "R:", cell.Coord.R);
  }

  private void SelectEntity(Entity entity) {
    if (entity.TryGetTrait<ActionsTrait>(out var actions)) {
      actions.ShowActionMenu();

      foreach (var action in actions.Actions) {
        void startedHandler() {
          OnActionStart(action);
          action.Started -= startedHandler;
        }

        void finishedHandler() {
          OnActionFinish(action);
          action.Finished -= finishedHandler;
        }

        action.Started += startedHandler;
        action.Finished += finishedHandler;
      }
    }
  }

  private void DeselectEntity(Entity entity) {
    if (entity.TryGetTrait<ActionsTrait>(out var actions)) {
      actions.HideActionMenu();

      GameBoard.ClearHighlights();
      // highlight units which you can select
      GameBoard.UpdateHighlights();
    }
  }

  private void OnActionStart(Action action) {
    CurrentAction = action;

    if (action is MoveAction move) {
      // TODO: maybe move it all to action, but it will tightly couple it with game systems
      if (action.GetEntity().TryGetTrait<MoveTrait>(out var moveTrait)) {
        foreach (var cell in GameBoard.GetCellsInRange(action.GetEntity().Cell.Coord, moveTrait.Data.MoveRange)) {
          if (GameBoard.GetEntitiesOnCell(cell).Length == 0) {
            cell.HighlightColor = Colors.Green;
          }
        }

        GameBoard.UpdateHighlights();
      }
    }
  }

  private void OnActionFinish(Action action) {
    CurrentAction = null;

    DeselectEntity(SelectedEntity);
    SelectedEntity = null;
  }
}

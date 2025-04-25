namespace HOB;

using System.Collections.Generic;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class HOBPlayerController {

  public partial class HighlightSystem : Node {
    private readonly GameBoard _gameBoard;
    private Dictionary<HighlightType, Color> HighlightColors { get; set; }
    private Dictionary<GameCell, Dictionary<HighlightType, Color>> ActiveHighlights { get; set; }

    private HOBPlayerController Controller { get; set; }

    private Entity? SelectedEntity => Controller.SelectedEntity;
    private HOBAbility.Instance? SelectedCommand => Controller.SelectedCommand;

    public HighlightSystem(HOBPlayerController controller, Godot.Collections.Array<HighlightColorMap> highlightColors, GameBoard gameBoard) {
      _gameBoard = gameBoard;
      Controller = controller;


      HighlightColors = new();

      foreach (var map in highlightColors) {
        HighlightColors.Add(map.Type, map.Color);
      }
      ActiveHighlights = new Dictionary<GameCell, Dictionary<HighlightType, Color>>();


      //Controller.SelectedEntityChanged += OnSelectedEntityChanged;
      //Controller.SelectedCommandChanged += OnSelectedCommandChanged;
    }

    public override void _PhysicsProcess(double delta) {
      UpdateSelectedCommandHighlights();
    }

    private void SetHighlight(HighlightType type, GameCell cell, bool darken = false) {
      if (!ActiveHighlights.TryGetValue(cell, out var value)) {
        value = new Dictionary<HighlightType, Color>();
        ActiveHighlights[cell] = value;
      }

      if (HighlightColors.TryGetValue(type, out var color)) {
        value[type] = darken ? color.Darkened(0.5f) : color;
      }
    }
    private void ClearAllHighlights() {
      ActiveHighlights.Clear();
      _gameBoard.ClearHighlights();
    }

    private void UpdateHighlights() {
      foreach (var (cell, highlights) in ActiveHighlights) {
        foreach (var (type, color) in highlights) {
          _gameBoard.SetHighlight(cell, color);
        }
      }

      _gameBoard.UpdateHighlights();
    }

    // private void OnSelectedEntityCellChanged() {
    //   ClearAllHighlights();

    //   if (SelectedEntity != null) {
    //     SetHighlight(HighlightType.Selection, SelectedEntity.Cell);
    //   }

    //   UpdateHighlights();
    // }

    // private void OnSelectedEntityDied() {
    //   ClearAllHighlights();
    //   UpdateHighlights();
    // }

    // private void OnSelectedEntityChanged() {
    //   ClearAllHighlights();

    //   if (SelectedEntity != null) {
    //     SetHighlight(HighlightType.Selection, SelectedEntity.Cell);
    //   }

    //   UpdateHighlights();
    // }

    private void UpdateSelectedCommandHighlights() {
      ClearAllHighlights();

      if (SelectedEntity != null) {
        SetHighlight(HighlightType.Selection, SelectedEntity.Cell);
      }

      if (SelectedCommand != null) {
        var darken = !SelectedCommand.CanActivateAbility(new() { Activator = Controller });

        if (SelectedCommand is MoveAbility.Instance moveAbility) {
          foreach (var cell in moveAbility.GetReachableCells()) {
            SetHighlight(HighlightType.Movement, cell, darken);
          }
        }
        else if (SelectedCommand is AttackAbility.Instance attackAbility) {
          foreach (var cell in attackAbility.GetAttackableEntities().cellsInRange) {
            SetHighlight(HighlightType.Attack, cell, darken);
          }
        }
      }

      UpdateHighlights();
    }
  }
}

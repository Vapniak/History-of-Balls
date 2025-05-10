namespace HOB;

using System.Collections.Generic;
using System.Linq;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class HOBPlayerController {
  public partial class HighlightSystem : Node {
    private class HighlightState {
      public Color CurrentColor;
      public Color TargetColor;
      public float TransitionProgress;
      public float TransitionSpeed = 10f;
    }

    private readonly GameBoard _gameBoard;
    private readonly Dictionary<GameCell, HighlightState> _highlightStates = new();
    private HOBPlayerController Controller { get; set; }
    private Entity? SelectedEntity => Controller.SelectedEntity;
    private HOBAbility.Instance? SelectedCommand => Controller.SelectedCommand;
    private HOBTheme? Theme => Controller.GetPlayerState().Theme as HOBTheme;

    public HighlightSystem(HOBPlayerController controller, GameBoard gameBoard) {
      _gameBoard = gameBoard;
      Controller = controller;

      Controller.HoveredCellChanged += () => {
        var cell = Controller.HoveredCell;

        var viewport = GetViewport();
        // || _isPanning || _isOrbiting ||
        if (cell == null || ProjectSettings.GetSetting("application/devmode", false).AsBool() || (viewport.GuiGetHoveredControl() != null && viewport.GuiGetHoveredControl().GetParent() is Control control && control.Visible)) {
          gameBoard.SetMouseHighlight(false);
        }
        else {
          gameBoard.SetMouseHighlight(true);
        }
      };
    }

    public override void _PhysicsProcess(double delta) {
      UpdateTransitions((float)delta);

      UpdateHighlightLogic();

      DisplayHighlights();
    }

    private void SetHighlight(Color targetColor, GameCell cell, bool darken = false) {
      if (Theme == null) {
        return;
      }

      targetColor = darken ? targetColor.Darkened(0.5f) : targetColor;

      if (!_highlightStates.TryGetValue(cell, out var state)) {
        state = new HighlightState {
          CurrentColor = targetColor,
          TargetColor = targetColor,
        };
        _highlightStates[cell] = state;
      }
      else {
        if (state.TargetColor != targetColor) {
          state.TargetColor = targetColor;
        }
      }
    }

    private void ClearAllHighlights() {
      foreach (var state in _highlightStates.Values) {
        state.TargetColor = state.CurrentColor with { A = 0f };
      }
    }

    private void UpdateTransitions(float deltaTime) {
      List<GameCell> toRemove = new();

      foreach (var (cell, state) in _highlightStates) {
        if (state.TargetColor.A < 0.1f &&
                state.CurrentColor.A < 0.1f) {
          toRemove.Add(cell);
        }
        else {
          state.CurrentColor = state.CurrentColor.Lerp(
              state.TargetColor,
              deltaTime * state.TransitionSpeed);
        }
      }

      foreach (var cell in toRemove) {
        _highlightStates.Remove(cell);
      }
    }

    private void UpdateHighlightLogic() {
      ClearAllHighlights();


      if (Theme == null || ProjectSettings.GetSetting("application/devmode", false).AsBool()) {
        return;
      }

      if (SelectedCommand == null) {

        foreach (var entity in Controller.EntityManagment.GetEntities()) {
          if (entity.TryGetOwner(out var owner)) {
            if (owner == Controller) {
              var showColor = false;
              foreach (var ability in entity.AbilitySystem.GetGrantedAbilities()) {
                if (ability.CanActivateAbility(null)) {
                  if (ability is AttackAbility.Instance attack) {
                    if (attack.GetAttackableEntities().entities.Length > 0) {
                      showColor = true;
                      break;
                    }
                  }

                  if (ability is MoveAbility.Instance) {
                    showColor = true;
                    break;
                  }

                  if (ability is EntityProductionAbility.Instance prod) {
                    if (owner.GetPlayerState().ProducedEntities.Any(e =>
                        prod.CanActivateAbility(new() {
                          Activator = owner,
                          TargetData = new() { Target = e }
                        }))) {
                      showColor = true;
                      break;
                    }
                  }
                }
              }

              if (showColor) {
                SetHighlight(owner.GetPlayerState().Country.Color, entity.Cell);
              }
            }
            else {
              SetHighlight(owner!.GetPlayerState().Country.Color, entity.Cell);
            }
          }
        }
      }

      if (SelectedEntity != null) {
        SetHighlight(Theme.FontColor, SelectedEntity.Cell);
      }

      if (SelectedCommand != null) {
        var darken = !SelectedCommand.CanActivateAbility(new() { Activator = Controller });

        if (SelectedCommand is MoveAbility.Instance moveAbility) {
          foreach (var cell in moveAbility.GetReachableCells()) {
            SetHighlight(Theme.MoveAbilityColor, cell, darken);
          }
        }
        else if (SelectedCommand is AttackAbility.Instance attackAbility) {
          foreach (var cell in attackAbility.GetAttackableEntities().cellsInRange) {
            SetHighlight(Theme.AttackAbilityColor, cell, darken);
          }
        }
      }
    }

    private void DisplayHighlights() {
      _gameBoard.ClearHighlights();

      foreach (var (cell, state) in _highlightStates) {
        _gameBoard.SetHighlight(cell, state.CurrentColor);
      }

      _gameBoard.UpdateHighlights();
    }
  }
}

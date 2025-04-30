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
      public float TransitionSpeed = 15f;
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
    }

    public override void _PhysicsProcess(double delta) {
      UpdateTransitions((float)delta);

      UpdateHighlightLogic();

      DisplayHighlights();
    }

    private void SetHighlight(Color targetColor, GameCell cell, bool darken = false) {
      if (Theme == null)
        return;

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

      if (Theme == null) {
        return;
      }

      foreach (var entity in Controller.EntityManagment.GetEntities()) {
        if (entity.TryGetOwner(out var owner)) {
          if (owner == Controller) {
            var color = Colors.Transparent;
            foreach (var ability in entity.AbilitySystem.GetGrantedAbilities()) {
              if (ability.CanActivateAbility(null)) {
                if (ability is AttackAbility.Instance) {
                  color = color.Blend(Theme.AttackAbilityColor with { A = 0.5f });
                }

                if (ability is MoveAbility.Instance) {
                  color = color.Blend(Theme.MoveAbilityColor with { A = 0.5f });
                }

                if (ability is EntityProductionAbility.Instance prod) {
                  if (owner.GetPlayerState().ProducedEntities.Any(e =>
                      prod.CanActivateAbility(new() {
                        Activator = owner,
                        TargetData = new() { Target = e }
                      }))) {
                    color = color.Blend(owner.GetPlayerState().Country.Color with { A = 0.5f });
                  }
                }
              }
            }

            if (color != Colors.Transparent) {
              SetHighlight(color, entity.Cell);
            }
          }
          else {
            SetHighlight(owner!.GetPlayerState().Country.Color, entity.Cell);
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

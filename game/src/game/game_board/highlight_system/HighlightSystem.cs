namespace HOB;

using System.Collections.Generic;
using Godot;

public class HighlightSystem {
  private readonly GameBoard _gameBoard;
  private Dictionary<HighlightType, Color> HighlightColors { get; set; }
  private Dictionary<GameCell, Dictionary<HighlightType, Color>> ActiveHighlights { get; set; }

  public HighlightSystem(Godot.Collections.Array<HighlightColorMap> highlightColors, GameBoard gameBoard) {
    _gameBoard = gameBoard;

    HighlightColors = new();

    foreach (var map in highlightColors) {
      HighlightColors.Add(map.Type, map.Color);
    }
    ActiveHighlights = new Dictionary<GameCell, Dictionary<HighlightType, Color>>();
  }

  public void SetHighlight(HighlightType type, GameCell cell, bool darken = false) {
    if (!ActiveHighlights.TryGetValue(cell, out var value)) {
      value = new Dictionary<HighlightType, Color>();
      ActiveHighlights[cell] = value;
    }

    if (HighlightColors.TryGetValue(type, out var color)) {
      value[type] = darken ? color.Darkened(0.5f) : color;
    }
  }
  public void ClearAllHighlights() {
    ActiveHighlights.Clear();
    _gameBoard.ClearHighlights();
  }
  public void UpdateHighlights() {
    foreach (var (cell, highlights) in ActiveHighlights) {
      foreach (var (type, color) in highlights) {
        _gameBoard.SetHighlight(cell, color);
      }
    }

    _gameBoard.UpdateHighlights();
  }
}

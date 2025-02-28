namespace HOB;

using System;
using System.Collections.Generic;
using Godot;

public class HighlightSystem {
  private readonly GameBoard _gameBoard;

  private Dictionary<HighlightType, Color> HighlightColors { get; set; } = new();
  private Dictionary<HighlightType, HashSet<GameCell>> Highlights { get; set; }

  public HighlightSystem(Godot.Collections.Array<HighlightColorMap> highlightColors, GameBoard gameBoard) {
    _gameBoard = gameBoard;

    foreach (var map in highlightColors) {
      HighlightColors.Add(map.Type, map.Color);
    }

    Highlights = new();
  }


  public void SetHighlight(HighlightType type, GameCell cell) {
    if (!Highlights.TryGetValue(type, out var value)) {
      value = new HashSet<GameCell>();
      Highlights[type] = value;
    }

    value.Add(cell);
  }

  public void ClearHighlights(HighlightType type) {
    if (Highlights.TryGetValue(type, out var value)) {
      value.Clear();
    }
  }

  public void ClearAllHighlights() {
    foreach (var type in Highlights.Keys) {
      Highlights[type].Clear();
    }
  }

  public void UpdateHighlights() {
    _gameBoard.ClearHighlights();

    foreach (var (type, cells) in Highlights) {
      if (HighlightColors.TryGetValue(type, out var color)) {
        foreach (var cell in cells) {
          _gameBoard.SetHighlight(cell, color);
        }
      }
    }

    _gameBoard.UpdateHighlights();
  }
}

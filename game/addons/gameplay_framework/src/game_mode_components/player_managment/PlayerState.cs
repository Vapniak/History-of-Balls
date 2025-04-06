namespace GameplayFramework;

using Godot;
using System;

/// <summary>
/// Holds every variable non related to current player character etc. score, player name, health, mana.
/// </summary>
[GlobalClass]
public partial class PlayerState : Node, IPlayerState {
  public string PlayerName { get; set; } = "Player";
  public int PlayerIndex { get; set; }
  private Controller? OwningController { get; set; }
  public PlayerState() { }

  public void SetPlayerName(string playerName) => PlayerName = playerName;
  public void SetPlayerIndex(int index) => PlayerIndex = index;

  public T? GetController<T>() where T : class, IController => GetController() as T;
  public IController GetController() => OwningController!;
  public void SetController(Controller controller) {
    OwningController = controller;
  }
}

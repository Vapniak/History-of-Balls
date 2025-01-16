namespace HOB;

using System;
using Godot;

public interface IPauseMenu {
  public Action Resume { get; set; }
  public Action MainMenu { get; set; }
  public Action Quit { get; set; }
}

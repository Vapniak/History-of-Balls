namespace HOB;

using System;
using Godot;

public interface IPauseMenu {
  Action Resume { get; set; }
  Action MainMenu { get; set; }
  Action Quit { get; set; }
}

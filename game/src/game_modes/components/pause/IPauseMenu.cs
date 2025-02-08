namespace HOB;

using System;
using Godot;

public interface IPauseMenu {
  public event Action ResumeEvent;
  public event Action MainMenuEvent;
  public event Action QuitEvent;
}

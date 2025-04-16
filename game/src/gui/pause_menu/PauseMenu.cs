namespace HOB;

using Godot;
using System;
using WidgetSystem;

public partial class PauseMenu : Widget, IWidget<PauseMenu> {
  public event Action? ResumeEvent;
  public event Action? MainMenuEvent;
  public event Action? QuitEvent;

  private void OnResumePressed() {
    ResumeEvent?.Invoke();
    CloseWidget();
  }

  public override void _ExitTree() {
    ResumeEvent?.Invoke();
  }

  private void OnSettingsPressed() {
    WidgetManager.PushWidget<SettingsMenu>();
  }

  private void OnTutorialPressed() {
    WidgetManager.PushWidget<TutorialPanel>();
  }

  private void OnMainMenuPressed() {
    MainMenuEvent?.Invoke();
  }

  private void OnQuitPressed() {
    QuitEvent?.Invoke();
  }

  static PauseMenu IWidget<PauseMenu>.Create() {
    return ResourceLoader.Load<PackedScene>("uid://y6icx2blahqk").Instantiate<PauseMenu>();
  }

  public override bool CanBePopped() => true;
}

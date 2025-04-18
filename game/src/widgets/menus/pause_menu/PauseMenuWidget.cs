namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class PauseMenuWidget : Widget, IWidgetFactory<PauseMenuWidget> {
  public event Action? ResumeEvent;
  public event Action? MainMenuEvent;
  public event Action? QuitEvent;

  private void OnResumePressed() {
    ResumeEvent?.Invoke();
    PopWidget();
  }

  public override void OnPopped() {
    base.OnPopped();
    ResumeEvent?.Invoke();
  }

  private void OnSettingsPressed() {
    WidgetManager.PushWidget<SettingsMenu>();
  }

  private void OnTutorialPressed() {
    WidgetManager.PushWidget<TutorialMenuWidget>();
  }

  private void OnMainMenuPressed() {
    MainMenuEvent?.Invoke();
  }

  private void OnQuitPressed() {
    QuitEvent?.Invoke();
  }

  static PauseMenuWidget IWidgetFactory<PauseMenuWidget>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://y6icx2blahqk").Instantiate<PauseMenuWidget>();
  }

  public override bool CanBePopped() => true;
}

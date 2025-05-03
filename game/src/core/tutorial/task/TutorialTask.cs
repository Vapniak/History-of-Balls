namespace HOB;

using System;
using Godot;

public partial class TutorialTask {
  public event Action? Completed;

  public bool IsCompeted { get; private set; }
  public virtual string? Text { get; private set; }

  public HOBPlayerController PlayerController { get; private set; }
  public TutorialTask(HOBPlayerController controller, string? textOverride = null) {
    PlayerController = controller;
    if (textOverride != null) {
      Text = textOverride;
    }
  }

  public virtual void Start() {

  }

  protected virtual void Complete() {
    IsCompeted = true;
    Completed?.Invoke();
    GD.Print("TASK: ", Text, " COMPLETED");
  }
}

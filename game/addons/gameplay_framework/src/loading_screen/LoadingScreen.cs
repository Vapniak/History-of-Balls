namespace GameplayFramework;

using Godot;

public partial class LoadingScreen : CanvasLayer {
  [Signal] public delegate void FullReachedEventHandler();
  [Export] private ProgressBar ProgressBar { get; set; }

  private float _value;

  public override void _Process(double delta) {
    ProgressBar.Value = Mathf.MoveToward(ProgressBar.Value, _value, (float)delta * 100);
    if (ProgressBar.Value == 100) {
      EmitSignal(SignalName.FullReached);
    }
  }
  public void SetProgressBarValue(float value) {
    _value = value;
  }

  public float GetProgressBarValue() => (float)ProgressBar.Value;
}

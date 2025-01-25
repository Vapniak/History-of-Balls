namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public abstract partial class Action : Resource {
  [Signal] public delegate void StartedEventHandler();
  [Signal] public delegate void FinishedEventHandler();

  [Export] public string Name { get; private set; }

  public ActionsTrait OwnerTrait { get; set; }

  public virtual void Start() {
    EmitSignal(SignalName.Started);
  }
  public virtual void Finish() {
    EmitSignal(SignalName.Finished);
  }

  public Entity GetEntity() => OwnerTrait.GetEntity();

  protected void HideActionMenu() => OwnerTrait.HideActionMenu();
}

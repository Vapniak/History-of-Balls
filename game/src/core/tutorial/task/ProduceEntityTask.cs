namespace HOB;

using System;

public class ProduceEntityTask : TutorialTask, IDisposable {
  private WeakReference<EntityProductionAbility.Instance>? _weakProductionAbility;
  private bool _disposed;

  public override string Text => "Start production in City";

  public ProduceEntityTask(HOBPlayerController controller) : base(controller) {
    PlayerController.SelectedEntityChanged += OnSelectedEntityChanged;
    SubscribeToCurrentCommand();
  }

  private void SubscribeToCurrentCommand() {
    if (TryGetProductionAbility(out var oldAbility)) {
      oldAbility.Activated -= Complete;
    }

    if (PlayerController.SelectedEntity?.AbilitySystem.GetGrantedAbility<EntityProductionAbility.Instance>() is EntityProductionAbility.Instance productionAbility) {
      _weakProductionAbility = new WeakReference<EntityProductionAbility.Instance>(productionAbility);
      productionAbility.Activated += Complete;
    }
    else {
      _weakProductionAbility = null;
    }
  }

  private bool TryGetProductionAbility(out EntityProductionAbility.Instance? ability) {
    ability = null;
    return _weakProductionAbility != null &&
           _weakProductionAbility.TryGetTarget(out ability) &&
           ability != null;
  }

  private void OnSelectedEntityChanged() {
    if (_disposed) {
      return;
    }

    SubscribeToCurrentCommand();
  }

  protected override void Complete() {
    if (_disposed) {
      return;
    }

    Dispose();
    base.Complete();
  }

  public void Dispose() {
    if (_disposed) {
      return;
    }

    if (TryGetProductionAbility(out var ability) && ability != null) {
      ability.Activated -= Complete;
    }

    if (PlayerController != null) {
      PlayerController.SelectedCommandChanged -= OnSelectedEntityChanged;
    }

    _weakProductionAbility = null;
    _disposed = true;
    GC.SuppressFinalize(this);
  }

  ~ProduceEntityTask() {
    Dispose();
  }
}
namespace GameplayAbilitySystem;

using System;
using System.Threading;
using System.Threading.Tasks;
using GameplayTags;

public class WaitForGameplayEventTask {
  public event Action? Completed;
  public event Action<GameplayEventData>? EventRecieved;
  public event Action? Cancelled;

  private readonly GameplayAbility.Instance _ability;
  private readonly Tag _tag;
  private readonly CancellationTokenSource _cts;
  private readonly TaskCompletionSource _tcs;

  private Task Task => _tcs.Task;

  public WaitForGameplayEventTask(
      GameplayAbility.Instance ability,
      Tag tag,
      CancellationToken cancellationToken = default) {
    _ability = ability;
    _tag = tag;
    _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    _tcs = new TaskCompletionSource();

    _cts.Token.Register(() => {
      _tcs.TrySetCanceled(_cts.Token);
      Cancelled?.Invoke();
      UnregisterEventHandler();
    });


    RegisterEventHandler();
  }

  private void RegisterEventHandler() {
    _ability.OwnerAbilitySystem.GameplayEventRecieved += OnGameplayEventReceived;
  }

  private void UnregisterEventHandler() {
    _ability.OwnerAbilitySystem.GameplayEventRecieved -= OnGameplayEventReceived;
  }

  private void OnGameplayEventReceived(Tag tag, GameplayEventData? eventData) {
    if (tag == _tag) {
      EventRecieved?.Invoke(eventData);
    }
  }

  public void Complete() {
    _tcs.TrySetResult();
    Completed?.Invoke();
    UnregisterEventHandler();
  }
  public void Cancel() {
    _cts.Cancel();
  }
}

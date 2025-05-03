namespace HOB;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameplayAbilitySystem;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TutorialPlayerController : HOBPlayerController {
  [Export] private InitializeStatsAbility InitializeStatsPlayerState { get; set; } = default!;
  public List<TutorialTask> Tasks { get; private set; } = new();
  private Control ObjectivesParent => GetHUD().ObjectivesListParent;

  private ObjectiveWidget CurrentObjective { get; set; } = default!;

  public override void _Ready() {
    base._Ready();

    ObjectivesParent.QueueFreeAllChildren();

    GetPlayerState().AbilitySystem.GrantAbility(InitializeStatsPlayerState);
  }

  public override void OnGameStarted() {
    base.OnGameStarted();

    CurrentObjective = ObjectiveWidget.CreateWidget();
    ObjectivesParent.AddChild(CurrentObjective);

    Tasks.Add(new ProduceEntityTask(this));
    Tasks.Add(new EndTurnTask(this));
    Tasks.Add(new EndTurnTask(this, "Entity has cooldown, end turn"));
    Tasks.Add(new TutorialTask(this, "Capture enemy city"));
    _ = StartTasks();
  }


  private async Task StartTasks() {
    var enumerator = Tasks.GetEnumerator();
    while (enumerator.MoveNext()) {
      CurrentObjective.Label.Text = enumerator.Current.Text;
      enumerator.Current.Start();
      var taskCompletion = new TaskCompletionSource();
      if (!enumerator.Current.IsCompeted) {
        enumerator.Current.Completed += () => taskCompletion.TrySetResult();
        await taskCompletion.Task;
      }
    }

    CurrentObjective.QueueFree();
  }
}

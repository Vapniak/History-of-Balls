namespace HOB;

using GameplayFramework;
using GameplayTags;

public class EndTurnTask : TutorialTask {
  public override string Text => base.Text ?? "End Turn";

  public EndTurnTask(HOBPlayerController controller, string? textOverride = null) : base(controller, textOverride) {
  }

  public override void Start() {
    base.Start();

    var gm = PlayerController.GetGameMode();
    gm.GetMatchEvents().MatchEvent += OnMatchEvent;
  }

  protected override void Complete() {
    base.Complete();

    var gm = PlayerController.GetGameMode();
    gm.GetMatchEvents().MatchEvent -= OnMatchEvent;
  }

  private void OnMatchEvent(Tag tag) {
    if (tag == TagManager.GetTag(HOBTags.EventTurnEnded)) {
      Complete();
    }
  }
}
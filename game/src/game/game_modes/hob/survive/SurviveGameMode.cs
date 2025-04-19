namespace HOB;

using GameplayTags;

public partial class SurviveGameMode : HOBGameMode {
  public uint RoundsToSurvive { get; set; }
  private uint RoundsLeft { get; set; }


  public override void _EnterTree() {
    base._EnterTree();

    RoundsLeft = RoundsToSurvive;

    GetMatchEvents().MatchEvent += OnMatchEvent;
  }


  private void OnMatchEvent(Tag eventTag) {
    if (eventTag.IsExact(TagManager.GetTag(HOBTags.EventRoundStarted))) {
      if (RoundsLeft > 0) {
        RoundsLeft--;
      }

      if (RoundsLeft == 0) {
        foreach (var player in GetGameState().PlayerArray) {
          if (player.GetController() is HOBPlayerController controller) {
            EndGame(controller);
          }
        }
      }
    }
  }
}

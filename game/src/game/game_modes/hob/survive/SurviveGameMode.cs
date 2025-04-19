namespace HOB;

using System.Collections.Generic;
using System.Linq;
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

    if (eventTag.IsExact(TagManager.GetTag(HOBTags.EventTurnStarted)) || eventTag.IsExact(TagManager.GetTag(HOBTags.EventTurnEnded))) {
      var alivePlayers = new List<IMatchController>();
      var eliminatedPlayers = new List<IMatchController>();

      foreach (var player in GetGameState().PlayerArray) {
        var controller = player.GetController<IMatchController>();
        var entities = GetEntityManagment().GetOwnedEntites(controller);

        if (entities.Any(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureCity)))) {
          alivePlayers.Add(controller);
        }
        else {
          eliminatedPlayers.Add(controller);
        }
      }

      if (eliminatedPlayers.Count > 0) {
        var winner = alivePlayers.FirstOrDefault();
        if (winner != null) {
          EndGame(winner);
        }
      }
    }
  }
}

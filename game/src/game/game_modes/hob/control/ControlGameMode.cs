namespace HOB;

using GameplayTags;
using System.Collections.Generic;
using System.Linq;

public partial class ControlGameMode : HOBGameMode {

  public override void _EnterTree() {
    base._EnterTree();

    GetMatchEvents().MatchEvent += OnMatchEvent;
  }


  protected virtual IMatchController? CheckWinner() {
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
      return alivePlayers.FirstOrDefault();
    }
    else {
      return null;
    }
  }

  private void OnMatchEvent(Tag eventTag) {
    if (eventTag.IsExact(TagManager.GetTag(HOBTags.EventRoundStarted))) {
      var winner = CheckWinner();

      if (winner != null) {
        EndGame(winner);
      }
    }
  }
}

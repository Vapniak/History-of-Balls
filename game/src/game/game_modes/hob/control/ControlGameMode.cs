namespace HOB;

using GameplayFramework;
using GameplayTags;
using System.Collections.Generic;
using System.Linq;

public partial class ControlGameMode : HOBGameMode {

  public override void _EnterTree() {
    base._EnterTree();

    GetMatchEvents().MatchEvent += OnMatchEvent;
  }


  protected virtual IMatchController? CheckWinner() {
    var cities = GetEntityManagment().GetEntities().Count(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureCity)));

    var playerCities = GetEntityManagment().GetOwnedEntites(GameInstance.Instance.GetPlayerController<IMatchController>()).Count(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureCity)));

    if (playerCities == cities) {
      return GameInstance.Instance.GetPlayerController<IMatchController>();
    }

    return null;
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

namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;

[GlobalClass]
public partial class HOBPlayerState : PlayerState, IMatchPlayerState {
  public Country? Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }

  public HOBPlayerState(PlayerAttributeSet playerAttributeSet) : base() {
    AbilitySystem = new();
    AbilitySystem.AttributeSystem.AddAttributeSet(playerAttributeSet);
    AddChild(AbilitySystem);
    AbilitySystem.Owner = this;

    AbilitySystem.AttributeSystem.AttributeValueChanged += OnAttributeValueChanged;
  }

  public bool IsCurrentTurn() => GetController<IMatchController>().IsCurrentTurn();

  private void OnAttributeValueChanged(GameplayAttribute attribute, float oldValue, float newValue) {
    if (AbilitySystem.AttributeSystem.TryGetAttributeSet<PlayerAttributeSet>(out var set)) {
      if ((attribute == set.SecondaryResource)) {
        foreach (var entity in GameInstance.GetGameMode<HOBGameMode>().GetEntityManagment().GetOwnedEntites(GetController<IMatchController>())) {
          entity.AbilitySystem.SendGameplayEvent(TagManager.GetTag(HOBTags.EventResourceGenerated));
        }
      }
    }
  }
}

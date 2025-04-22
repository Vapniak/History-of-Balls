namespace HOB;


using GameplayFramework;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass]
public partial class HOBPlayerState : PlayerState, IMatchPlayerState {
  public Country Country { get; private set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }

  public Array<ProductionConfig> ProducedEntities { get; set; }
  public Array<EntityData> Entities { get; set; }

  public Theme Theme { get; private set; } = default!;

  public HOBPlayerState(PlayerAttributeSet playerAttributeSet, Array<ProductionConfig> producedEntities, Array<EntityData> entities, Country country) : base() {
    ProducedEntities = producedEntities;
    Entities = entities;
    Country = country;
    AbilitySystem = new();

    TreeEntered += () => {
      AddChild(AbilitySystem);
      AbilitySystem.Owner = this;

      AbilitySystem.AttributeSystem.AddAttributeSet(playerAttributeSet);
    };
  }

  public override void _Ready() {
    var theme = ThemeDB.GetProjectTheme().Duplicate() as HOBTheme;
    theme.PrimaryColor = Country.Color;
    theme.AccentColor = Colors.White;
    theme.GenerateTheme();

    Theme = theme;
  }

  public bool IsCurrentTurn() => GetController<IMatchController>().IsCurrentTurn();
}

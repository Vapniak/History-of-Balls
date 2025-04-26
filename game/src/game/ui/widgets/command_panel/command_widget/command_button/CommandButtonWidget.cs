namespace HOB;

using Godot;

[GlobalClass]
public partial class CommandButtonWidget : ButtonWidget {
  private HOBAbility.Instance? BoundAbility { get; set; }
  private bool _shouldShowTooltip;

  public void BindAbility(HOBAbility.Instance ability) {
    BoundAbility = ability;
    Button.Icon = ability.AbilityResource.Icon;
  }

  // TODO: tooltips
}
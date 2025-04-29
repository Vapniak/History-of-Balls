namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class CommandButtonWidget : ButtonWidget {
  private HOBAbility.Instance? BoundAbility { get; set; }
  private bool _shouldShowTooltip;

  public void BindAbility(HOBAbility.Instance ability) {
    BoundAbility = ability;
    Button.Icon = ability.AbilityResource.Icon;

    UpdateIconColor();
  }

  public override void _PhysicsProcess(double delta) {
    base._PhysicsProcess(delta);

    UpdateIconColor();
  }

  private void UpdateIconColor() {
    var pc = GameInstance.Instance?.GetPlayerController();
    if (BoundAbility == null || pc == null) {
      return;
    }

    var color = Colors.White;
    if (pc.GetPlayerState<IMatchPlayerState>()?.Theme is HOBTheme theme) {
      if (BoundAbility is AttackAbility.Instance) {
        color = theme.AttackAbilityColor;
      }
      else if (BoundAbility is MoveAbility.Instance) {
        color = theme.MoveAbilityColor;
      }


      if (!BoundAbility.CanActivateAbility(new() { Activator = pc })) {
        color = color.Darkened(0.5f);
        Button.Modulate = Button.Modulate with { A = 0.6f };
      }
      else {
        Button.Modulate = Colors.White;
      }

      Button.AddThemeColorOverride("icon_normal_color", color);
    }
  }
  // TODO: tooltips
}

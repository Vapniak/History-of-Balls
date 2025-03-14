namespace HOB;

using Godot;
using HOB.GameEntity;
using System;

public partial class EntityUI : Control {
  [Export] private TextureRect IconTextureRect { get; set; }
  [Export] private Control TeamColorContainer { get; set; }
  [Export] private Control CommandIconsContainer { get; set; }
  public override void _Ready() {
    IconTextureRect.Visible = false;
    HideCommandIcons();
  }

  public void SetTeamColor(Color color) {
    TeamColorContainer.SelfModulate = color;

    if (color.Luminance > 0.5) {
      IconTextureRect.SelfModulate = Colors.Black;
    }
    else {
      IconTextureRect.SelfModulate = Colors.White;
    }
  }
  public void SetIcon(Texture2D texture) {
    IconTextureRect.Visible = true;
    IconTextureRect.Texture = texture;
  }

  public void ShowCommandIcons(Entity entity) {
    if (entity.TryGetTrait<CommandTrait>(out var commandTrait)) {
      foreach (var child in CommandIconsContainer.GetChildren()) {
        child.Free();
      }

      foreach (var command in commandTrait.GetCommands()) {
        if (command.Data.ShowInUI || command is ProcessResourcesCommand or ProduceEntityCommand) {
          var icon = new TextureRect {
            Texture = command.Data.Icon,
            ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
            SelfModulate = command.CanBeUsed() ? Colors.Green : command.InUse ? Colors.Yellow : Colors.Red
          };

          CommandIconsContainer.AddChild(icon);
        }
      }

      if (CommandIconsContainer.GetChildCount() == 0) {
        HideCommandIcons();
      }
      else {
        CommandIconsContainer.GetParent<Control>().Show();
      }
    }
    else {
      HideCommandIcons();
    }
  }

  public void HideCommandIcons() {
    CommandIconsContainer.GetParent<Control>().Hide();
    foreach (var child in CommandIconsContainer.GetChildren()) {
      child.Free();
    }
  }
}

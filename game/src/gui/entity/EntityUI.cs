namespace HOB;

using Godot;
using HOB.GameEntity;
using System;

public partial class EntityUI : Control {
  [Export] private TextureRect IconTextureRect { get; set; }
  [Export] private Control TeamColorContainer { get; set; }
  [Export] private Control CommandIconsContainer { get; set; }
  [Export] private PanelContainer IconTextureContainer { get; set; }
  [Export] private StyleBox UnitIconPanelStyleBox { get; set; }
  [Export] private StyleBox StructureIconPanelStyleBox { get; set; }

  public override void _Ready() {
    //IconTextureRect.Visible = false;
    //HideCommandIcons();
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

  public void HideCommandIcons() {
    CommandIconsContainer.GetParent<Control>().Hide();
    foreach (var child in CommandIconsContainer.GetChildren()) {
      child.Free();
    }
  }
}

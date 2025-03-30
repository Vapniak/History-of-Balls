namespace HOB;

using GameplayTags;
using Godot;
using HOB.GameEntity;
using System;

public partial class EntityUI : Control {
  [Export] private TextureRect? IconTextureRect { get; set; }
  [Export] private Control? TeamColorContainer { get; set; }
  [Export] private Control? CommandIconsContainer { get; set; }
  [Export] private PanelContainer? IconTextureContainer { get; set; }
  [Export] private StyleBox? UnitIconPanelStyleBox { get; set; }
  [Export] private StyleBox? StructureIconPanelStyleBox { get; set; }

  private Entity? Entity { get; set; }

  public void Initialize(Entity entity) {
    Entity = entity;
  }
  public override void _Ready() {
    //IconTextureRect.Visible = false;
    //HideCommandIcons();
  }

  public void SetTeamColor(Color color) {
    if (TeamColorContainer != null) {
      TeamColorContainer.SelfModulate = color;
    }


    if (IconTextureRect != null) {
      IconTextureRect.SelfModulate = color.Luminance > 0.5 ? Colors.Black : Colors.White;
    }
  }

  public void SetIcon(Texture2D texture) {
    if (IconTextureRect != null) {
      IconTextureRect.Visible = true;
      IconTextureRect.Texture = texture;
    }

    if (IconTextureContainer != null && Entity != null) {
      if (Entity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure))) {
        IconTextureContainer?.AddThemeStyleboxOverride("panel", StructureIconPanelStyleBox);
      }
      else if (Entity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeUnit))) {
        IconTextureContainer?.AddThemeStyleboxOverride("panel", UnitIconPanelStyleBox);
      }
    }
  }

  public void HideCommandIcons() {
    if (CommandIconsContainer != null) {
      CommandIconsContainer.GetParent<Control>().Hide();
      foreach (var child in CommandIconsContainer.GetChildren()) {
        child.Free();
      }
    }
  }
}

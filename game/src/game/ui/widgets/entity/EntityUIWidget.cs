namespace HOB;

using System.Collections.Generic;
using System.Diagnostics;
using GameplayFramework;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using WidgetSystem;

[GlobalClass]
public partial class EntityUIWidget : HOBWidget, IWidgetFactory<EntityUIWidget> {
  [Signal] public delegate void UpdateViewportEventHandler();

  [Export] public Control EntriesContainer { get; private set; } = default!;
  private Entity? Entity { get; set; }

  private Dictionary<HOBAbility.Instance, TextureRect> AbilityToIcon { get; set; } = new();

  public void BindTo(Entity entity) {
    if (entity == null) {
      GD.PushError("Cannot initialize with null entity");
      return;
    }

    Entity = entity;

    //EntityNameWidget.BindTo(entity);
    var widget = EntityNameWidget.CreateWidget();
    widget.BindTo(entity);
    if (entity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure))) {

    }
    else {
      widget.NameLabel.Hide();
    }

    EntriesContainer.AddChild(widget);

    OnOwnerControllerChanged();

    Entity.OwnerControllerChanged += OnOwnerControllerChanged;

    // if (CommandIconsContainer == null) {
    //   Debug.Assert(false, "Icons container cannot be null");
    //   return;
    // }

    // foreach (var child in CommandIconsContainer.GetChildren()) {
    //   child.Free();
    // }

    // HideCommandIcons();

    // Entity.AbilitySystem.GameplayAbilityGranted += (ability) => {
    //   if (ability is HOBAbility.Instance hob) {
    //     if (!hob.AbilityResource.ShowInUI) {
    //       return;
    //     }

    //     var icon = new TextureRect() {
    //       Texture = hob.AbilityResource.Icon,
    //       ExpandMode = TextureRect.ExpandModeEnum.FitWidth,
    //     };

    //     if (AbilityToIcon.TryAdd(hob, icon)) {
    //       if (ability.CanActivateAbility(null)) {
    //         icon.SelfModulate = Colors.Green;
    //       }
    //       CommandIconsContainer.AddChild(icon);
    //       CommandIconsContainer.MoveChild(icon, hob.AbilityResource.UIOrder);

    //       ShowCommandIcons();
    //     }
    //   }
    // };

    // Entity.AbilitySystem.GameplayAbilityRevoked += (ability) => {
    //   if (ability is HOBAbility.Instance hob) {
    //     if (AbilityToIcon.TryGetValue(hob, out var icon)) {
    //       icon.Free();
    //       AbilityToIcon.Remove(hob);
    //       ShowCommandIcons();
    //     }
    //   }
    // };


    if (entity.AbilitySystem == null) {
      GD.PushError("Entity must have an AbilitySystem");
      return;
    }

    EmitSignal(SignalName.UpdateViewport);
  }

  // public override void _PhysicsProcess(double delta) {
  //   foreach (var (ability, icon) in AbilityToIcon) {
  //     icon.SelfModulate = ability.CanActivateAbility(new() { Activator = Entity.OwnerController }) ? Colors.Green : Colors.Red;
  //   }
  // }

  // private void SetTeamColor(Color color) {
  //   IconTextureRect?.SetSelfModulate(color);

  //   if (IconTextureRect != null) {
  //     //IconTextureRect.SelfModulate = color.Luminance > 0.5f ? Colors.Black : Colors.White;
  //   }
  // }

  // private void SetIcon(Texture2D? texture) {
  //   if (IconTextureRect == null) {
  //     return;
  //   }

  //   IconTextureRect.Visible = texture != null;
  //   IconTextureRect.Texture = texture;

  //   if (IconTextureContainer == null || Entity?.AbilitySystem?.OwnedTags == null) {
  //     return;
  //   }

  //   var styleBox = Entity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure))
  //       ? StructureIconPanelStyleBox
  //       : UnitIconPanelStyleBox;

  //   if (styleBox != null) {
  //     IconTextureContainer.AddThemeStyleboxOverride("panel", styleBox);
  //   }
  // }

  private void OnOwnerControllerChanged() {
    //ShowCommandIcons();


    if (Entity == null) {
      return;
    }

    if (Entity.TryGetOwner(out var owner)) {
      Theme = owner.GetPlayerState().Theme;
    }
    else {
      Theme = ThemeDB.GetProjectTheme();
    }


    EmitSignal(SignalName.UpdateViewport);
  }

  // private void ShowCommandIcons() {
  //   if (CommandIconsContainer?.GetChildCount() > 0 && Entity != null && Entity.TryGetOwner(out var owner) && owner is PlayerController) {
  //     CommandIconsParent.Show();
  //   }
  //   else {
  //     HideCommandIcons();
  //   }
  // }

  // private void HideCommandIcons() {
  //   CommandIconsParent.Hide();
  // }

  static EntityUIWidget IWidgetFactory<EntityUIWidget>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://c0wn2cfx3bg73").Instantiate<EntityUIWidget>();
  }
}

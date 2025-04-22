namespace HOB;

using Godot;
using HOB;
using HOB.GameEntity;
using System;
using WidgetSystem;

[GlobalClass]
public partial class EntityNameWidget : HOBWidget, IWidgetFactory<EntityNameWidget> {
  [Export] public Label NameLabel { get; private set; } = default!;
  [Export] public EntityIconWidget EntityIconWidget { get; private set; } = default!;

  private Entity? BoundEntity { get; set; }
  public static EntityNameWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://5dufraf2q3i").Instantiate<EntityNameWidget>();
  }

  public override void _ExitTree() {
    Unbind(BoundEntity);
  }

  public void BindTo(Entity? entity) {
    BoundEntity = entity;
    Unbind(BoundEntity);
    if (entity == null) {
      return;
    }

    NameLabel.Text = entity.EntityName;
    EntityIconWidget.SetIcon(entity.Icon);
    UpdateIconColor();

    entity.OwnerControllerChanged += UpdateIconColor;
  }

  public void Unbind(Entity? entity) {
    if (entity != null) {
      entity.OwnerControllerChanged -= UpdateIconColor;
    }
  }

  private void UpdateIconColor() {
    if (BoundEntity == null) {
      return;
    }

    if (BoundEntity.TryGetOwner(out var owner)) {
      EntityIconWidget.Modulate = owner!.GetPlayerState().Country.Color;
    }
    else {
      EntityIconWidget.Modulate = Colors.White;
    }
  }
}

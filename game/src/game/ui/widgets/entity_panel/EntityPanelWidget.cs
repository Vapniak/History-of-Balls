namespace HOB;

using GameplayAbilitySystem;
using Godot;
using HOB.GameEntity;
using System.Collections.Generic;
using System.Linq;
using WidgetSystem;

[GlobalClass]
public partial class EntityPanelWidget : HOBWidget {
  [Export] private Label NameLabel { get; set; } = default!;
  [Export] private Control? EntriesList { get; set; }
  [Export] private EntityIconWidget IconWidget { get; set; } = default!;
  private Entity? CurrentEntity { get; set; }

  public void Initialize(HOBPlayerController playerController) {
    Hide();

    playerController.SelectedEntityChanged += () => {
      var entity = playerController.SelectedEntity;

      if (entity != null) {
        BindToEntity(entity);
      }
      else {
        Unbind();
      }
    };
  }

  private void BindToEntity(Entity entity) {
    Unbind();

    IconWidget.SetIcon(entity.Icon);
    if (entity.TryGetOwner(out var owner)) {
      IconWidget.Modulate = owner!.GetPlayerState().Country.Color;
    }
    NameLabel.Text = entity.EntityName;

    CurrentEntity = entity;

    ClearEntries();
    foreach (var attribute in entity.AbilitySystem.AttributeSystem.GetAllAttributes().OrderBy(a => a.AttributeName)) {
      var widget = AttributeEntryWidget.CreateWidget();
      widget.BindTo(entity, attribute);
      EntriesList?.AddChild(widget);
    }

    Show();
  }

  private void Unbind() {
    Hide();
    ClearEntries();
    if (CurrentEntity == null) {
      return;
    }
    CurrentEntity = null;
  }

  private void ClearEntries() {
    foreach (var child in EntriesList.GetChildren()) {
      child.Free();
    }
  }
  // private void UpdateAttributeEntry(GameplayAttribute attribute) {
  //   if (CurrentEntity == null) {
  //     return;
  //   }

  //   if (CurrentEntity.AbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var set) && (attribute == set.HealthAttribute || attribute == set.MaxHealthAttribute)) {
  //     var healthValue = CurrentEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.HealthAttribute).GetValueOrDefault();
  //     var maxHealthValue = CurrentEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.MaxHealthAttribute).GetValueOrDefault();
  //     //var icon = GetIconForAttribute(set.HealthAttribute);
  //     UpdateEntry(set.HealthAttribute.AttributeName, $"{healthValue}/{maxHealthValue}", null, null);
  //   }
  //   else {
  //     var currentValue = CurrentEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(attribute).GetValueOrDefault();
  //     //var icon = GetIconForAttribute(attribute);
  //     UpdateEntry(attribute.AttributeName, currentValue.ToString(), null, null);
  //   }
  // }
}

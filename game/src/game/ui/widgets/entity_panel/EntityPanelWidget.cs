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
  private Godot.Collections.Dictionary<string, AttributeEntryWidget> Entries { get; set; } = new();

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
    NameLabel.Text = entity.EntityName;

    CurrentEntity = entity;
    CurrentEntity.AbilitySystem.AttributeSystem.AttributeValueChanged += OnAttributeValueChanged;

    ClearEntries();
    foreach (var attribute in entity.AbilitySystem.AttributeSystem.GetAllAttributes().OrderBy(a => a.AttributeName)) {
      if (attribute != null) {
        UpdateAttributeEntry(attribute);
      }
    }

    Show();
  }

  private void Unbind() {
    Hide();
    if (CurrentEntity == null) {
      return;
    }

    CurrentEntity.AbilitySystem.AttributeSystem.AttributeValueChanged -= OnAttributeValueChanged;
    CurrentEntity = null;
  }

  private void ClearEntries() {
    Entries.Clear();
    foreach (var child in EntriesList.GetChildren()) {
      child.Free();
    }
  }

  private void UpdateEntry(string name, string value, Texture2D? icon, Color? color) {
    AttributeEntryWidget entry;
    if (!Entries.TryGetValue(name, out entry!)) {
      entry = AttributeEntryWidget.CreateWidget();
      entry.SetEntryName(name + ": ");
    }

    entry.SetEntryValue(value);
    entry.SetIcon(icon, color);



    if (Entries.TryAdd(name, entry)) {
      EntriesList!.AddChild(entry);
    }
  }

  private void OnAttributeValueChanged(GameplayAttribute attribute, float oldValue, float newValue) {
    UpdateAttributeEntry(attribute);
  }

  private void UpdateAttributeEntry(GameplayAttribute attribute) {
    if (CurrentEntity == null) {
      return;
    }

    if (CurrentEntity.AbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var set) && (attribute == set.HealthAttribute || attribute == set.MaxHealthAttribute)) {
      var healthValue = CurrentEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.HealthAttribute).GetValueOrDefault();
      var maxHealthValue = CurrentEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.MaxHealthAttribute).GetValueOrDefault();
      //var icon = GetIconForAttribute(set.HealthAttribute);
      UpdateEntry(set.HealthAttribute.AttributeName, $"{healthValue}/{maxHealthValue}", null, null);
    }
    else {
      var currentValue = CurrentEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(attribute).GetValueOrDefault();
      //var icon = GetIconForAttribute(attribute);
      UpdateEntry(attribute.AttributeName, currentValue.ToString(), null, null);
    }
  }
}

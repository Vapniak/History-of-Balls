namespace HOB;

using System;
using GameplayAbilitySystem;
using Godot;
using HOB.GameEntity;
using WidgetSystem;

public partial class AttributeEntryWidget : HOBWidget, IWidgetFactory<AttributeEntryWidget> {
  [Export] private Label EntryNameLabel { get; set; } = default!;
  [Export] private Label EntryValueLabel { get; set; } = default!;
  [Export] private TextureRect IconTexture { get; set; } = default!;

  private Entity? BoundEntity { get; set; }
  private GameplayAttribute? BoundAttribute { get; set; }

  public void BindTo(Entity entity, GameplayAttribute attribute) {
    if (entity.AbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var set)) {
      if (attribute == set.MaxHealthAttribute) {
        QueueFree();
      }
    }

    BoundEntity = entity;
    BoundAttribute = attribute;

    SetEntryName(attribute.AttributeName);


    SetIcon(GameAssetsRegistry.Instance.GetIconFor(attribute));
    OnAttributeValueChanged(BoundAttribute, 0, 0);

    entity.AbilitySystem.AttributeSystem.AttributeValueChanged += OnAttributeValueChanged;
  }

  public void UnBind() {
    if (BoundEntity != null) {
      BoundEntity.AbilitySystem.AttributeSystem.AttributeValueChanged -= OnAttributeValueChanged;
    }

    BoundEntity = null;
    BoundAttribute = null;
  }

  public override void _ExitTree() {
    UnBind();
  }

  private void OnAttributeValueChanged(GameplayAttribute attribute, float oldValue, float newValue) {
    if (BoundEntity == null || attribute != BoundAttribute) {
      return;
    }

    if (BoundEntity.AbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var set) && (attribute == set.HealthAttribute || attribute == set.MaxHealthAttribute)) {
      var healthValue = BoundEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.HealthAttribute).GetValueOrDefault();
      var maxHealthValue = BoundEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.MaxHealthAttribute).GetValueOrDefault();
      SetEntryValue($"{healthValue}/{maxHealthValue}");
    }
    else {
      var currentValue = BoundEntity.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(attribute).GetValueOrDefault();
      SetEntryValue(currentValue.ToString());
    }
  }
  private void SetEntryName(string name) {
    EntryNameLabel.Text = name;
    EntryNameLabel.ResetSize();
    ResetSize();
  }

  private void SetIcon(Texture2D? icon, Color? color = null) {
    if (icon == null) {
      IconTexture.Hide();
    }
    else {
      IconTexture.Texture = icon;
      IconTexture.SelfModulate = color.GetValueOrDefault(Colors.White);
    }
  }

  private void SetEntryValue(string value) {
    EntryValueLabel.Text = value.ToString();
    EntryValueLabel.ResetSize();
    ResetSize();
  }

  public static AttributeEntryWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bqwbd74lsjxd3").Instantiate<AttributeEntryWidget>();
  }
}

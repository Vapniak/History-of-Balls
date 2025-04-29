namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class EntityIconWidget : HOBWidget, IWidgetFactory<EntityIconWidget> {
  [Export] private TextureRect IconRect { get; set; } = default!;
  public static EntityIconWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bbasg6roqlht7").Instantiate<EntityIconWidget>();
  }

  public void SetIcon(Texture2D? icon) {
    IconRect.Texture = icon;
  }
}

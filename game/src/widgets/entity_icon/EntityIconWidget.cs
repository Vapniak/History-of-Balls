namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class EntityIconWidget : Widget, IWidgetFactory<EntityIconWidget> {
  [Export] private TextureRect IconRect { get; set; } = default!;
  public static EntityIconWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("").Instantiate<EntityIconWidget>();
  }

  public void SetIcon(Texture2D icon) {
    IconRect.Texture = icon;
  }
}

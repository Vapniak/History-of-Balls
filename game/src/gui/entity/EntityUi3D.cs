namespace HOB;

using Godot;
using System;

public partial class EntityUi3D : Node {
  [Export] private TextureRect TextureRect { get; set; }
  [Export] private Control UIRoot { get; set; }
  public void SetFlag(Texture2D texture) {
    TextureRect.Texture = texture;
  }

  public void SetVisible(bool value) {
    UIRoot.Visible = value;
  }
}

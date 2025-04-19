namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class FlagWidget : HOBWidget {
  [Export] private TextureRect FlagRect { get; set; } = default!;

  public void SetFlag(Texture2D flag) {
    FlagRect.Texture = flag;
  }
}

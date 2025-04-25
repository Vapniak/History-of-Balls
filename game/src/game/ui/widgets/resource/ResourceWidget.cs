namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class ResourceWidget : HOBWidget {
  [Export] public Label ValueLabel { get; private set; } = default!;
  [Export] public TextureRect Icon { get; private set; } = default;
}

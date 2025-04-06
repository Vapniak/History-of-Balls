namespace HOB;

using Godot;
using System;

public partial class EntityList : Control {
  [Export] private ScrollContainer? ScrollContainer { get; set; }
  [Export] private Control? List { get; set; }
  public override void _Ready() {
    // List!.Resized += () => {
    //   ScrollContainer.CustomMinimumSize = new(List.Size.X, ScrollContainer.CustomMinimumSize.Y);
    // };
  }
}

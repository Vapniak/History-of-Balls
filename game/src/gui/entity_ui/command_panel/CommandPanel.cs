namespace HOB;

using Godot;
using System;

public partial class CommandPanel : Control {
  [Export] private ItemList CommandList { get; set; }

  public override void _Ready() {
    //CommandList.Clear();
  }
}

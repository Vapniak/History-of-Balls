namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class EntityTypeStats : BaseStat {
  [Export] public Texture2D Icon { get; private set; }
}

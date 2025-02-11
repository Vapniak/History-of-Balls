namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class EntityData : Resource {
  [Export] public string EntityName { get; private set; }
  [Export] public StatsContainer Stats { get; private set; }
  [Export] public PackedScene TraitsScene { get; private set; }
  [Export] public PackedScene Body { get; private set; }
}

namespace HOB;

using GameplayTags;
using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class EntityIcon : Resource {
  [Export] public Tag? EntityType { get; private set; }
  [Export] public Texture2D? Icon { get; private set; }
}

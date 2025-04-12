namespace AudioManager;

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class SoundBank : Resource {
  [Export] public string Label { get; private set; } = default!;
  [Export] public string Bus { get; private set; } = default!;
  [Export] public Array<SoundEvent> Events { get; private set; } = default!;
}

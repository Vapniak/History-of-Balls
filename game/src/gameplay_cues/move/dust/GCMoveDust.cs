namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using System;

[GlobalClass]
public partial class GCMoveDust : GameplayCue {
  [Export] private GpuParticles3D Particles { get; set; } = default!;
  public override bool HandlesEvent(GameplayCueEventType @event) => @event is GameplayCueEventType.Executed;

  public override void OnExecute(GameplayCueParameters parameters) {
    base.OnExecute(parameters);

    Particles.GlobalPosition = parameters.Position;
    Particles.Finished += QueueFree;
    Particles.Restart();
  }
}

namespace HOB;

using System;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class PropSetting : Resource {
  [Export] public PackedScene PropScene { get; private set; } = default!;
  [Export(PropertyHint.Range, "0,100")] public float Chance { get; private set; }
  [Export] public int Amount { get; set; } = 1;

  public (Mesh mesh, Transform3D transform) GetMesh() {
    var node = PropScene.InstantiateOrNull<Node3D>();
    var meshInstance = node.GetChildByType<MeshInstance3D>();
    node.QueueFree();
    return (meshInstance.Mesh, meshInstance.Transform);
  }
}
namespace HOB.Entity;

using Godot;
using System;

[GlobalClass]
public abstract partial class GameEntityTrait : Node, IGameEntityTrait<IGameTraitData> {
  private IGameTraitData _data;

  public void SetData(IGameTraitData data) => _data = data;
  public virtual IGameTraitData GetData() => _data;
}

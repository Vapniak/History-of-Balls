namespace GameEntitySystem;

using Godot;
using System;

[GlobalClass]
public abstract partial class GameEntityComponent : Node, IGameEntityComponent<IGameEntityData> {
  private IGameEntityData _data;
  public void SetData(IGameEntityData data) => _data = data;
  public virtual IGameEntityData GetData() => _data;
}

namespace HOB;

using System;
using System.Collections.Generic;
using GameplayFramework;
using HexGridMap;
using HOB.GameEntity;

public interface IMatchController : IController {
  public event Action<CubeCoord> CellSelected;
  public List<Entity> OwnedEntities { get; set; }
}

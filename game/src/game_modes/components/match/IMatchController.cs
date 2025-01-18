namespace HOB;

using System;
using System.Collections.Generic;
using GameplayFramework;
using HexGridMap;
using HOB.GameEntity;

public interface IMatchController : IController {
  public event Action<HexCell> CellSelected;
  public List<Entity> OwnedEntities { get; set; }
}

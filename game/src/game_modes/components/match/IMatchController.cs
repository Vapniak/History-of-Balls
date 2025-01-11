namespace HOB;

using System;
using System.Collections.Generic;
using GameplayFramework;
using HexGridMap;
using HOB.GameEntity;

public interface IMatchController : IController {
  event Action<HexCoordinates> CellSelected;
  List<Entity> OwnedEntities { get; set; }
}

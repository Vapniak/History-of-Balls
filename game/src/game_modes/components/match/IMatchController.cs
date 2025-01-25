namespace HOB;

using System;
using GameplayFramework;
using HexGridMap;

public interface IMatchController : IController {
  public event Action<CubeCoord> CoordClicked;
}

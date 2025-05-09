namespace HOB;

using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class UnitBody : EntityBody {
  [Export] public UnitAttribute? UnitAttribute { get; private set; }
}

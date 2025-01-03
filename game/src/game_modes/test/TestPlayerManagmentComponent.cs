namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass]
public partial class TestPlayerManagmentComponent : PlayerManagmentComponent {
  [Export] private Array<EntityDefinition> PlayerEntities { get; set; }

  protected override PlayerState CreatePlayerState() {
    var state = new TestPlayerState {
      Entities = PlayerEntities
    };

    return state;
  }
}

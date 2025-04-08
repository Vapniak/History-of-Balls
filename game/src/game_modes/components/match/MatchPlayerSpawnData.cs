namespace HOB;

using GameplayTags;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass, Tool]
public partial class MatchPlayerSpawnData : Resource {
  [Export] public PlayerType PlayerType { get; private set; }
  [Export] public AIProfile? AIProfile { get; private set; }
  [Export] public Country? Country { get; private set; }
  [Export] public int PrimaryResourceInitialValue { get; private set; }
  [Export] public int SecondaryResourceInitialValue { get; private set; }
  [Export] public Array<ProductionConfig>? ProducableEntities { get; private set; }
  [Export] public Array<EntitySpawnData>? Entities { get; private set; }


  // public override Array<Dictionary> _GetPropertyList() {
  //   var properties = new Array<Dictionary>();

  //   foreach (var tag in TagManager.GetTag(HOBTags.EntityTypeStructure).GetChildren()) {
  //     var property = new Dictionary
  //     {
  //               { "name", $"Structures/{tag.Name}" },
  //               { "type", (int)Variant.Type.Object },
  //               { "usage", (int)PropertyUsageFlags.Default },
  //               { "hint", (int)PropertyHint.ResourceType },
  //               { "hint_string", $"{nameof(EntityData)}" }
  //           };
  //     properties.Add(property);
  //   }

  //   return properties;
  // }
}

public enum PlayerType {
  Player,
  AI,
  None
}

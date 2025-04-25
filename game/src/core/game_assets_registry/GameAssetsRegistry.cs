namespace HOB;

using System;
using System.Drawing;
using System.Linq;
using GameplayAbilitySystem;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

// This should not be global node but exist on player controller
public partial class GameAssetsRegistry : Node {
  public static GameAssetsRegistry Instance { get; private set; } = default!;

  [Export] public Array<AttributeIcon> AttributeIcons { get; private set; } = new();
  [Export] public Array<EntityIcon> EntityIcons { get; private set; } = new();

  [ExportGroup("Cursors")]
  [Export] public Texture2D? DefaultCursor { get; private set; }
  [Export] public Texture2D? PointingHandCursor { get; private set; }

  public override void _EnterTree() {
    Instance = this;

    Input.SetCustomMouseCursor(DefaultCursor);
    Input.SetCustomMouseCursor(PointingHandCursor, Input.CursorShape.PointingHand);
  }

  public Texture2D? GetIconFor(GameplayAttribute attribute) {
    return AttributeIcons.FirstOrDefault(a => a.Attribute == attribute)?.Icon;
  }

  public Texture2D? GetIconFor(EntityData entity) {
    return EntityIcons.FirstOrDefault(e => entity.Tags.HasTag(e.EntityType))?.Icon;
  }

  public Texture2D? GetIconFor(Entity entity) {
    return EntityIcons.FirstOrDefault(e => entity.AbilitySystem.OwnedTags.HasTag(e.EntityType))?.Icon;
  }
}

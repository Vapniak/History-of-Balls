namespace Tooltip;

using Godot;

public interface ITooltipTrigger {
  public string Text { get; }
  public Vector2 Position { get; }
}
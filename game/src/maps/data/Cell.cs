namespace HOB;

using Godot;

[GlobalClass, Tool]
public partial class Cell : RefCounted {
  [Export] public int Col;
  [Export] public int Row;
  [Export] public int Id;
  [Export] public int ObjectId;
}

namespace GameplayTags;

using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class Tag : Resource {
  public static readonly Tag Empty = new(string.Empty);

  public string Name { get; } = string.Empty;
  public string FullName { get; } = string.Empty;
  public Tag? Parent { get; set; }
  private readonly HashSet<Tag> _children = new();

  public Tag() { }

  public Tag(string fullName) {
    FullName = fullName;
    var parts = fullName.Split('.');
    Name = parts[^1];

    Parent = parts.Length > 1 ? new Tag(string.Join(".", parts.Take(parts.Length - 1))) : Empty;

    _children = new HashSet<Tag>();
  }

  public bool Matches(Tag other) {
    return this == other || IsDescendantOf(other);
  }

  public bool IsDescendantOf(Tag parent) {
    var current = this;
    while (current != Empty) {
      if (current?.Parent == parent) {
        return true;
      }

      current = current?.Parent;
    }
    return false;
  }

  public void AddChild(Tag child) {
    _children.Add(child);
  }

  public IEnumerable<Tag> GetChildren() {
    return _children;
  }
}
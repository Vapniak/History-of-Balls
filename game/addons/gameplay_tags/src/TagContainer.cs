namespace GameplayTags;

using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class TagContainer : Node {
  [Export] public Tag MyTag;

  private readonly HashSet<Tag> _tags = new();

  public void AddTag(Tag tag) {
    _tags.Add(tag);
  }

  public void RemoveTag(Tag tag) {
    _tags.Remove(tag);
  }

  public bool HasTag(Tag tag) {
    return _tags.Any(t => t.Matches(tag));
  }

  public bool HasExactTag(Tag tag) {
    return _tags.Contains(tag);
  }

  public IEnumerable<Tag> GetTags() {
    return _tags;
  }
}
namespace GameplayTags;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TagContainer : Resource {
  [Export] private Array<Tag> Tags { get; set; } = new();

  public TagContainer() { }
  public TagContainer(params Tag[] tags) {
    AddTags(new(tags));
  }

  public void AddTags(TagContainer tags) {
    foreach (var tag in tags.GetTags()) {
      AddTag(tag);
    }
  }

  public void AddTag(Tag tag) {
    if (!Tags.Contains(tag)) {
      Tags.Add(tag);
    }
  }

  public void RemoveTag(Tag tag) {
    Tags.Remove(tag);
  }

  public void RemoveTags(TagContainer tags) {
    foreach (var tag in tags.GetTags()) {
      RemoveTag(tag);
    }
  }

  public bool HasTag(Tag tag) {
    return Tags.Any(t => t.Matches(tag));
  }

  public bool HasAllTags(TagContainer tags) {
    return tags.GetTags().Intersect(GetTags()).Any();
  }

  public bool HasExactTag(Tag tag) {
    return Tags.Contains(tag);
  }

  public IEnumerable<Tag> GetTags() {
    return Tags;
  }
}
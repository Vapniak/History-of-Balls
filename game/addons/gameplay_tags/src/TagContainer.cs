namespace GameplayTags;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class TagContainer : Resource {
  [Signal] public delegate void TagAddedEventHandler(Tag tag);
  [Signal] public delegate void TagRemovedEventHandler(Tag tag);

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
      EmitSignal(SignalName.TagAdded, tag);
    }
  }

  public void RemoveTag(Tag tag) {
    if (Tags.Remove(tag)) {
      EmitSignal(SignalName.TagRemoved, tag);
    }
  }

  public void RemoveTags(TagContainer tags) {
    foreach (var tag in tags.GetTags()) {
      RemoveTag(tag);
    }
  }

  public bool HasTag(Tag tag) {
    return Tags.Any(t => t == tag);
  }

  public bool HasAnyOfTags(TagContainer tags) {
    return tags.GetTags().Intersect(GetTags()).Any();
  }
  public bool HasAllTags(TagContainer tags) {
    return tags.GetTags().Intersect(GetTags()).Count() == tags.GetTags().Count();
  }

  public bool HasExactTag(Tag tag) {
    return Tags.Any(t => t.IsExact(tag));
  }

  public IEnumerable<Tag> GetAllTags() {
    var tags = new List<Tag>();

    foreach (var tag in GetTags()) {
      tags.Add(tag);
      tags.AddRange(tag.GetAllChildren());
    }

    return tags;
  }

  public IEnumerable<Tag> GetTags() {
    return Tags;
  }
}
namespace HOB.GameEntity;

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiresTraitAttribute<T> : Attribute where T : Trait { }

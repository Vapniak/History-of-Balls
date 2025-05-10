namespace Console;

using System.Reflection;

public class ConsoleCommandReference {
  public required string Command { get; init; }
  public required string Description { get; init; }
  public required string Usage { get; init; }
  public required MethodInfo Method { get; init; }
}

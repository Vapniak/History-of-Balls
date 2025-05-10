namespace Console;

using System.Linq;
using Godot;

public class BuiltinCommands {
  [ConsoleCommand("timescale", Description = "Sets the timescale", Usage = "timescale 1.5")]
  public void DebugSetTimescale(float timescale) {
    if (timescale == 0) {
      timescale = 1;
    }
    Engine.TimeScale = timescale;
    Console.Instance.Print($"Set timescale to {timescale}");
  }

  [ConsoleCommand("help", Description = "Shows this help", Usage = "help [command?]")]
  public void PrintHelp(string? command = null) {
    if (command == null) {
      Console.Instance.Print("Commands:", Console.PrintType.Hint);

      Console.Instance.Commands.ForEach(commandItem => Console.Instance.Print($"{commandItem.Command} - {commandItem.Description}"));
    }
    else {
      var commandAttribute = Console.Instance.Commands.FirstOrDefault(x => x.Command == command);
      if (commandAttribute == null) {
        Console.Instance.Print($"The command '{command}' does not exist.", Console.PrintType.Error);
        return;
      }

      Console.Instance.Print($"{commandAttribute.Command}");
      Console.Instance.Print($"{commandAttribute.Description}", Console.PrintType.Hint);
      Console.Instance.Print($"Usage: {commandAttribute.Usage}");
    }
  }

  [ConsoleCommand("info", Description = "Prints general information")]
  private void DebugPrintInfo() {
    Console.Instance.Print("Versioning:");
    Console.Instance.Print($"Godot Version: {Engine.GetVersionInfo()["string"]}", Console.PrintType.Hint);
    Console.Instance.Print($"CPU Architecture: {Engine.GetArchitectureName()}", Console.PrintType.Hint);
    Console.Instance.Space();
    Console.Instance.Print("Operating System:");
    Console.Instance.Print($"Host: {OS.GetName()}", Console.PrintType.Hint);
    Console.Instance.Print($"Version: {OS.GetVersion()}", Console.PrintType.Hint);
    Console.Instance.Print($"Language: {OS.GetLocaleLanguage()}", Console.PrintType.Hint);
    Console.Instance.Print($"Locale: {OS.GetLocale()}", Console.PrintType.Hint);
    Console.Instance.Space();
    Console.Instance.Print("Hardware:");
    Console.Instance.Print($"Processor Name: {OS.GetProcessorName()}", Console.PrintType.Hint);
    Console.Instance.Print($"Processor Cores: {OS.GetProcessorCount()}", Console.PrintType.Hint);
    Console.Instance.Print($"Video Driver Name: {OS.GetVideoAdapterDriverInfo()[0]}", Console.PrintType.Hint);
    Console.Instance.Print($"Video Driver Version: {OS.GetVideoAdapterDriverInfo()[1]}", Console.PrintType.Hint);
    Console.Instance.Print($"Memory (physical): {OS.GetMemoryInfo()["physical"].AsInt64() / 1024 / 1024}MB", Console.PrintType.Hint);
    Console.Instance.Print($"Memory (available): {OS.GetMemoryInfo()["available"].AsInt64() / 1024 / 1024}MB", Console.PrintType.Hint);
    Console.Instance.Print($"Memory (free): {OS.GetMemoryInfo()["free"].AsInt64() / 1024 / 1024}MB", Console.PrintType.Hint);
  }

  [ConsoleCommand("clear", Description = "Clears the console history")]
  public void ClearConsole() {
    Console.Instance.ClearConsole();
  }
}

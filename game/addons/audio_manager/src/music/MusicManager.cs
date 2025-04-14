namespace AudioManager;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class MusicManager : Node {
  [Signal]
  public delegate void BanksUpdatedEventHandler();

  [Export] private Array<MusicBank> MusicBanks { get; set; } = new();


  public static MusicManager Instance { get; private set; } = default!;
  private Godot.Collections.Dictionary<string, MusicBank> _musicTable = new();
  private int _musicTableHash;
  private readonly List<StemmedMusicStreamPlayer> _musicStreams = new();
  private float _volume = 1.0f;

  public override void _Ready() {
    Instance = this;

    foreach (var b in MusicBanks) {
      AddBank(b);
    }
  }

  public override void _Process(double delta) {
    if (_musicTableHash != _musicTable.GetHashCode()) {
      _musicTableHash = _musicTable.GetHashCode();
      EmitSignal(SignalName.BanksUpdated);
    }
  }

  public bool Play(string bankLabel, string trackName, float crossfadeTime = 5.0f, bool autoLoop = false) {
    if (!_musicTable.TryGetValue(bankLabel, out var bank)) {
      GD.PushError($"Tried to play the music track [{trackName}] from an unknown bank [{bankLabel}].");
      return false;
    }

    var track = bank.Tracks.FirstOrDefault(t => t.Name == trackName);
    if (track == null) {
      GD.PushError($"Tried to play an unknown music track [{trackName}] from the bank [{bankLabel}].");
      return false;
    }

    if (track.Stems.Count == 0) {
      GD.PushError($"- The music track [{trackName}] on bank [{bankLabel}] has no stems.");
      return false;
    }

    foreach (var stem in track.Stems) {
      if (stem.Stream == null) {
        GD.PushError($"- The stem [{stem.Name}] on the music track [{trackName}] on bank [{bankLabel}] is missing an audio stream.");
        return false;
      }
    }

    var bus = GetBus(bank.Bus, track.Bus);
    var player = StemmedMusicStreamPlayer.Create(bankLabel, trackName, bus, _volume, autoLoop);

    if (_musicStreams.Count > 0) {
      Stop(crossfadeTime);
    }

    _musicStreams.Add(player);
    AddChild(player);
    player.StartStems(track.Stems, crossfadeTime);
    player.Stopped += () => OnPlayerStopped(player);

    if (autoLoop) {
      player.AutoLoopCompleted += OnAutoLoopCompleted;
    }

    return true;
  }

  public void Stop(float fadeOutTime) {
    foreach (var stream in _musicStreams) {
      stream.StopStems(fadeOutTime);
    }
  }

  public void Stop(string trackName, float fadeOutTime) {
    _musicStreams.First(m => m.TrackName == trackName).StopStems(fadeOutTime);
  }

  private void AddBank(MusicBank bank) {
    if (_musicTable.TryGetValue(bank.Label, out var value)) {
      return;
    }

    _musicTable[bank.Label] = bank;
  }

  private static string GetBus(string bankBus, string trackBus) {
    if (!string.IsNullOrEmpty(trackBus)) {
      return trackBus;
    }

    if (!string.IsNullOrEmpty(bankBus)) {
      return bankBus;
    }

    Debug.Assert(false, "Bus is null");
    return "";
  }

  private void OnPlayerStopped(StemmedMusicStreamPlayer player) {
    _musicStreams.Remove(player);
    player.QueueFree();
  }

  private void OnAutoLoopCompleted(string bankLabel, string trackName, float crossfadeTime) {
    Play(bankLabel, trackName, crossfadeTime, true);
  }
}
namespace AudioManager;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class StemmedMusicStreamPlayer : Node {
  [Signal]
  public delegate void StoppedEventHandler();

  [Signal]
  public delegate void AutoLoopCompletedEventHandler(string bankLabel, string trackName, float crossfadeTime);

  public bool IsStopping { get; private set; }
  public string BankLabel { get; private set; } = "";
  public string TrackName { get; private set; } = "";

  private string Bus { get; set; } = default!;
  private float Volume { get; set; } = 1.0f;
  private bool AutoLoop { get; set; }

  private Godot.Collections.Dictionary<string, StemPlayer> _stems = new();

  public static StemmedMusicStreamPlayer Create(string bankLabel, string trackName, string bus, float volume, bool autoLoop) {
    var player = new StemmedMusicStreamPlayer {
      BankLabel = bankLabel,
      TrackName = trackName,
      Bus = bus,
      Volume = volume,
      AutoLoop = autoLoop
    };

    return player;
  }

  public void StartStems(IEnumerable<MusicStem> stems, float fadeInTime) {
    foreach (var stem in stems) {
      var stemPlayer = new StemPlayer(stem);
      AddChild(stemPlayer);

      stemPlayer.Bus = Bus;
      stemPlayer.Play(stem.Enabled ? fadeInTime : 0);

      _stems[stem.Name] = stemPlayer;
    }

    if (AutoLoop) {
      var longestStem = GetLongestStem();
      if (longestStem != null) {
        CallDeferred(nameof(ScheduleAutoLoop), longestStem);
      }
    }
  }

  public void StopStems(float fadeOutTime) {
    IsStopping = true;

    foreach (var stem in _stems.Values) {
      stem.Stop(fadeOutTime);
    }

    GetTree().CreateTimer(fadeOutTime).Timeout += () => EmitSignal(SignalName.Stopped);
  }

  public void SetVolume(float volume) {
    Volume = volume;

    foreach (var stem in _stems.Values) {
      stem.SetVolume(volume);
    }
  }

  public void ToggleStem(string name, bool enabled, float fadeTime) {
    if (!_stems.TryGetValue(name, out var stem)) {
      GD.PushWarning($"Cannot toggle the stem [{name}] as it doesn't exist.");
      return;
    }

    stem.SetEnabled(enabled, fadeTime);
  }

  public void SetStemVolume(string name, float volume) {
    if (!_stems.TryGetValue(name, out var stem)) {
      GD.PushWarning($"Cannot set the volume of stem [{name}] as it doesn't exist.");
      return;
    }

    stem.SetVolume(volume);
  }

  public IEnumerable<StemPlayer> GetStem(string name) {
    return _stems.Values;
  }

  private StemPlayer? GetLongestStem() {
    if (_stems.Count == 0) {
      return null;
    }

    StemPlayer? longestStem = null;
    float longestDuration = 0;

    foreach (var stem in _stems.Values) {
      var duration = stem.StreamLength;
      if (duration > longestDuration) {
        longestDuration = duration;
        longestStem = stem;
      }
    }

    return longestStem;
  }

  private void ScheduleAutoLoop(StemPlayer stem) {
    var streamLength = stem.StreamLength;

    if (streamLength <= 0) {
      return;
    }

    var crossfadeTime = 5.0f;
    var timeToLoop = streamLength - crossfadeTime;

    if (timeToLoop <= 0) {
      timeToLoop = 0.1f;
    }

    var timer = GetTree().CreateTimer(timeToLoop);
    timer.Timeout += () => {
      if (!IsStopping) {
        EmitSignal(SignalName.AutoLoopCompleted, BankLabel, TrackName, crossfadeTime);
      }
    };
  }
}

public partial class StemPlayer : AudioStreamPlayer {
  public string StemName { get; private set; }
  public bool IsEnabled { get; private set; }
  public float Volume { get; private set; }
  public float StreamLength {
    get {
      if (Stream == null) {
        return 0;
      }

      return (float)Stream.GetLength();
    }
  }

  private Tween? _tween;

  public StemPlayer(MusicStem stem) {
    StemName = stem.Name;
    IsEnabled = stem.Enabled;
    Stream = stem.Stream;
    Volume = stem.Volume;
    VolumeDb = -80.0f;
  }

  public new void Play(float fadeInTime) {
    Play();

    if (IsEnabled) {
      CreateVolumeTransition(-80.0f, Volume, fadeInTime);
    }
  }

  public void Stop(float fadeOutTime) {
    if (fadeOutTime > 0) {
      CreateVolumeTransition(VolumeDb, -80.0f, fadeOutTime);
      GetTree().CreateTimer(fadeOutTime).Timeout += Stop;
    }
    else {
      Stop();
    }
  }

  public void SetEnabled(bool pEnabled, float pFadeTime) {
    IsEnabled = pEnabled;

    if (pEnabled) {
      CreateVolumeTransition(VolumeDb, Volume, pFadeTime);
    }
    else {
      CreateVolumeTransition(VolumeDb, -80.0f, pFadeTime);
    }
  }

  public void SetVolume(float pVolume) {
    Volume = pVolume;

    if (IsEnabled) {
      VolumeDb = Mathf.LinearToDb(pVolume);
    }
  }

  private void CreateVolumeTransition(float from, float to, float duration) {
    if (duration <= 0) {
      VolumeDb = to;
      return;
    }

    if (_tween != null && _tween.IsValid()) {
      _tween.Kill();
    }

    _tween = CreateTween();
    _tween.TweenProperty(this, "volume_db", to, duration);
  }
}
namespace AudioManager;

using Godot;

public partial class PooledAudioStreamPlayer : PooledAudioStreamPlayerBase {
  private AudioStreamPlayer _player = default!;

  public static PooledAudioStreamPlayer Create() {
    var pooled = new PooledAudioStreamPlayer {
      _player = new AudioStreamPlayer()
    };
    pooled.AddChild(pooled._player);
    pooled._player.Finished += pooled.OnFinished;
    return pooled;
  }

  public override void Configure(System.Collections.Generic.List<AudioStream> pStreams, bool pReserved,
                                string pBus, bool pPolyphonic, float pVolumeDb, float pPitchScale, int pMode) {
    base.Configure(pStreams, pReserved, pBus, pPolyphonic, pVolumeDb, pPitchScale, pMode);

    _streams = pStreams;
    _player.Bus = _bus;
    _player.VolumeDb = _volume_db;
    _player.PitchScale = _pitch_scale;
  }

  public override void Trigger() {
    if (_streams == null || _streams.Count == 0) {
      GD.Print("null");
      return;
    }

    if (_polyphonic && Playing) {
      Stop();
    }

    GD.Print("trigerr");
    _player.Stream = GetStream();
    _player.Play();
    Playing = true;
  }

  public override void TriggerVaried(float pPitchVariation = 1.0f, float pVolumeVariation = 0.0f) {
    if (_streams == null || _streams.Count == 0) {
      return;
    }

    if (_polyphonic && Playing) {
      Stop();
    }

    _player.Stream = GetStream();
    _player.PitchScale = _pitch_scale * pPitchVariation;
    _player.VolumeDb = _volume_db + pVolumeVariation;
    _player.Play();
    Playing = true;
  }

  public override void Stop() {
    if (!Playing) {
      return;
    }

    _player.Stop();
    Playing = false;
  }
}
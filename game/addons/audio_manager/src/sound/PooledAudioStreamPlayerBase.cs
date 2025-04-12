namespace AudioManager;

using System;
using System.Collections.Generic;
using Godot;


public abstract partial class PooledAudioStreamPlayerBase : Node {
  [Signal]
  public delegate void ReleasedEventHandler();

  [Signal]
  public delegate void FinishedEventHandler();

  public bool Playing { get; protected set; }
  public bool Reserved { get; protected set; }
  public bool Releasing { get; protected set; }

  protected List<AudioStream> _streams = new();
  protected string _bus = "";
  protected bool _polyphonic;
  protected float _volume_db;
  protected float _pitch_scale = 1.0f;
  protected int _mode;


  public virtual void Configure(List<AudioStream> pStreams, bool pReserved, string pBus,
                               bool pPolyphonic, float pVolumeDb, float pPitchScale, int pMode) {
    _streams = pStreams;
    Reserved = pReserved;
    _bus = pBus;
    _polyphonic = pPolyphonic;
    _volume_db = pVolumeDb;
    _pitch_scale = pPitchScale;
    _mode = pMode;
  }

  public virtual void AttachTo(object pAttachment) {

  }

  public virtual void Trigger() {

  }

  public virtual void TriggerVaried(float pPitchVariation = 1.0f, float pVolumeVariation = 0.0f) {

  }

  public virtual void Stop() {

  }

  // Release this player back to the pool
  public virtual void Release(bool pFinishPlaying = false) {
    if (Releasing) {
      return;
    }

    Releasing = true;

    if (!pFinishPlaying || !Playing) {
      Stop();
      OnReleaseComplete();
    }
  }

  protected virtual void OnReleaseComplete() {
    Releasing = false;
    Reserved = false;
    EmitSignal(SignalName.Released);
  }

  protected virtual void OnFinished() {
    Playing = false;
    EmitSignal(SignalName.Finished);

    if (Releasing) {
      OnReleaseComplete();
    }
  }

  protected virtual AudioStream? GetStream() {
    if (_streams == null || _streams.Count == 0) {
      return null;
    }

    if (_streams.Count == 1) {
      return _streams[0];
    }

    return _streams[new Random().Next(_streams.Count)];
  }
}
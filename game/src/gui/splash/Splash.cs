namespace HOB;

using System.Runtime.CompilerServices;
using GameplayFramework;
using Godot;

public partial class Splash : Control {
  [Export] private VideoStreamPlayer _videoStreamPlayer;
  [Export] private AudioStreamPlayer _audioStreamPlayer;

  [Export] private VideoStream _firstVideo;
  [Export] private VideoStream _secondVideo;

  public override void _Ready() {
    _videoStreamPlayer.Finished += () => {
      if (_videoStreamPlayer.Stream == _firstVideo) {
        _videoStreamPlayer.Stream = _secondVideo;
        _videoStreamPlayer.Play();
      }
      else {
        // FIXME: video disappears when finished
        GoToMainMenu();
      }
    };

    PlayIntro();
  }

  // TODO: minigame to skip video
  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed) {
      GoToMainMenu();
    }
  }
  public void PlayIntro() {
    _videoStreamPlayer.Stream = _firstVideo;

    _videoStreamPlayer.Play();
    _audioStreamPlayer.Play();
  }
  private void GoToMainMenu() {
    _videoStreamPlayer.Paused = true;
    _audioStreamPlayer.StreamPaused = true;
    GameInstance.GetWorld().OpenLevel("main_menu_level");
  }
}

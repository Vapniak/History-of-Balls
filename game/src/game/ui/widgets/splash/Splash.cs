namespace HOB;

using System.Runtime.CompilerServices;
using AudioManager;
using GameplayFramework;
using Godot;

public partial class Splash : Control {
  [Export] private VideoStreamPlayer _videoStreamPlayer;

  [Export] private VideoStream _firstVideo;
  [Export] private VideoStream _secondVideo;

  private bool _canSkip;

  public override void _Ready() {
    _videoStreamPlayer.Finished += () => {
      if (_videoStreamPlayer.Stream == _firstVideo) {
        MusicManager.Instance.Play("music", "intro", 1);
        _videoStreamPlayer.Stream = _secondVideo;
        _canSkip = true;
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
#if DEBUG
    _canSkip = true;
#endif
    if (_canSkip && @event is InputEventMouseButton mouseButton && mouseButton.Pressed) {
      GoToMainMenu();
    }
  }
  public void PlayIntro() {
    _videoStreamPlayer.Stream = _firstVideo;

    MusicManager.Instance.Play("music", "splash", 1);
    _videoStreamPlayer.Play();
  }
  private void GoToMainMenu() {
    _videoStreamPlayer.Paused = true;
    MusicManager.Instance.Stop(1);
    _ = GameInstance.GetWorld().OpenLevel("main_menu_level");
  }
}

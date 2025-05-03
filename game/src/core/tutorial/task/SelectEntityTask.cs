namespace HOB;

using System.Linq;
using Godot;
using HOB.GameEntity;

public partial class SelectEntityTask : TutorialTask {
  public override string Text => "Select " + EntityToSelect.EntityName;

  private Entity EntityToSelect { get; set; }
  public SelectEntityTask(HOBPlayerController playerController, Entity entity) : base(playerController) {
    playerController.SelectedEntityChanged += OnSelectedEntityChanged;
    EntityToSelect = entity;
  }

  private void OnSelectedEntityChanged() {
    if (PlayerController.SelectedEntity == EntityToSelect) {
      Complete();
    }
  }

  protected override void Complete() {
    base.Complete();

    PlayerController.SelectedCommandChanged -= OnSelectedEntityChanged;
  }
}

namespace HOB;

using Godot;
using Godot.Collections;
using WidgetSystem;

[GlobalClass]
public partial class SelectCampaignWidget : HOBWidget, IWidgetFactory<SelectCampaignWidget> {
  [Export] private Array<Campaign> Campaigns { get; set; } = default!;
  [Export] private Control CampaignsParent { get; set; } = default!;
  public static SelectCampaignWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bd8hg7vew7ukx").Instantiate<SelectCampaignWidget>();
  }

  public override void _Ready() {
    base._Ready();

    foreach (var child in CampaignsParent.GetChildren()) {
      child.Free();
    }

    foreach (var campaign in Campaigns) {
      var widget = CampaignWidget.CreateWidget();
      widget.BindTo(campaign);

      widget.Button.Pressed += () => {
        var selectMission = SelectMissionWidget.CreateWidget();
        selectMission.BindTo(campaign.Missions);
        WidgetManager.PushWidget(selectMission);
      };

      CampaignsParent.AddChild(widget);

    }
  }
}

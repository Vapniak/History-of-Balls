namespace HOB;

using System.Threading.Tasks;
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

  // static async Task<SelectCampaignWidget> IAsyncWidgetFactory<SelectCampaignWidget>.CreateWidget() {
  //   return await LoadWidget();
  // }

  // private static async Task<SelectCampaignWidget> LoadWidget() {
  //   var loader = ResourceLoader.LoadThreadedRequest("uid://bd8hg7vew7ukx");
  //   while (ResourceLoader.LoadThreadedGetStatus("uid://bd8hg7vew7ukx") == ResourceLoader.ThreadLoadStatus.InProgress) {
  //     await Task.Delay(1);
  //   }

  //   return (ResourceLoader.LoadThreadedGet("uid://bd8hg7vew7ukx") as PackedScene).Instantiate<SelectCampaignWidget>();
  // }

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

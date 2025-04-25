namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class CampaignTraitWidget : HOBWidget, IWidgetFactory<CampaignTraitWidget> {
  [Export] private Label NameLabel { get; set; } = default!;
  [Export] private ProgressBar ProgressBar { get; set; } = default!;
  public static CampaignTraitWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://dtew8dh45vhlj").Instantiate<CampaignTraitWidget>();
  }

  public void BindTo(CampaignTrait trait) {
    NameLabel.Text = trait.Name;
    ProgressBar.Value = trait.Percent;
  }
}

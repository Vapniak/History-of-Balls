namespace HOB;

using Godot;
using WidgetSystem;

public partial class AttributeEntryWidget : HOBWidget, IWidgetFactory<AttributeEntryWidget> {
  [Export] private Label EntryNameLabel { get; set; } = default!;
  [Export] private Label EntryValueLabel { get; set; } = default!;
  [Export] private TextureRect IconTexture { get; set; } = default!;

  public void SetEntryName(string name) {
    EntryNameLabel.Text = name;
    EntryNameLabel.ResetSize();
    ResetSize();
  }

  public void SetIcon(Texture2D? icon, Color? color = null) {
    if (icon == null) {
      IconTexture.Hide();
    }
    else {
      IconTexture.Texture = icon;
      IconTexture.SelfModulate = color.GetValueOrDefault(Colors.White);
    }
  }

  public void SetEntryValue(string value) {
    EntryValueLabel.Text = value;
    EntryValueLabel.ResetSize();
    ResetSize();
  }

  public static AttributeEntryWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bqwbd74lsjxd3").Instantiate<AttributeEntryWidget>();
  }
}

namespace HOB;

using Godot;

[GlobalClass, Tool]
public partial class HOBTheme : ProgrammaticTheme {
  [Export] public Color FontColor { get; private set; } = Colors.White;
  [Export] public Color BaseColor { get; private set; }
  [Export] public Color PrimaryColor { get; private set; }
  [Export] public Color AccentColor { get; private set; }

  [Export] public int BaseSpacing { get; private set; }
  [Export] public int BaseCornerRadius { get; private set; }
  [Export] public int BaseBorderWidth { get; private set; }

  public Color PrimaryTransparent => new(PrimaryColor, .3f);
  public int BaseMargin => BaseSpacing;

  public override void DefineTheme() {
    // var baseButton = new Style {
    //   ["font_color"] = Colors.White,
    //   ["hover"] = StyleboxFlat(new Style {
    //     ["bg_color"] = new Color(0.2f, 0.2f, 0.2f),
    //     ["border_width"] = BorderWidth(2),
    //     ["border_color"] = Colors.Gray
    //   })
    // };

    // DefineStyle("Button", baseButton);

    // // Primary button variant
    // var primaryButton = Inherit(baseButton, new Style {
    //   ["normal"] = StyleboxFlat(new Style {
    //     ["bg_color"] = new Color(0.1f, 0.3f, 0.6f),
    //     ["border_width"] = BorderWidth(2),
    //     ["border_color"] = new Color(0.2f, 0.4f, 0.8f)
    //   })
    // });

    // DefineVariantStyle("PrimaryButton", "Button", primaryButton);

    var baseStyleBox = StyleboxFlat(new() {
      ["bg_color"] = BaseColor,
      ["content_margins_"] = ContentMargins(BaseMargin),
      ["corner_radius_"] = CornerRadius(BaseCornerRadius),
      // ["border_color"] = PrimaryTransparent,
      // ["border_width_"] = BorderWidth(BaseBorderWidth),
    });

    DefineStyle("Panel", new Style() {
      ["panel"] = baseStyleBox,
    });

    DefineVariantStyle("TopBarPanel", "Panel", new() {
      ["panel"] = Inherit(baseStyleBox, StyleboxFlat(new() {
        ["corner_radius_"] = CornerRadius(0, 0, BaseCornerRadius, BaseCornerRadius)
      }))
    });


    var sizes = new[] { "Small", "Medium", "Large" };

    for (var i = 1; i <= sizes.Length; i++) {
      var size = sizes[i - 1];
      DefineVariantStyle("Margin" + size, "MarginContainer", new() {
        ["margins_"] = Margin(BaseMargin * i)
      });

      DefineVariantStyle("Header" + size, "Label", new() {
        ["font_size"] = DefaultFontSize + 8 * i,
      });

      DefineVariantStyle("Spacing" + size, "BoxContainer", new Style() {
        ["separation"] = BaseSpacing * i,
      });
    }

    DefineStyle("BoxContainer", new Style() {
      ["separation"] = BaseSpacing,
    });


    var baseButtonStyle = Inherit(baseStyleBox, StyleboxFlat(new() {
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_color"] = PrimaryColor,
      ["content_margins_"] = ContentMargins(BaseMargin, BaseMargin / 2, BaseMargin, BaseMargin / 2),
    }));

    var buttonStyle = new Style() {
      ["normal"] = baseButtonStyle,
      ["hover"] = Inherit(baseButtonStyle, StyleboxFlat(new() {
        ["bg_color"] = BaseColor.Lightened(.1f),
        ["border_color"] = PrimaryColor.Lightened(.2f),
      })),
      ["focus"] = Inherit(baseButtonStyle, StyleboxFlat(new() {
        ["border_color"] = new Color(FontColor, .5f),
        ["draw_center"] = false,
      })),
      ["disabled"] = Inherit(baseButtonStyle, StyleboxFlat(new() {
        ["bg_color"] = BaseColor.Lightened(.2f),
        ["border_width_"] = BorderWidth(0),
      })),
      ["pressed"] = Inherit(baseButtonStyle, StyleboxFlat(new() {
        ["border_color"] = AccentColor,
      })),
      ["icon_max_width"] = 24,
    };

    buttonStyle["hover_pressed"] = Merge(buttonStyle["pressed"].As<Style>(), buttonStyle["hover"].As<Style>(), StyleboxFlat(new() { ["border_color"] = AccentColor.Lightened(.2f) }));

    DefineStyle("Button", buttonStyle);

    DefineStyle("ProgressBar", new Style() {
      ["fill"] = Inherit(baseStyleBox, StyleboxFlat(new() {
        ["bg_color"] = PrimaryTransparent,
        ["border_width_"] = BorderWidth(BaseBorderWidth),
        ["border_color"] = PrimaryColor,
      })),
      ["background"] = baseStyleBox,
    });

    DefineStyle("HSeparator", new() {
      ["separator"] = StyleboxLine(new() {
        ["color"] = new Color(PrimaryColor, 0.5f),
      })
    });


  }
}
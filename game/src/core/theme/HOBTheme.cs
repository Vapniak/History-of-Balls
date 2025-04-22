namespace HOB;

using Godot;

[GlobalClass, Tool]
public partial class HOBTheme : ProgrammaticTheme {
  [Export] public Color FontColor { get; set; } = Colors.White;
  [Export] public Color BaseColor { get; set; }
  [Export] public Color PrimaryColor { get; set; }
  [Export] public Color AccentColor { get; set; }

  [Export] public int BaseSpacing { get; private set; }
  [Export] public int BaseCornerRadius { get; private set; }
  [Export] public int BaseBorderWidth { get; private set; }

  public Color PrimaryTransparent => new(PrimaryColor, .1f);
  public Color AccentTransparent => new(AccentColor.Darkened(.5f).Blend(BaseColor), BaseColor.A);
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
      ["bg_color"] = AccentTransparent,
      ["content_margins_"] = ContentMargins(BaseMargin),
      ["corner_radius_"] = CornerRadius(BaseCornerRadius),
      // ["border_color"] = PrimaryTransparent,
      // ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_blend"] = true,
      ["border_color"] = PrimaryTransparent,
    });

    DefineStyle("Panel", new Style() {
      ["panel"] = baseStyleBox,
    });

    DefineVariantStyle("BorderedPanel", "Panel", new() {
      ["panel"] = Inherit(baseStyleBox, StyleboxFlat(new() {
        ["border_width_"] = BorderWidth(BaseBorderWidth),
        ["border_blend"] = true,
        ["border_color"] = PrimaryTransparent,
      })),
    });

    DefineVariantStyle("TopBarPanel", "Panel", new() {
      ["panel"] = Inherit(baseStyleBox, StyleboxFlat(new() {
        ["corner_radius_"] = CornerRadius(0, 0, BaseCornerRadius, BaseCornerRadius)
      }))
    });

    DefineStyle("TabContainer", new() {
      ["panel"] = baseStyleBox,
    });

    DefineStyle("PopupPanel", new() {
      ["panel"] = baseStyleBox,
    });

    DefineStyle("PopupMenu", new() {
      ["panel"] = baseStyleBox,
      // ["embedded_border"] = StyleboxEmpty(new()),
      // ["embedded_unfocused_border"] = StyleboxEmpty(new()),
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

      DefineVariantStyle("RichTextLabel" + size, "RichTextLabel", new() {
        ["normal_font_size"] = DefaultFontSize + 8 * i,
      });
    }

    DefineStyle("BoxContainer", new Style() {
      ["separation"] = BaseSpacing,
    });


    var borderedStyle = Inherit(baseStyleBox, StyleboxFlat(new() {
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_blend"] = true,
      ["border_color"] = PrimaryColor,
      ["content_margins_"] = ContentMargins(BaseMargin),
      ["anti_aliasing"] = false,
    }));

    var buttonStyle = new Style() {
      ["normal"] = borderedStyle,
      ["hover"] = Inherit(borderedStyle, StyleboxFlat(new() {
        ["bg_color"] = AccentTransparent.Lightened(.1f),
        ["border_color"] = PrimaryColor.Lightened(.2f),
      })),
      ["focus"] = Inherit(borderedStyle, StyleboxFlat(new() {
        ["border_color"] = new Color(FontColor, .5f),
        ["draw_center"] = false,
      })),
      ["disabled"] = Inherit(borderedStyle, StyleboxFlat(new() {
        ["bg_color"] = AccentTransparent.Lightened(.2f),
        ["border_width_"] = BorderWidth(0),
      })),
      ["pressed"] = Inherit(borderedStyle, StyleboxFlat(new() {
        ["border_color"] = AccentColor,
      })),
      ["icon_max_width"] = 24,
    };

    buttonStyle["hover_pressed"] = Merge(buttonStyle["pressed"].As<Style>(), buttonStyle["hover"].As<Style>(), StyleboxFlat(new() { ["border_color"] = AccentColor.Lightened(.2f) }));

    DefineStyle("Button", buttonStyle);

    var endTurnStyle = StyleboxFlat(new() { ["corner_radius_"] = CornerRadius(9999), ["border_width"] = BaseBorderWidth * 10, ["content_marigns_"] = ContentMargins(BaseMargin * 2) });

    DefineVariantStyle("EndTurnButton", "Button", new Style() {
      ["normal"] = Inherit(buttonStyle["normal"].As<Style>(), endTurnStyle),

      ["hover"] = Inherit(buttonStyle["hover"].As<Style>(), endTurnStyle),

      ["focus"] = Inherit(buttonStyle["focus"].As<Style>(), endTurnStyle),

      ["disabled"] = Inherit(buttonStyle["disabled"].As<Style>(), endTurnStyle),

      ["pressed"] = Inherit(buttonStyle["pressed"].As<Style>(), endTurnStyle),

      ["hover_pressed"] = Inherit(buttonStyle["hover_pressed"].As<Style>(), endTurnStyle),

    });

    DefineStyle("ProgressBar", new Style() {
      ["fill"] = Inherit(baseStyleBox, StyleboxFlat(new() {
        ["bg_color"] = PrimaryTransparent,
        ["border_width_"] = BorderWidth(BaseBorderWidth),
        ["border_color"] = PrimaryColor,
      })),
      ["background"] = baseStyleBox,
    });

    var separatorStyle = StyleboxLine(new() {
      ["color"] = new Color(PrimaryColor, 0.5f),
      ["thickness"] = BaseBorderWidth,
    });
    DefineStyle("HSeparator", new() {
      ["separator"] = separatorStyle,
    });

    DefineStyle("VSeparator", new() {
      ["separator"] = Inherit(separatorStyle, new Style() {
        ["vertical"] = true
      })
    });
  }
}
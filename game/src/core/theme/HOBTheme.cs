namespace HOB;

using Godot;

[GlobalClass, Tool]
public partial class HOBTheme : ProgrammaticTheme {
  [Export] public Color FontColor { get; set; } = Colors.White;
  [Export] public Color BaseColor { get; set; }
  [Export] public Color PrimaryColor { get; set; }
  [Export] public Color AccentColor { get; set; }

  [Export] public int BaseSpacing { get; private set; }
  [Export] public int Separation { get; private set; }
  [Export] public int BaseCornerRadius { get; private set; }
  [Export] public int BaseBorderWidth { get; private set; }

  [Export] public FontVariation SmallFont { get; private set; } = default!;

  public Color PrimaryBase => PrimaryColor.Darkened(0.9f);
  public Color AccentBase => AccentColor.Darkened(.9f);

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
      ["bg_color"] = AccentBase,
      ["content_margins_"] = ContentMargins(BaseMargin),
      ["corner_radius_"] = CornerRadius(BaseCornerRadius),
      // ["border_color"] = PrimaryTransparent,
      // ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_blend"] = true,
      ["border_color"] = PrimaryColor,
      ["shadow_color"] = PrimaryBase,
      ["shadow_size"] = BaseBorderWidth * 2,
    });

    var fullRoundedStyle = StyleboxFlat(new() { ["corner_radius_"] = CornerRadius(9999), ["border_width"] = BaseBorderWidth * 10, ["content_marigns_"] = ContentMargins(BaseMargin * 2) });

    DefineStyle("Panel", new Style() {
      ["panel"] = baseStyleBox,
    });

    DefineVariantStyle("BorderedPanel", "Panel", new() {
      ["panel"] = Inherit(baseStyleBox, StyleboxFlat(new() {
        ["border_width_"] = BorderWidth(BaseBorderWidth),
        ["border_blend"] = true,
        ["border_color"] = PrimaryColor,
      })),
    });

    DefineVariantStyle("RoundedPanel", "Panel", new() {
      ["panel"] = Inherit(baseStyleBox, fullRoundedStyle),
    });

    DefineVariantStyle("TopBarPanel", "Panel", new() {
      ["panel"] = Inherit(baseStyleBox, StyleboxFlat(new() {
        ["corner_radius_"] = CornerRadius(0),
        ["border_width_"] = BorderWidth(0, 0, 0, BaseBorderWidth),
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


    var sizes = new[] { "Small", "Medium", "Large", "ExtraLarge" };

    for (var i = 1; i <= sizes.Length; i++) {
      var size = sizes[i - 1];
      DefineVariantStyle("Margin" + size, "MarginContainer", new() {
        ["margins_"] = Margin(BaseMargin * i)
      });

      var font = DefaultFont;
      var scale = i;



      DefineVariantStyle("Spacing" + size, "BoxContainer", new Style() {
        ["separation"] = Separation * scale,
      });


      if (size == "Small") {
        //font = BoldFont;
        scale -= 1;
        font = SmallFont;
      }
      DefineVariantStyle("Header" + size, "Label", new() {
        ["font_color"] = FontColor,
        ["font_size"] = DefaultFontSize + 8 * scale,
        ["font"] = font,
      });
      DefineVariantStyle("RichTextLabel" + size, "RichTextLabel", new() {
        ["normal_font_size"] = DefaultFontSize + 8 * scale,
        ["normal_font"] = font,
        ["default_color"] = FontColor,
      });
    }

    var separationStyle = new Style() {
      ["separation"] = Separation,
    };
    DefineStyle("BoxContainer", separationStyle);

    DefineStyle("HBoxContainer", separationStyle);
    DefineStyle("VBoxContainer", separationStyle);


    var borderedStyle = Inherit(baseStyleBox, StyleboxFlat(new() {
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_blend"] = true,
      ["border_color"] = PrimaryColor,
      ["content_margins_"] = ContentMargins(BaseMargin),
      ["anti_aliasing"] = false,
      // ["shadow_color"] = PrimaryColor.Darkened(.1f),
      // ["shadow_offset"] = new Vector2(BaseBorderWidth, BaseBorderWidth),
      // ["shadow_size"] = BaseBorderWidth,
    }));

    var buttonStyle = new Style() {
      ["normal"] = borderedStyle,
      ["hover"] = Inherit(borderedStyle, StyleboxFlat(new() {
        ["bg_color"] = AccentBase.Lightened(.1f),
        ["border_color"] = PrimaryColor.Lightened(.2f),
        //["shadow_color"] = PrimaryColor.Lightened(.1f),
      })),
      ["focus"] = Inherit(borderedStyle, StyleboxFlat(new() {
        ["border_color"] = new Color(FontColor, .5f),
        //["shadow_size"] = Vector2.Zero,
        ["draw_center"] = false,
      })),
      ["disabled"] = Inherit(borderedStyle, StyleboxFlat(new() {
        ["bg_color"] = AccentBase.Lightened(.2f),
        //["shadow_color"] = AccentBase.Lightened(.1f),
        ["border_width_"] = BorderWidth(0),
      })),
      ["pressed"] = Inherit(borderedStyle, StyleboxFlat(new() {
        ["border_color"] = AccentColor,
        //["shadow_color"] = AccentColor.Darkened(.1f),
      })),
      ["icon_max_width"] = 24,
    };

    buttonStyle["hover_pressed"] = Merge(buttonStyle["pressed"].As<Style>(), buttonStyle["hover"].As<Style>(), StyleboxFlat(new() {
      ["border_color"] = AccentColor.Lightened(.2f),
      //["shadow_color"] = AccentColor.Lightened(.1f),
    }
    ));

    DefineStyle("Button", buttonStyle);


    DefineVariantStyle("EndTurnButton", "Button", new Style() {
      ["normal"] = Inherit(buttonStyle["normal"].As<Style>(), fullRoundedStyle),

      ["hover"] = Inherit(buttonStyle["hover"].As<Style>(), fullRoundedStyle),

      ["focus"] = Inherit(buttonStyle["focus"].As<Style>(), fullRoundedStyle),

      ["disabled"] = Inherit(buttonStyle["disabled"].As<Style>(), fullRoundedStyle),

      ["pressed"] = Inherit(buttonStyle["pressed"].As<Style>(), fullRoundedStyle),

      ["hover_pressed"] = Inherit(buttonStyle["hover_pressed"].As<Style>(), fullRoundedStyle),

    });

    DefineStyle("ProgressBar", new Style() {
      ["fill"] = Inherit(baseStyleBox, StyleboxFlat(new() {
        ["bg_color"] = PrimaryBase,
        ["border_width_"] = BorderWidth(BaseBorderWidth),
        ["border_color"] = PrimaryColor,
      })),
      ["background"] = baseStyleBox,
    });

    var separatorStyle = StyleboxLine(new() {
      ["color"] = PrimaryColor,
      ["thickness"] = BaseBorderWidth,
    });
    DefineStyle("HSeparator", new() {
      ["separation"] = Separation,
      ["separator"] = separatorStyle,
    });

    DefineStyle("VSeparator", new() {
      ["separation"] = Separation,
      ["separator"] = Inherit(separatorStyle, new Style() {
        ["vertical"] = true
      })
    });

    DefineStyle("FlowContainer", new() {
      ["h_separation"] = Separation,
      ["v_separation"] = Separation,
    });
  }
}
namespace HOB;

using System.Windows.Markup;
using Godot;

[GlobalClass, Tool]
public partial class HOBTheme : ProgrammaticTheme {
  [Export] public Color FontColor { get; set; } = Colors.White;
  [Export] public Color BaseColor { get; set; }
  [Export] public Color PrimaryColor { get; set; }
  [Export] public Color AccentColor { get; set; }
  [Export] public Color MoveAbilityColor { get; set; }
  [Export] public Color AttackAbilityColor { get; set; }

  [Export] public int BaseSpacing { get; private set; }
  [Export] public int Separation { get; private set; }
  [Export] public int BaseCornerRadius { get; private set; }
  [Export] public int BaseBorderWidth { get; private set; }

  [Export] public FontVariation SmallFont { get; private set; } = default!;

  public Color PrimaryBase => PrimaryColor.Darkened(0.9f);
  public Color AccentBase => AccentColor.Darkened(.9f);
  public Color ShadowColor => new(0, 0, 0, 0.2f);
  public int BaseMargin => BaseSpacing;

  public override void DefineTheme() {
    // ======================
    // Utility Functions
    // ======================
    Style createBaseStyleBox(Color bgColor, Color borderColor, float shadowOpacity = 0.1f) => StyleboxFlat(new() {
      ["bg_color"] = bgColor,
      ["content_margins_"] = ContentMargins(BaseMargin),
      ["corner_radius_"] = CornerRadius(BaseCornerRadius),
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_color"] = borderColor,
      ["border_blend"] = true,
      // ["shadow_color"] = new Color(0, 0, 0, shadowOpacity),
      // ["shadow_size"] = BaseBorderWidth * 3,
      // ["shadow_offset"] = new Vector2(BaseBorderWidth, BaseBorderWidth),
      ["anti_aliasing"] = true
    });

    Style createRoundedStyle(Color bgColor, Color borderColor) => Inherit(createBaseStyleBox(bgColor, borderColor), StyleboxFlat(new() {
      ["corner_radius_"] = CornerRadius(9999),
      ["content_margins_"] = ContentMargins((int)(BaseMargin * 1.5f))
    }));

    // ======================
    // Panel Styles
    // ======================
    var panelStyle = createBaseStyleBox(BaseColor, PrimaryColor.Darkened(0.3f), 0.15f);

    DefineStyle("Panel", new Style() {
      ["panel"] = panelStyle
    });

    DefineVariantStyle("Card", "PanelContainer", new Style() {
      ["panel"] = Inherit(panelStyle, new Style() {
        ["content_margins_"] = ContentMargins(BaseBorderWidth),
      }),
    });

    var backgroundTransparencyPanel = Inherit(panelStyle, new Style() {
      ["bg_color"] = AccentBase with { A = 0.5f },
      ["corner_radius_"] = CornerRadius(0),
      ["border_width_"] = BorderWidth(0),
    });

    DefineVariantStyle("BackgroundTransparency", "Panel", new() {
      ["panel"] = backgroundTransparencyPanel,
    });

    DefineVariantStyle("ScreenPanel", "Panel", new() {
      ["panel"] = Inherit(createBaseStyleBox(AccentBase, PrimaryColor.Darkened(0.3f)), StyleboxFlat(new() {
        ["corner_radius_"] = CornerRadius(0),
        ["border_width_"] = BorderWidth(0),
      }))
    });


    DefineVariantStyle("RoundedPanel", "Panel", new() {
      ["panel"] = createRoundedStyle(BaseColor, PrimaryColor)
    });

    DefineVariantStyle("TopBarPanel", "Panel", new() {
      ["panel"] = Inherit(panelStyle, StyleboxFlat(new() {
        ["corner_radius_"] = CornerRadius(0),
        ["border_width_"] = BorderWidth(0, 0, 0, BaseBorderWidth * 2),
        //["shadow_size"] = 0 // No shadow for top bar
      }))
    });

    // ======================
    // Container Styles
    // ======================
    var separationStyle = new Style() {
      ["separation"] = Separation,
    };

    DefineStyle("BoxContainer", separationStyle);
    DefineStyle("HBoxContainer", separationStyle);
    DefineStyle("VBoxContainer", separationStyle);
    DefineStyle("FlowContainer", new() {
      ["h_separation"] = Separation * 2,
      ["v_separation"] = Separation * 2,
    });

    // ======================
    // Button Styles
    // ======================
    var buttonNormal = createBaseStyleBox(PrimaryBase, PrimaryColor, 0.2f);
    var buttonHover = Inherit(buttonNormal, StyleboxFlat(new() {
      ["bg_color"] = PrimaryBase.Lightened(0.1f),
      ["border_color"] = PrimaryColor.Lightened(0.2f),
      //["shadow_color"] = new Color(PrimaryColor, 0.25f)
    }));

    var buttonPressed = Inherit(buttonNormal, StyleboxFlat(new() {
      ["bg_color"] = AccentBase,
      ["border_color"] = PrimaryColor.Lightened(0.3f),
      // ["shadow_size"] = BaseBorderWidth,
      // ["shadow_offset"] = new Vector2(0, BaseBorderWidth)
    }));

    var buttonDisabled = Inherit(buttonNormal, StyleboxFlat(new() {
      ["bg_color"] = PrimaryBase.Lightened(.1f),
      ["border_color"] = new Color(PrimaryColor, 0.3f),
      // ["shadow_size"] = 0
    }));

    var buttonFocus = Inherit(buttonNormal, StyleboxFlat(new() {
      ["border_color"] = FontColor,
      ["draw_center"] = false
    }));

    var buttonHoverPressed = Merge(buttonPressed, buttonHover, StyleboxFlat(new() {
      ["border_color"] = PrimaryColor.Lightened(0.4f)
    }));

    var buttonStyle = new Style() {
      ["normal"] = buttonNormal,
      ["hover"] = buttonHover,
      ["focus"] = buttonFocus,
      ["disabled"] = buttonDisabled,
      ["pressed"] = buttonPressed,
      ["hover_pressed"] = buttonHoverPressed,
      ["font_color"] = FontColor,
      ["font_hover_color"] = FontColor.Lightened(0.2f),
      ["font_focus_color"] = FontColor,
      ["font_pressed_color"] = FontColor,
      ["font_disabled_color"] = FontColor.Darkened(0.5f),
      ["icon_max_width"] = 24,
    };

    DefineStyle("Button", buttonStyle);

    var roundedButtonNormal = createRoundedStyle(PrimaryBase, PrimaryColor);
    DefineVariantStyle("EndTurnButton", "Button", Inherit(buttonStyle, new Style() {
      ["normal"] = roundedButtonNormal,
      ["hover"] = Inherit(roundedButtonNormal, StyleboxFlat(new() {
        ["bg_color"] = PrimaryBase.Lightened(0.1f),
        //["shadow_color"] = new Color(PrimaryColor, 0.3f)
      })),
      ["hover_pressed"] = Inherit(buttonHoverPressed, roundedButtonNormal),
      ["disabled"] = Inherit(buttonDisabled, roundedButtonNormal),
      ["focus"] = Inherit(buttonFocus, roundedButtonNormal),
      ["pressed"] = Inherit(roundedButtonNormal, StyleboxFlat(new() {
        ["bg_color"] = AccentBase,
        //["shadow_offset"] = new Vector2(0, BaseBorderWidth)
      })),
    }));

    DefineVariantStyle("AccentButton", "Button", new Style() {
      ["normal"] = createBaseStyleBox(AccentBase, AccentColor, 0.2f),
      ["hover"] = Inherit(buttonHover, StyleboxFlat(new() {
        ["bg_color"] = AccentBase.Lightened(0.1f),
        ["border_color"] = AccentColor.Lightened(0.2f),
        //["shadow_color"] = new Color(AccentColor, 0.25f)
      })),
      ["pressed"] = Inherit(buttonPressed, StyleboxFlat(new() {
        ["bg_color"] = AccentColor,
        ["border_color"] = AccentColor.Lightened(0.3f)
      }))
    });

    // ======================
    // Progress Bar
    // ======================

    var progressBarBackground = createBaseStyleBox(AccentBase, AccentBase);
    DefineStyle("ProgressBar", new Style() {
      ["fill"] = Inherit(createBaseStyleBox(PrimaryColor, PrimaryColor.Lightened(0.2f)), StyleboxFlat(new() {
        ["border_width_"] = BorderWidth(0),
        //["corner_radius_"] = CornerRadius(BaseCornerRadius / 2)
      })),
      ["background"] = progressBarBackground,
      ["font_color"] = FontColor,
      ["font_size"] = DefaultFontSize - 2
    });

    // ======================
    // Separators
    // ======================
    var separatorStyle = StyleboxLine(new() {
      ["color"] = PrimaryColor with { A = 0.5f },
      ["thickness"] = BaseBorderWidth,
      //["grow_"] = Grow(1)
    });

    DefineStyle("HSeparator", new() {
      ["separation"] = Separation * 2,
      ["separator"] = separatorStyle
    });

    DefineStyle("VSeparator", new() {
      ["separation"] = Separation * 2,
      ["separator"] = Inherit(separatorStyle, new Style() {
        ["vertical"] = true
      })
    });

    // ======================
    // Typography
    // ======================
    var sizes = new[] { "Small", "Medium", "Large", "ExtraLarge" };

    for (var i = 0; i < sizes.Length; i++) {
      var size = sizes[i];
      var scale = i + 1;
      var font = i == 0 ? SmallFont : DefaultFont;

      // Margin Containers
      DefineVariantStyle("Margin" + size, "MarginContainer", new() {
        ["margins_"] = Margin(BaseMargin * scale)
      });

      // Spacing Containers
      DefineVariantStyle("Spacing" + size, "BoxContainer", new Style() {
        ["separation"] = Separation * scale,
      });

      // Text Styles
      DefineVariantStyle("Header" + size, "Label", new() {
        ["font_color"] = FontColor,
        ["font_size"] = DefaultFontSize + 8 * scale,
        ["font"] = font,
        // ["shadow_color"] = ShadowColor,
        // ["shadow_offset"] = new Vector2(BaseBorderWidth, BaseBorderWidth)
      });

      DefineVariantStyle("RichTextLabel" + size, "RichTextLabel", new() {
        ["normal_font_size"] = DefaultFontSize + 8 * scale,
        ["normal_font"] = font,
        //["bold_font"] = BoldFont,
        ["default_color"] = FontColor,
        // ["shadow_color"] = ShadowColor,
        // ["shadow_offset_x"] = BaseBorderWidth / 2,
        // ["shadow_offset_y"] = BaseBorderWidth / 2,
      });
    }

    // Base Label Style
    DefineStyle("Label", new() {
      ["font_color"] = FontColor,
      // ["shadow_color"] = ShadowColor,
      // ["shadow_offset"] = new Vector2(BaseBorderWidth / 2, BaseBorderWidth / 2)
    });

    // ======================
    // Special Controls
    // ======================

    // // ======================
    // // TabContainer Styles
    // // ======================

    var baseTab = StyleboxFlat(new() {
      ["corner_radius_"] = CornerRadius(BaseCornerRadius, BaseCornerRadius, 0, 0),
      ["border_width_"] = BorderWidth(0, 0, 0, BaseBorderWidth),
    });
    DefineStyle("TabContainer", new() {
      ["panel"] = Inherit(backgroundTransparencyPanel, StyleboxFlat(new() {

      })),

      ["tabbar_background"] = Inherit(StyleboxFlat(new() {
        ["bg_color"] = Colors.Transparent,
        ["border_color"] = PrimaryColor.Darkened(0.3f)
      })),

      ["tab_unselected"] = Inherit(buttonNormal, baseTab
      , StyleboxFlat(new() {
        //["corner_radius_"] = CornerRadius(BaseCornerRadius, BaseCornerRadius, 0, 0),
        //["content_margins_"] = ContentMargins(BaseMargin * 2, BaseMargin, BaseMargin * 2, BaseMargin),
        ["shadow_size"] = 0
      })),

      ["tab_selected"] = Inherit(buttonPressed, baseTab, StyleboxFlat(new() {
        //["corner_radius_"] = CornerRadius(BaseCornerRadius, BaseCornerRadius, 0, 0),
        //["content_margins_"] = ContentMargins(BaseMargin * 2, BaseMargin, BaseMargin * 2, BaseMargin),
        ["bg_color"] = BaseColor,
        ["border_width_"] = BorderWidth(BaseBorderWidth),
        ["border_color"] = PrimaryColor
      })),

      ["tab_disabled"] = Inherit(buttonDisabled, baseTab, StyleboxFlat(new() {
        //["corner_radius_"] = CornerRadius(BaseCornerRadius, BaseCornerRadius, 0, 0)
      })),

      ["tab_hovered"] = Inherit(buttonHover, baseTab, StyleboxFlat(new() {
        //["corner_radius_"] = CornerRadius(BaseCornerRadius, BaseCornerRadius, 0, 0)
      })),

      ["tab_focus"] = Inherit(buttonFocus, baseTab, StyleboxFlat(new() {

      })),

      // Font settings matching your theme
      ["font"] = DefaultFont,
      ["font_size"] = DefaultFontSize,
      ["font_selected_color"] = FontColor,
      ["font_hovered_color"] = FontColor.Lightened(0.1f),
      ["font_disabled_color"] = FontColor.Darkened(0.5f),
    });


    DefineStyle("PopupPanel", new() {
      //["panel"] = StyleboxEmpty(),
      ["panel"] = Inherit(panelStyle, StyleboxFlat(new Style() {
        //["shadow_size"] = BaseBorderWidth * 4,
        //["shadow_color"] = new Color(0, 0, 0, 0.3f)
      }))
    });

    DefineStyle("PopupMenu", new() {
      ["panel"] = panelStyle,
      ["hover"] = createBaseStyleBox(PrimaryBase with { A = 0.7f },
      PrimaryColor with { A = 0.5f }),
      ["separator"] = separatorStyle
    });

    // ======================
    // Slider Styles
    // ======================
    var sliderTrackStyle = StyleboxFlat(new() {
      ["bg_color"] = AccentBase,
      ["corner_radius_"] = CornerRadius(BaseCornerRadius),
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_color"] = PrimaryColor.Darkened(0.3f),
      ["content_margins_"] = ContentMargins(0, BaseMargin / 2, 0, BaseMargin / 2)
    });

    var sliderTrackHighlightStyle = StyleboxFlat(new() {
      ["bg_color"] = PrimaryBase.Lightened(0.1f),
      ["corner_radius_"] = CornerRadius(BaseCornerRadius),
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_color"] = PrimaryColor,
      ["content_margins_"] = ContentMargins(0, BaseMargin / 2, 0, BaseMargin / 2)
    });

    var sliderGrabberStyle = StyleboxFlat(new() {
      ["bg_color"] = PrimaryColor,
      ["corner_radius_"] = CornerRadius(9999),
      ["border_width_"] = BorderWidth(BaseBorderWidth),
      ["border_color"] = PrimaryColor.Lightened(0.2f),
      // ["shadow_color"] = new Color(0, 0, 0, 0.2f),
      // ["shadow_size"] = BaseBorderWidth,
      // ["shadow_offset"] = new Vector2(0, BaseBorderWidth / 2)
    });

    var sliderGrabberHoverStyle = Inherit(sliderGrabberStyle, StyleboxFlat(new() {
      ["bg_color"] = PrimaryColor.Lightened(0.1f),
      //["shadow_color"] = new Color(PrimaryColor, 0.2f)
    }));

    DefineStyle("HSlider", new Style() {
      ["slider"] = sliderTrackStyle,
      ["grabber_area"] = StyleboxEmpty(),
      ["grabber"] = sliderGrabberStyle,
      ["grabber_highlight"] = sliderGrabberHoverStyle
    });

    DefineStyle("VSlider", new Style() {
      ["slider"] = sliderTrackStyle,
      ["grabber_area"] = StyleboxEmpty(),
      ["grabber"] = sliderGrabberStyle,
      ["grabber_highlight"] = sliderGrabberHoverStyle
    });

    // DefineStyle("Slider", new Style() {
    //   ["slider"] = sliderTrackStyle,
    //   ["grabber_area"] = StyleboxEmpty(),
    //   ["grabber"] = sliderGrabberStyle,
    //   ["grabber_highlight"] = sliderGrabberHoverStyle
    // });

    // Progress bar variant for sliders
    DefineVariantStyle("ProgressSlider", "HSlider", new Style() {
      ["slider"] = Inherit(sliderTrackStyle, StyleboxFlat(new() {
        ["border_width_"] = BorderWidth(0)
      })),
      ["grabber_area_highlight"] = sliderTrackHighlightStyle
    });

    // Tintable slider variant
    DefineVariantStyle("AccentSlider", "HSlider", new Style() {
      ["slider"] = Inherit(sliderTrackStyle, StyleboxFlat(new() {
        ["bg_color"] = AccentBase,
        ["border_color"] = AccentColor.Darkened(0.3f)
      })),
      ["grabber_area_highlight"] = Inherit(sliderTrackHighlightStyle, StyleboxFlat(new() {
        ["bg_color"] = AccentBase.Lightened(0.1f),
        ["border_color"] = AccentColor
      })),
      ["grabber"] = Inherit(sliderGrabberStyle, StyleboxFlat(new() {
        ["bg_color"] = AccentColor,
        ["border_color"] = AccentColor.Lightened(0.2f)
      })),
      ["grabber_highlight"] = Inherit(sliderGrabberHoverStyle, StyleboxFlat(new() {
        ["bg_color"] = AccentColor.Lightened(0.1f),
        //["shadow_color"] = new Color(AccentColor, 0.2f)
      }))
    });

    // // Range slider styling
    // DefineStyle("RangeSlider", new Style() {
    //   ["slider"] = sliderTrackStyle,
    //   ["grabber_area"] = StyleboxEmpty(),
    //   ["grabber_start"] = Inherit(sliderGrabberStyle, StyleboxFlat(new() {
    //     ["bg_color"] = PrimaryColor.Darkened(0.1f)
    //   })),
    //   ["grabber_end"] = sliderGrabberStyle,
    //   ["grabber_start_highlight"] = Inherit(sliderGrabberHoverStyle, StyleboxFlat(new() {
    //     ["bg_color"] = PrimaryColor.Darkened(0.05f)
    //   })),
    //   ["grabber_end_highlight"] = sliderGrabberHoverStyle,
    //   ["range_texture"] = sliderTrackHighlightStyle
    // });
  }
}
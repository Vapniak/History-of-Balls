namespace HOB;

using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;

[Tool, GlobalClass]
public abstract partial class ProgrammaticTheme : Theme {
  public partial class Style : RefCounted {
    private Godot.Collections.Dictionary<string, Variant> Dictionary { get; } = new();

    public Style() { }

    public Style(Style other) {
      Dictionary = other.Dictionary.Duplicate();
    }

    public Variant this[string key] {
      get => Dictionary[key];
      set => Dictionary[key] = value;
    }

    public bool ContainsKey(string key) => Dictionary.ContainsKey(key);
    public void Remove(string key) => Dictionary.Remove(key);
    public System.Collections.Generic.ICollection<string> Keys => Dictionary.Keys;

    public static implicit operator Godot.Collections.Dictionary<string, Variant>(Style style) => style.Dictionary;
  }

  //[Export] private bool Regenerate { get => false; set => GenerateTheme(); }

  private readonly Godot.Collections.Dictionary<string, string> _variantToParentTypeName = new();
  public Godot.Collections.Dictionary<string, Style> Styles { get; } = new();

  [Export]
  private bool Generate {
    get => false;
    set {
      if (value) {
        GenerateTheme();
      }
    }
  }

  public ProgrammaticTheme() {
    //GenerateTheme();
  }

  public virtual void DefineTheme() { }

  public void DefineStyle(string name, Style style) {
    Styles[name] = style;
  }

  public void DefineVariantStyle(string name, string baseTypeName, Style style) {
    _variantToParentTypeName[name] = baseTypeName;
    DefineStyle(name, style);
  }

  public Style Inherit(Style baseStyle, params Style[] stylesToMerge) {
    var inheritedStyle = new Style(baseStyle);

    foreach (var style in stylesToMerge) {
      if (style != null) {
        foreach (var key in style.Keys) {
          inheritedStyle[key] = style[key];
        }
      }
    }

    return inheritedStyle;
  }

  public Style Merge(params Style[] styles) {
    if (styles.Length == 0) {
      return new Style();
    }

    return Inherit(styles[0], styles.Skip(1).ToArray());
  }

  public Style Copy(Style style) {
    return new Style(style);
  }

  public void Include(Style mainStyle, params Style[] stylesToInclude) {
    foreach (var style in stylesToInclude) {
      if (style != null) {
        foreach (var key in style.Keys) {
          mainStyle[key] = style[key];
        }
      }
    }
  }

  public Style StyleboxFlat(Style style) {
    var newStyle = new Style(style);
    newStyle["type"] = "stylebox_flat";
    return newStyle;
  }

  public Style StyleboxLine(Style style) {
    var newStyle = new Style(style);
    newStyle["type"] = "stylebox_line";
    return newStyle;
  }

  public Style StyleboxEmpty(Style style) {
    var newStyle = new Style(style);
    newStyle["type"] = "stylebox_empty";
    return newStyle;
  }

  public Style StyleboxTexture(Style style) {
    var newStyle = new Style(style);
    newStyle["type"] = "stylebox_texture";
    return newStyle;
  }

  private void GenerateTheme() {
    Reset();
    Clear();
    DefineTheme();

    LoadVariants();

    foreach (var typeName in Styles.Keys) {
      var style = Styles[typeName];
      style = PreprocessStyle(style);
      LoadStyle(typeName, style);
    }
  }

  private void Reset() {
    Styles.Clear();
    _variantToParentTypeName.Clear();
  }

  private void LoadVariants() {
    foreach (var variantName in _variantToParentTypeName.Keys) {
      GD.Print("Load variant: ", variantName);
      AddType(variantName);
    }

    foreach (var kvp in _variantToParentTypeName) {
      GD.Print("Set type variation: ", kvp.Key);
      SetTypeVariation(kvp.Key, kvp.Value);
    }
  }

  private Style PreprocessStyle(Style style) {
    var processedStyle = new Style(style);
    var keys = processedStyle.Keys.ToArray();

    foreach (var key in keys) {
      if (processedStyle[key].Obj is Style nestedStyle) {
        processedStyle[key] = PreprocessStyle(nestedStyle);
      }

      if (key.EndsWith("_")) {
        MergeSubDictIntoMainDict(processedStyle, key);
      }
    }

    return processedStyle;
  }

  private void MergeSubDictIntoMainDict(Style mainDict, string subDictName) {
    if (!mainDict.ContainsKey(subDictName))
      return;

    var subDict = mainDict[subDictName].As<Style>();
    if (subDict == null)
      return;

    foreach (var key in subDict.Keys) {
      mainDict[key] = subDict[key];
    }

    mainDict.Remove(subDictName);
  }

  private void LoadStyle(string typeName, Style style) {
    GD.Print("Load style: ", typeName);
    AddType(typeName);

    foreach (var key in style.Keys) {
      LoadStyleItem(typeName, key, style[key]);
    }
  }

  private void LoadStyleItem(string typeName, string itemName, Variant value) {
    var dataType = GetDataTypeForValue(typeName, itemName);
    GD.Print(itemName);
    if (dataType == -1) {
      GD.PushError($"Item name '{itemName}' not recognized for type '{typeName}'.");
      return;
    }

    if ((DataType)dataType == DataType.Stylebox && value.Obj is Style styleDict) {
      value = CreateStyleboxFromDict(styleDict);
    }

    GD.Print("Set theme item: ", itemName, typeName);
    SetThemeItem((DataType)dataType, itemName, typeName, value);
  }

  private StyleBox? CreateStyleboxFromDict(Style data) {
    if (!data.ContainsKey("type") || data["type"].AsString() is not string type) {
      return null;
    }

    StyleBox? stylebox = type switch {
      "stylebox_flat" => new StyleBoxFlat(),
      "stylebox_line" => new StyleBoxLine(),
      "stylebox_empty" => new StyleBoxEmpty(),
      "stylebox_texture" => new StyleBoxTexture(),
      _ => null,
    };

    if (stylebox == null) {
      return null;
    }

    foreach (var key in data.Keys) {
      if (key == "type") {
        continue;
      }

      stylebox.Set(key, data[key]);
    }

    return stylebox;
  }

  private int GetDataTypeForValue(string typeName, string itemName) {
    GD.Print(typeName, itemName);
    if (TypeHasColorItem(typeName, itemName))
      return (int)DataType.Color;
    if (TypeHasConstantItem(typeName, itemName))
      return (int)DataType.Constant;
    if (TypeHasFontItem(typeName, itemName))
      return (int)DataType.Font;
    if (TypeHasFontSizeItem(typeName, itemName))
      return (int)DataType.FontSize;
    if (TypeHasIconItem(typeName, itemName))
      return (int)DataType.Icon;
    if (TypeHasStyleItem(typeName, itemName))
      return (int)DataType.Stylebox;

    var parent = GetTypeVariationBase(typeName);
    if (string.IsNullOrEmpty(parent)) {
      var defaultTheme = ThemeDB.GetDefaultTheme();
      parent = defaultTheme.GetTypeVariationBase(typeName);
    }

    if (string.IsNullOrEmpty(parent))
      return -1;

    return GetDataTypeForValue(parent, itemName);
  }

  private bool TypeHasColorItem(string typeName, string itemName) {
    return TypeHasProperty(typeName, $"theme_override_colors/{itemName}");
  }

  private bool TypeHasConstantItem(string typeName, string itemName) {
    return TypeHasProperty(typeName, $"theme_override_constants/{itemName}");
  }

  private bool TypeHasFontItem(string typeName, string itemName) {
    return TypeHasProperty(typeName, $"theme_override_fonts/{itemName}");
  }

  private bool TypeHasFontSizeItem(string typeName, string itemName) {
    return TypeHasProperty(typeName, $"theme_override_font_sizes/{itemName}");
  }

  private bool TypeHasIconItem(string typeName, string itemName) {
    return TypeHasProperty(typeName, $"theme_override_icons/{itemName}");
  }

  private bool TypeHasStyleItem(string typeName, string itemName) {
    return TypeHasProperty(typeName, $"theme_override_styles/{itemName}");
  }

  private bool TypeHasProperty(string typeName, string propertyName) {
    if (!ClassDB.ClassExists(typeName)) {
      return false;
    }

    var properties = ClassDB.Instantiate(typeName).AsGodotObject().GetPropertyList();
    foreach (var property in properties) {
      if (property["name"].AsString() == propertyName) {
        return true;
      }
    }
    return false;
  }

  public Style BorderWidth(int left, int? top = null, int? right = null, int? bottom = null) {
    top ??= left;
    right ??= left;
    bottom ??= top;

    var style = new Style();
    style["border_width_left"] = left;
    style["border_width_top"] = top.GetValueOrDefault();
    style["border_width_right"] = right.GetValueOrDefault();
    style["border_width_bottom"] = bottom.GetValueOrDefault();
    return style;
  }

  public Style CornerRadius(int topLeft, int? topRight = null, int? bottomRight = null, int? bottomLeft = null) {
    topRight ??= topLeft;
    bottomRight ??= topRight;
    bottomLeft ??= topLeft;

    var style = new Style();
    style["corner_radius_top_left"] = topLeft;
    style["corner_radius_top_right"] = topRight.GetValueOrDefault();
    style["corner_radius_bottom_right"] = bottomRight.GetValueOrDefault();
    style["corner_radius_bottom_left"] = bottomLeft.GetValueOrDefault();
    return style;
  }

  public Style ExpandMargins(int left, int? top = null, int? right = null, int? bottom = null) {
    top ??= left;
    right ??= left;
    bottom ??= top;

    var style = new Style();
    style["expand_margin_left"] = left;
    style["expand_margin_top"] = top.GetValueOrDefault();
    style["expand_margin_right"] = right.GetValueOrDefault();
    style["expand_margin_bottom"] = bottom.GetValueOrDefault();
    return style;
  }

  public Style ContentMargins(int left, int? top = null, int? right = null, int? bottom = null) {
    top ??= left;
    right ??= left;
    bottom ??= top;

    var style = new Style();
    style["content_margin_left"] = left;
    style["content_margin_top"] = top.GetValueOrDefault();
    style["content_margin_right"] = right.GetValueOrDefault();
    style["content_margin_bottom"] = bottom.GetValueOrDefault();
    return style;
  }

  public Style Margin(int left, int? top = null, int? right = null, int? bottom = null) {
    top ??= left;
    right ??= left;
    bottom ??= top;

    var style = new Style();
    style["margin_left"] = left;
    style["margin_top"] = top.GetValueOrDefault();
    style["margin_right"] = right.GetValueOrDefault();
    style["margin_bottom"] = bottom.GetValueOrDefault();
    return style;
  }

  public Style TextureMargins(int left, int? top = null, int? right = null, int? bottom = null) {
    top ??= left;
    right ??= left;
    bottom ??= top;

    var style = new Style();
    style["texture_margin_left"] = left;
    style["texture_margin_top"] = top.GetValueOrDefault();
    style["texture_margin_right"] = right.GetValueOrDefault();
    style["texture_margin_bottom"] = bottom.GetValueOrDefault();
    return style;
  }

  // public Style CreateColorStyle(Color color) {
  //   var style = new Style();
  //   style["color"] = color;
  //   return style;
  // }

  // public Style CreateFontStyle(Font font, int? size = null) {
  //   var style = new Style();
  //   style["font"] = font;
  //   if (size.HasValue) {
  //     style["font_size"] = size.Value;
  //   }

  //   return style;
  // }

  // public Style CreateConstantStyle(int value) {
  //   var style = new Style();
  //   style["constant"] = value;
  //   return style;
  // }

  // public Style CreateIconStyle(Texture2D icon) {
  //   var style = new Style();
  //   style["icon"] = icon;
  //   return style;
  // }

  // public Style CreateStylebox(StyleBox stylebox) {
  //   var style = new Style();
  //   style["stylebox"] = stylebox;
  //   return style;
  // }
}
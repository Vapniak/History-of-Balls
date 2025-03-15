@tool
extends ProgrammaticTheme

const UPDATE_ON_SAVE = true

var background_color: Color = Color(.1, .1, .1)
var primary_color = Color.CORNFLOWER_BLUE
var text_color = Color.WHITE

var default_font = "res://res/fonts/normal_font.tres"
var large_font = "res://res/fonts/large_font.tres"
var base_font_size = 16

var base_corner_radius = 5
var base_border_width = 7
var base_margin = 5

func setup():
	set_save_path("res://res/default_theme.tres")

func define_theme():
	define_default_font(ResourceLoader.load(default_font))
	define_default_font_size(base_font_size)

	var panel_style = stylebox_flat({
		bg_color = background_color.darkened(0.5),
		corner_radius_ = corner_radius(base_corner_radius * 2),
		content_margins_ = content_margins(base_margin * 2),
	})

	define_style("PanelContainer", {
		panel = panel_style,
	})

	define_variant_style("TopBarPanel", "PanelContainer", {
		panel = inherit(panel_style, {
			content_margins_ = content_margins(base_margin),
			corner_radius_ = corner_radius(0, 0, base_corner_radius* 2, base_corner_radius * 2)
		}),
	})

	var button_style = stylebox_flat({
		bg_color = background_color,
		content_margins_ = content_margins(base_margin),
		corner_radius_ = corner_radius(base_corner_radius),
		#border_width_ = border_width(0, 0, 0, base_border_width),
		#border_color = background_color.darkened(0.3)
	})

	var hover_style = inherit(button_style, {
		bg_color = background_color.lightened(0.15),
		#border_color = background_color.lightened(0.1)
	})

	var focus_style = inherit(button_style, {
		draw_center = false,
	})

	var pressed_style = inherit(button_style, {
		bg_color = background_color.darkened(1),
	})

	var disabled_style = inherit(button_style, {
		bg_color = background_color.lightened(.1)
	})

	define_style("Button", {
		normal = button_style,
		hover = hover_style,
		focus = focus_style,
		pressed = pressed_style,
		disabled = disabled_style,
	})

	define_variant_style("ActionButton", "Button", {

	})

	define_variant_style("ButtonMenu", "Button", {
		font_size = base_font_size + 8
	})

	define_variant_style("HeaderLarge", "Label", {
		font = ResourceLoader.load(large_font),
		font_size = base_font_size + 24,
	})

	define_variant_style("HeaderMedium", "Label", {
		font_size = base_font_size + 16,
	})

	define_variant_style("HeaderSmall", "Label", {
		font_size = base_font_size + 8,
	})

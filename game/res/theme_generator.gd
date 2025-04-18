@tool
extends ProgrammaticTheme

const UPDATE_ON_SAVE = true

var background_color: Color = Color(.1, .1, .1, .8)
var primary_color = Color("#fcc428")
var text_color = Color.WHITE

var default_font = "res://res/fonts/normal_font.tres"
var large_font = "res://res/fonts/large_font.tres"
var base_font_size = 16

var base_corner_radius = 5
var base_border_width = 2
var base_margin = 10

func setup():
	set_save_path("res://res/default_theme.tres")

func define_theme():
	define_default_font(ResourceLoader.load(default_font))
	define_default_font_size(base_font_size)

	var panel_style = stylebox_flat({
		bg_color = background_color.darkened(0.5),
		corner_radius_ = corner_radius(base_corner_radius * 2),
		content_margins_ = content_margins(base_margin * 2),
		#border_width_ = border_width(1),
		#border_color = background_color.lightened(.5),
	})

	define_style("PanelContainer", {
		panel = stylebox_empty({}),
	})

	define_style("Panel", {
		panel = panel_style,
	})

	define_variant_style("TooltipPanel", "Panel",{
		panel = stylebox_empty({})
	});

	define_variant_style("TopBarPanel", "Panel", {
		panel = inherit(panel_style, {
			content_margins_ = content_margins(base_margin),
			corner_radius_ = corner_radius(0, 0, base_corner_radius* 2, base_corner_radius * 2)
		}),
	})

	define_variant_style("BackgroundPanel", "Panel", {
		panel = inherit(panel_style, {
			corner_radius_ = corner_radius(0)
		})
	})

	var transparent_bg = Color.BLACK
	transparent_bg.a = 0.9
	define_variant_style("BackgroundTransparency", "Panel", {
		panel = stylebox_flat({
			bg_color = transparent_bg,
		})
	})

	define_style("BoxContainer", {
		separation = base_margin
	})

	var button_style = stylebox_flat({
		bg_color = background_color,
		content_margins_ = content_margins(base_margin),
		corner_radius_ = corner_radius(base_corner_radius),
		border_width_ = border_width(0, 0, 0, base_border_width * 3),
		border_color = primary_color.darkened(0.3)
	})

	var hover_style = inherit(button_style, {
		bg_color = background_color.lightened(0.05),
		#border_color = background_color.lightened(0.2)
	})

	var focus_style = inherit(button_style, {
		draw_center = false,
	})

	var pressed_style = inherit(button_style, {
		bg_color = primary_color.darkened(.3),
		#border_color = background_color.darkened(.5)
	})

	var disabled_style = inherit(button_style, {
		bg_color = background_color.lightened(.2)
	})

	define_style("Button", {
		normal = button_style,
		hover = hover_style,
		focus = focus_style,
		pressed = pressed_style,
		disabled = disabled_style,
		icon_max_width = 24,
	})

	var action_button_style = stylebox_flat({
		bg_color = background_color,
		border_width_ = border_width(0),
	})

	var action_button_border_style = stylebox_flat(({
		border_width_ = border_width(base_border_width),
		border_color = primary_color.lightened(.7),
	}))

	define_variant_style("ActionButton", "Button", {
		normal = inherit(button_style, action_button_style),
		hover = merge(inherit(hover_style, action_button_style),action_button_border_style),
		focus = merge(inherit(focus_style, action_button_style), action_button_border_style),
		pressed = inherit(pressed_style, action_button_style),
		disabled = inherit(disabled_style, action_button_style),
	})

	define_variant_style("LargeButton", "Button", {
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

	define_variant_style("MarginLarge", "MarginContainer", {
		content_margins_ = margin(base_margin * 3),
	})

	define_variant_style("MarginMedium", "MarginContainer", {
		content_margins_ = margin(base_margin * 2),
	})

	define_variant_style("MarginSmall", "MarginContainer", {
		margin_ = margin(base_margin)
	})


func margin(left: int, top = null, right = null, bottom = null):
	if top == null: top = left
	if right == null: right = left
	if bottom == null: bottom = top

	return {
		"margin_left": left,
		"margin_top": top,
		"margin_right": right,
		"margin_bottom": bottom
	}

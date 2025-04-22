shader_type canvas_item;

uniform sampler2D background_texture;
uniform vec4 tint: source_color;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	//vec4 tex = texture(background_texture, 1. / TEXTURE_PIXEL_SIZE * UV);
	vec4 tex = texture(background_texture, SCREEN_UV);
	vec4 color = COLOR + tex * tint;
	color.a = COLOR.a;
	COLOR = color;
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}

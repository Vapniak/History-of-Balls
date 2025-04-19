shader_type spatial;
render_mode blend_mix, unshaded, cull_disabled;

// Cloud settings
uniform vec4 cloud_color : source_color = vec4(1.0, 1.0, 1.0, 0.3);
uniform float density : hint_range(0, 5) = 1.5;
uniform float speed : hint_range(0, 1) = 0.1;
uniform sampler2D noise_texture : hint_default_white;

void fragment() {
	ALPHA = UV.y * 0.9;
	ALBEDO = texture(noise_texture, UV * TIME * speed).rgb;
}
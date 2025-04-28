shader_type canvas_item;

uniform float strength : hint_range(0.0, 0.5, 0.005) = 0.05;
uniform float speed : hint_range(0.0, 2.0, 0.05) = 0.5;
uniform float frequency : hint_range(0.1, 20.0, 0.1) = 3.0;
uniform vec2 direction = vec2(1.0, 0.0);
uniform float noise_amount : hint_range(0.0, 0.5, 0.01) = 0.1;

uniform sampler2D noise_texture : hint_default_white, repeat_enable;

void fragment() {
    vec2 wind_dir = normalize(direction);


    float wind_wave = sin((UV.y * frequency) + (TIME * speed)) * 3.14159;

    float noise = texture(noise_texture, UV * 2.0 + vec2(TIME * 0.3)).r;
    noise = (noise - 0.5) * 2.0;

    float combined_effect = wind_wave + (noise * noise_amount);


    vec2 distortion = wind_dir * (combined_effect * strength * 0.1);

    vec2 distorted_uv = clamp(UV + distortion, vec2(0.001), vec2(0.999));


    COLOR = texture(TEXTURE, distorted_uv);

    COLOR.rgb *= 1.0 + (combined_effect * 0.03);
}
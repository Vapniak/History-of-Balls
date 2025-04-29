shader_type spatial;

#include "include/hex_utils.gdshaderinc"

uniform float hex_texture_scale;
uniform sampler2DArray hex_textures;
uniform sampler2D hex_texture_lookup;

uniform sampler2D noise_texture : hint_default_white;
uniform float distortion_strength = 0.1;
uniform float noise_scale = 5.0;
uniform float color_variation = 0.1;
uniform float texture_opacity;
uniform float rotation_variation;
uniform float scale_variation;
uniform float edge_smoothness;


uniform sampler2D highlight_data_texture : source_color, repeat_disable, hint_default_transparent;
uniform sampler2D terrain_data_texture : source_color, repeat_disable;

uniform float highlight_opacity : hint_range(0.0, 1.0) = 0.5;
uniform float mouse_highlight_opacity : hint_range(0.0, 1.0) = 0.3;
uniform float highlight_intensity : hint_range(0.0, 2.0) = 1.2;

uniform float grid_overlay_color_range: hint_range(0.0, 1.0, 0.01);
uniform ivec2 grid_size;
uniform vec3 grid_color: source_color;

uniform vec2 mouse_world_pos;
uniform bool show_mouse_highlight;

const int CELL_SCALE = 2;

vec3 smooth_blend(vec3 base, vec4 tex, float opacity) {
    float edge = smoothstep(0.5 - edge_smoothness, 0.5 + edge_smoothness, tex.r);

    edge = edge * edge * (3.0 - 2.0 * edge);

    return mix(base, tex.rgb, edge * opacity);
}

vec4 overlay(vec4 base, vec4 blend, float opacity) {
    float base_lum = dot(base.rgb, vec3(0.299, 0.587, 0.114));
    float blend_lum = dot(blend.rgb, vec3(0.299, 0.587, 0.114));

    vec3 result;
    if (base_lum < 0.5) {
        result = 2.0 * base.rgb * blend.rgb;
    } else {
        result = 1.0 - 2.0 * (1.0 - base.rgb) * (1.0 - blend.rgb);
    }

    result = mix(base.rgb, result, opacity * blend.a);
    return vec4(result, base.a);
}

vec4 apply_highlight(vec4 terrain_color, vec4 highlight_color, HexCellData data, bool is_mouse_highlight) {
    float highlight_strength = smoothstep_range(data, grid_overlay_color_range - .1, grid_overlay_color_range);
    
    float alpha = is_mouse_highlight ? mouse_highlight_opacity : highlight_opacity * highlight_color.a;
    
    vec3 blended_color;
    
    if (length(highlight_color.rgb) > 0.1) {
        blended_color = terrain_color.rgb + (highlight_color.rgb * highlight_intensity * alpha * highlight_strength);
    } 
    else {
        blended_color = mix(
            terrain_color.rgb,
            vec3(1.0),
            alpha * highlight_strength * 0.7
        );
    }
    
    return vec4(blended_color, terrain_color.a);
}

void vertex() {

    // Vertex shader logic (if needed)
}

void fragment() {
    vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
    HexCellData data = get_hex_cell_data(vec2(world_pos.x, world_pos.z), CELL_SCALE);

	vec4 terrain_color = texelFetch(terrain_data_texture, data.offsetCoord, 0);
	if(terrain_color.a > 0.){
		ALBEDO = terrain_color.rgb;

		vec2 scaled_uv = UV * hex_texture_scale;
		vec2 uv_int = floor(scaled_uv);
		vec2 uv_fract = fract(scaled_uv);
		vec2 mirrored_uv = mix(uv_fract, 1.0 - uv_fract, mod(uv_int, 2.0));

		float tex_index = texelFetch(hex_texture_lookup, data.offsetCoord, 0).r * 255.0;


		 if(tex_index > 0.5) {
			vec4 noise1 = texture(noise_texture, mirrored_uv * noise_scale * 0.3);
			vec4 noise2 = texture(noise_texture, mirrored_uv * noise_scale * 1.7 + vec2(0.3, 0.7));

		    float rotation_angle = (noise1.r - 0.5) * rotation_variation;
		    vec2 uv = mirrored_uv - 0.5;
		    uv = vec2(
		        cos(rotation_angle) * uv.x - sin(rotation_angle) * uv.y,
		        sin(rotation_angle) * uv.x + cos(rotation_angle) * uv.y
		    );

		    float scale_factor = 1.0 + (noise1.g - 0.5) * scale_variation;
		    uv *= scale_factor;
		    uv += 0.5;

		    vec2 distortion = (noise2.rg - 0.5) * 2.0 * distortion_strength;
		    uv += distortion / 1000.;

		    vec4 tex_color = texture(hex_textures, vec3(uv, tex_index - 1.0));

		        tex_color.rgb *= (1.0 + (noise1.b - 0.5) * color_variation);

		    vec3 blended_color = smooth_blend(ALBEDO, tex_color, texture_opacity);
//
		    //ALBEDO = overlay(vec4(ALBEDO, 1.0), vec4(blended_color, 1.0), 0.5).rgb;
			ALBEDO *= mix(ALBEDO, blended_color, texture_opacity);

		    float edge_ao = 1.0 - smoothstep(0.4, 0.6, tex_color.r) * 0.2;
        }

	}
	else {
		ALBEDO = vec3(0, 0, 0);
	}



	// apply grid
	ALBEDO = mix(ALBEDO, grid_color, smoothstep_range(data, grid_overlay_color_range, 1));

	HexCellData mouse_hex_data = get_hex_cell_data(mouse_world_pos, CELL_SCALE);

	vec4 highlight_color = texelFetch(highlight_data_texture, data.offsetCoord, 0);

	//highlight_color.rgb = highlight_color.rgb + 0.1f;
	
	ALBEDO = apply_highlight(vec4(ALBEDO, 1.), highlight_color, data, mouse_hex_data.offsetCoord == data.offsetCoord && show_mouse_highlight).rgb;
	
	METALLIC = 0.0;
    ROUGHNESS = 1.0;
}
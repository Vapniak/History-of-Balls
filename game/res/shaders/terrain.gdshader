shader_type spatial;

#include "include/hex_utils.gdshaderinc"

uniform sampler2DArray hex_textures;
uniform sampler2D hex_texture_lookup;

uniform sampler2D highlight_data_texture : source_color, repeat_disable;
uniform sampler2D terrain_data_texture : source_color, repeat_disable;

uniform ivec2 grid_size;

uniform vec2 mouse_world_pos;
uniform bool show_mouse_highlight;

const int CELL_SCALE = 2;

vec4 overlay(vec4 base, vec4 blend){
	vec4 limit = step(0.5, base);
	return mix(2.0 * base * blend, 1.0 - 2.0 * (1.0 - base) * (1.0 - blend), limit);
}


// TODO: when colors are simmilar highlight is not visible
vec3 apply_highlight(vec3 base_color, vec3 highlight_color, HexCellData data){
	vec3 highlighted_color = mix(base_color, highlight_color.rgb, smoothstep_range(data, .75, .84));
	return mix(highlighted_color, highlight_color + vec3(0.2), smoothstep_range(data, .7, .75));
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

		vec2 scaled_uv = UV * 2.;
		vec2 uv_int = floor(scaled_uv);
		vec2 uv_fract = fract(scaled_uv);
		vec2 mirrored_uv = mix(uv_fract, 1.0 - uv_fract, mod(uv_int, 2.0));
		//ALBEDO *= max(texture(noise, UV / 5.).rgb, vec3(0.9));
		float tex_index = texelFetch(hex_texture_lookup, data.offsetCoord, 0).r * 255.0;
		
		//ALBEDO = overlay(vec4(ALBEDO, 1), texture(hex_textures, vec3(mirrored_uv.x, mirrored_uv.y, id.r))).rgb;
		 if(tex_index > 0.5) { 
            vec4 tex_color = texture(hex_textures, vec3(mirrored_uv, tex_index - 1.0));
            ALBEDO *= mix(ALBEDO, tex_color.rgb, 0.2);
        }
        
	}
	else {
		ALBEDO = vec3(0, 0, 0);
	}



	// apply grid
	ALBEDO = mix(ALBEDO, vec3(0.01), smoothstep_range(data, 0.84, 1));

	HexCellData mouse_hex_data = get_hex_cell_data(mouse_world_pos, CELL_SCALE);

	vec4 highlight_color = texelFetch(highlight_data_texture, data.offsetCoord, 0);

	//highlight_color.rgb = highlight_color.rgb + 0.1f;

	if(mouse_hex_data.offsetCoord == data.offsetCoord && show_mouse_highlight){
		if (highlight_color.a > 0.) {
    		ALBEDO = apply_highlight(ALBEDO, highlight_color.rgb + 0.1, data);
		}
		else {
			ALBEDO = apply_highlight(ALBEDO, ALBEDO + 0.1, data);
		}
	}
	else if (highlight_color.a > 0.) {
  		ALBEDO = apply_highlight(ALBEDO, highlight_color.rgb, data);
	}

}
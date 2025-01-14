shader_type spatial;

#include "hex_utils.gdshaderinc"

uniform sampler2D highlight_data_texture : source_color, repeat_disable;
uniform sampler2D terrain_data_texture : source_color, repeat_disable;
uniform ivec2 terrain_size = ivec2(10, 10);

const int CELL_SCALE = 2;

void vertex() {
    // Vertex shader logic (if needed)
}

void fragment() {
    vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
    HexCellData data = get_hex_cell_data(vec2(world_pos.x, world_pos.z), CELL_SCALE);
	
	vec4 terrain_color = texelFetch(terrain_data_texture, data.offsetCoords, 0);
	if(terrain_color.a > 0.){	
		ALBEDO = terrain_color.rgb;
	}
	else {
		ALBEDO = vec3(0, 0, 0);
	}
	
	// apply grid
	ALBEDO = mix(ALBEDO, ALBEDO - vec3(0.1), step(.85, data.distanceToEdge)); //* 0.01;

	vec4 highlight_color = texelFetch(highlight_data_texture, data.offsetCoords, 0);
    if (highlight_color.a > 0.) {
    	ALBEDO = mix(ALBEDO, highlight_color.rgb, smoothstep_range(data, .5, .7));
    }
}
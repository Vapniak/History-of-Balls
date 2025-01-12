shader_type spatial;
render_mode unshaded;

#include "hex_utils.gdshaderinc"

uniform sampler2D terrain_data : source_color,repeat_disable;
uniform vec2 terrain_size;

vec3 apply_grid(vec3 base_color, vec3 grid_color, HexCellData data){
	return mix(base_color, grid_color, smoothstep01(data, .85));
}

vec3 apply_highlight(vec3 base_color, HexCellData data)
{
	// TODO: apply white highligh but add textures
	return mix(base_color, vec3(0, 1, 1), smoothstep_range(data, 0.6, 0.8));
}

vec4 get_terrain_data(ivec2 offset_coords){
	vec2 uv = vec2(offset_coords);
	uv.x /= terrain_size.x;
	uv.y /= terrain_size.y;
	if(uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0){
		return vec4(0, 0, 0, 0);
	}
	return texture(terrain_data, uv);
}


void vertex() {

}

void fragment() {
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	HexCellData data = get_hex_cell_data(vec2(world_pos.x, world_pos.z), 2);
	
	ALBEDO = get_terrain_data(data.offsetCoords).rgb;
	
	ALBEDO = apply_grid(ALBEDO, vec3(0.1, 0.1, 0.1), data);
	
	//if(is_highlighted(data, highlight_texture)){	
		//ALBEDO = apply_highlight(ALBEDO, data);
	//}
}


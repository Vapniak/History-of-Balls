shader_type spatial;
render_mode unshaded;

#include "hex_utils.gdshaderinc"

uniform sampler2D highlight_data : source_color;

vec3 apply_grid(vec3 base_color, vec3 grid_color, HexCellData data){
	return mix(grid_color, base_color, smoothstep10(data, 0.95));
}

vec3 apply_highlight(vec3 base_color, HexCellData data)
{
	// TODO: apply white highligh but add textures
	return mix(base_color, vec3(0, 1, 1), smoothstep_range(data, 0.7, 0.8));
}


void vertex() {

}

void fragment() {
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	HexCellData data = get_hex_cell_data(vec2(world_pos.x, world_pos.z));
	
	ALBEDO = apply_grid(vec3(1, 1, 1), vec3(1, 0, 0), data);
	
	//if(is_highlighted(data, highlight_texture)){	
		//ALBEDO = apply_highlight(ALBEDO, data);
	//}
}


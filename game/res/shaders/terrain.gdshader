shader_type spatial;
render_mode unshaded;

#include "hex_utils.gdshaderinc"

vec3 apply_grid(vec3 base_color, HexCellData data){
	return base_color * smoothstep10(data, 0.965);
}


void vertex() {

}

void fragment() {
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	HexCellData data = get_hex_cell_data(vec2(world_pos.x, world_pos.z));
	ALBEDO = apply_grid(vec3(1, 1, 1), data);
}


shader_type spatial;
render_mode unshaded;

#include "include/hex_utils.gdshaderinc"

const int CELL_SCALE = 2;

void vertex() {

    // Vertex shader logic (if needed)
}

void fragment() {
    vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
    HexCellData data = get_hex_cell_data(vec2(world_pos.x, world_pos.z), CELL_SCALE);

	ALBEDO = vec3(0, 0, 0);

	// apply grid
	ALBEDO = mix(ALBEDO, vec3(0.01), smoothstep_range(data, 0.84, 1));
}

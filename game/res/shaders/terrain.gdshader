shader_type spatial;
render_mode unshaded;

#include "hex_utils.gdshaderinc"

uniform sampler2D terrain_data_texture : source_color, repeat_disable;
uniform sampler2D highlight_data_texture : source_color, repeat_disable;
uniform ivec2 terrain_size = ivec2(10, 10);

const int CELL_SCALE = 2;

vec3 apply_grid(vec3 base_color, vec3 grid_color, HexCellData data) {
    return mix(base_color, grid_color, smoothstep01(data, 0.85));
}

vec3 apply_highlight(vec3 base_color, HexCellData data) {
    vec4 highlight_color = texelFetch(highlight_data_texture, data.offsetCoords, 0);

    return mix(base_color, highlight_color.rgb, smoothstep_range(data, 0.6, 0.8));
}

vec4 get_terrain_data_texture(ivec2 offset_coords) {
	// i hate this... i sit for about 10 hours trying to figure out why it colors 4 cells. it turns out that texture gets blurred when you use texture() function...
    return texelFetch(terrain_data_texture, offset_coords, 0);
}

void vertex() {
    // Vertex shader logic (if needed)
}

void fragment() {
    vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
    HexCellData data = get_hex_cell_data(vec2(world_pos.x, world_pos.z), CELL_SCALE);
		

    vec3 base_color = get_terrain_data_texture(data.offsetCoords).rgb;

    ALBEDO = apply_grid(base_color, vec3(0.001, 0.001, 0.001), data);

    if (is_highlighted(data, highlight_data_texture)) {
        ALBEDO = apply_highlight(ALBEDO, data);
    }
}
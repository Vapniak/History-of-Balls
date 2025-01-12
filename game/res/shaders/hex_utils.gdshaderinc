ivec2 axial_round(vec2 frac)
{
	float s0 = -frac.x - frac.y;
    float q = round(frac.x);
    float r = round(frac.y);
    float s = round(s0);

    float q_diff = abs(q - frac.x);
    float r_diff = abs(r - frac.y);
    float s_diff = abs(s - s0);

    if (q_diff > r_diff && q_diff > s_diff)
	{
        q = -r-s;
	}
    else if (r_diff > s_diff)
	{
        r = -q-s;
	}
    return ivec2( int( q  ), int( r ));
}

vec2 point_to_axial_space(vec2 xy){
	float q = (sqrt(3) / 3. * xy.x) + (-1. / 3. * xy.y);
	float r = (2./3. * xy.y);
	return vec2(q, r);
}

ivec2 point_to_axial(vec2 xy){
	return axial_round(point_to_axial_space(xy));
}



vec2 axial_to_point(ivec2 axial){
	float x = (float(axial.x) * sqrt(3)) + (float(axial.y) * sqrt(3) / 2.);
	float y = (float(axial.y) * 3. / 2.);
	return vec2(x, y);
}

vec2 point_axial_center(vec2 xy){
	return axial_to_point(point_to_axial(xy));
}

ivec2 axial_to_evenr(ivec2 hex){
	int col = hex.x + ((hex.y + ((hex.y % 2))) / 2);
	int row = hex.y;
	return ivec2(col, row);
}

ivec2 point_to_evenr(vec2 xy){
	return axial_to_evenr(point_to_axial(xy));
}


struct HexCellData{
	vec2 center;
	ivec2 offsetCoords;
	vec2 UV;
	float distanceToEdge;
	float distanceToCenter;
	float distanceSmoothing;
};

float smoothstep01(HexCellData data, float threshold)
{
	return smoothstep(
		threshold - data.distanceSmoothing,
		threshold + data.distanceSmoothing,
		data.distanceToEdge);
}

float smoothstep10(HexCellData data, float threshold){
	return smoothstep(
		threshold + data.distanceSmoothing,
		threshold - data.distanceSmoothing,
		data.distanceToEdge);
}

float smoothstep_range(HexCellData data, float innerThreshold, float outerThreshold)
{
	return smoothstep01(data, innerThreshold) * smoothstep10(data, outerThreshold);
}

#define HEX_ANGLED_EDGE_VECTOR vec2(1, sqrt(3))

// Calculate hexagonal center-edge distance for point relative to
// the center in hex space. 0 at cell center and 1 at edges.
float point_to_edge_distance(vec2 p)
{
	// Reduce problem to one quadrant.
	p = abs(p);
	// Calculate distance to angled edge.
	float d = dot(p, normalize(HEX_ANGLED_EDGE_VECTOR));
	// Incorporate distance to vertical edge.
	d = max(d, p.x);
	// Double to increase range from center to edge to 0-1.
	return 2. * d;
}

HexCellData get_hex_cell_data(vec2 worldPositionXZ, int cell_scale)
{
	vec2 pos = worldPositionXZ / float(cell_scale);
	vec2 center = point_axial_center(pos);

	HexCellData d;
	d.offsetCoords = point_to_evenr(pos);
	d.center = center;
	d.distanceToEdge = point_to_edge_distance(center - pos) / float(cell_scale);
	d.distanceToCenter = distance(pos, center);
	d.distanceSmoothing = .001;
	return d;
}

bool is_highlighted(HexCellData data, sampler2D highlight_texture){
	vec4 color = texelFetch(highlight_texture, data.offsetCoords, 0);
	if(color.a > 0.){
		return true;
	}
	return false;
}
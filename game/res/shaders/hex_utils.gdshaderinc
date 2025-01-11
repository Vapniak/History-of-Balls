#define HEX_ANGLED_EDGE_VECTOR vec2(1, sqrt(3))

#define OUTER_TO_INNER 0.866025404
// HEIGHT CELL IN METERS, WE ARE USING POINTY TOP ORIENTATION
#define OUTER_RADIUS 2.

// TODO: rework mapping world to hex space, also when you get texture coords should be between 0 and 1 so find way to map hex coordinates to texture space
vec2 world_to_hex_space(vec2 p)
{
	return p * (1.0 / (2.0 * OUTER_RADIUS * OUTER_TO_INNER));
}

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

ivec2 get_axial_coords(vec2 xy){
	float x = xy.x;
	float q = (2./3. * x);
	float r = (-1./3. * x + sqrt(3)/3. * xy.y);
	return axial_round(vec2(q, r));
}

ivec2 axial_to_oddq(ivec2 hex){
	int col = hex.x;
	int row = hex.y + ((hex.x + (hex.x % 2)) / 2);
	return ivec2(col, row);
}

ivec2 get_oddq(vec2 xy){
	return axial_to_oddq(get_axial_coords(xy));
}


struct HexCellData{
	vec2 center;
	ivec2 offsetCoords;
	vec2 UV;
	float distanceToCenter;
	float distanceSmoothing;
};

float smoothstep01(HexCellData data, float threshold)
{
	return smoothstep(
		threshold - data.distanceSmoothing,
		threshold + data.distanceSmoothing,
		data.distanceToCenter);
}

float smoothstep10(HexCellData data, float threshold){
	return smoothstep(
		threshold + data.distanceSmoothing,
		threshold - data.distanceSmoothing,
		data.distanceToCenter);
}

float smoothstep_range(HexCellData data, float innerThreshold, float outerThreshold)
{
	return smoothstep01(data, innerThreshold) * smoothstep10(data, outerThreshold);
}


float hex_center_to_edge_distance(vec2 p)
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

// Calculate hex-based modulo to find position vector.
vec2 hex_modulo(vec2 p)
{
	return p - HEX_ANGLED_EDGE_VECTOR * floor(p / HEX_ANGLED_EDGE_VECTOR);
}

HexCellData get_hex_cell_data(vec2 worldPositionXZ)
{
	vec2 p = world_to_hex_space(worldPositionXZ);

	// Vectors from nearest two cell centers to position.
	vec2 gridOffset = HEX_ANGLED_EDGE_VECTOR * 0.5;
	vec2 a = hex_modulo(p) - gridOffset;
	vec2 b = hex_modulo(p - gridOffset) - gridOffset;
	bool aIsNearest = dot(a, a) < dot(b, b);

	vec2 vectorFromCenterToPosition = aIsNearest ? a : b;

	HexCellData d;
	d.center = p - vectorFromCenterToPosition;
	d.offsetCoords = get_oddq(worldPositionXZ);
	d.UV = vectorFromCenterToPosition + 0.5;
	d.distanceToCenter = hex_center_to_edge_distance(
		vectorFromCenterToPosition);
	d.distanceSmoothing = 0.01;
	return d;
}

bool is_highlighted(HexCellData data, sampler2D highlight_texture){
	vec4 color = texture(highlight_texture, vec2(data.offsetCoords));

	return false;
}
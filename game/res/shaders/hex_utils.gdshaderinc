#define HEX_ANGLED_EDGE_VECTOR vec2(1, sqrt(3))

#define OUTER_TO_INNER 0.866025404
#define OUTER_RADIUS 2.


vec2 world_to_hex_space(vec2 p)
{
	return p * (1.0 / (2.0 * OUTER_RADIUS * OUTER_TO_INNER));
}

struct HexCellData{
	vec2 center;
	vec2 offsetCoords;
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
	d.offsetCoords.x = d.center.x - (aIsNearest ? 0.5 : 0.0);
	d.offsetCoords.y = d.center.y / OUTER_TO_INNER;
	d.UV = vectorFromCenterToPosition + 0.5;
	d.distanceToCenter = hex_center_to_edge_distance(
		vectorFromCenterToPosition);
	d.distanceSmoothing = 0.01;
	return d;
}

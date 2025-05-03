shader_type spatial;
render_mode blend_mix, unshaded, cull_disabled, depth_prepass_alpha;

// Texture variety system
uniform sampler2DArray cloud_textures;
uniform float texture_variation = 4.0; // Number of textures in array
uniform sampler2D noise_texture;
uniform float distortion_strength = 0.1;

varying float tex_id;

void vertex() {
    // Random texture selection per instance
    tex_id = mod(float(INSTANCE_ID), texture_variation);
}

void fragment() {
    // Apply noise distortion for organic shapes
    vec2 noise = texture(noise_texture, UV * 2.0).rg;
    vec3 distorted_uv = vec3(UV.xy + (noise - 0.5) * distortion_strength, tex_id);

    // Sample from texture array
    vec4 cloud = texture(cloud_textures, distorted_uv);

    // Soft edges
    float alpha = smoothstep(0.3, 0.8, cloud.a);
    ALBEDO = cloud.rgb;
    ALPHA = alpha * COLOR.a; // Respect particle system alpha
}
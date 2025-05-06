shader_type spatial;
render_mode blend_mix, unshaded, cull_disabled, diffuse_burley, depth_prepass_alpha;

// Base material properties
uniform vec4 albedo : source_color;
uniform sampler2D texture_albedo : source_color, filter_linear_mipmap, repeat_enable;

uniform float roughness : hint_range(0.0, 1.0);
uniform sampler2D texture_metallic : hint_default_white, filter_linear_mipmap, repeat_enable;
uniform vec4 metallic_texture_channel;
uniform sampler2D texture_roughness : hint_roughness_r, filter_linear_mipmap, repeat_enable;

uniform float specular : hint_range(0.0, 1.0, 0.01);
uniform float metallic : hint_range(0.0, 1.0, 0.01);

// Normal mapping
uniform sampler2D texture_normal : hint_normal, filter_linear_mipmap, repeat_enable;
uniform float normal_scale : hint_range(0.0, 2.0) = 1.0;

// Wind sway parameters (now in global space)
uniform bool enable_wind = false;
uniform int wind_type = 0; // 0: Smooth, 1: Gusty, 2: Wave, 3: Directional
uniform float wind_strength : hint_range(0.0, 2.0) = 0.9;
uniform float wind_speed : hint_range(0.0, 5.0) = 1.0;
uniform float wind_frequency : hint_range(0.0, 10.0) = 2.0;
uniform vec2 wind_direction = vec2(1.0, 0.0); // Global direction (XZ plane)

// Distance fade settings
uniform bool enable_fade = false;
uniform float fade_start_distance : hint_range(0.0, 200.0) = 50.0;
uniform float fade_length : hint_range(1.0, 50.0) = 20.0;

void vertex() {
    if (enable_wind) {
        // Get vertex in world space
        vec4 world_pos = MODEL_MATRIX * vec4(VERTEX, 1.0);

        float sway_intensity = 1.0 - COLOR.r;
        float strength = wind_strength;

        // Apply wind in world space
        if (wind_type == 0) { // Smooth wind
            world_pos.y += sin(world_pos.x * wind_frequency + TIME * wind_speed) * strength * sway_intensity;
        } else if (wind_type == 1) { // Gusty wind
            float turbulence = sin(TIME * wind_speed * 0.7) * cos(TIME * wind_speed * 1.3);
            world_pos.y += sin(world_pos.x * wind_frequency + TIME * wind_speed) * turbulence * strength * sway_intensity;
        } else if (wind_type == 2) { // Wave-like wind
            world_pos.y += sin(world_pos.x * wind_frequency + TIME * wind_speed) * cos(world_pos.z * wind_frequency + TIME * wind_speed) * strength * sway_intensity;
        } else if (wind_type == 3) { // Directional wind
            world_pos.xz += wind_direction * sin(TIME * wind_speed) * strength * sway_intensity;
        }

        // Convert back to local space
        VERTEX = (inverse(MODEL_MATRIX) * world_pos).xyz;
    }
}

void fragment() {
    vec2 base_uv = UV;

    // Sample textures
    vec4 albedo_tex = texture(texture_albedo, base_uv);
    float metallic_tex = dot(texture(texture_metallic, base_uv), metallic_texture_channel);
    float roughness_tex = texture(texture_roughness, base_uv).r;

    // Compute base material properties
    ALBEDO = albedo.rgb * albedo_tex.rgb;
    METALLIC = metallic_tex * metallic;
    SPECULAR = specular;
    ROUGHNESS = roughness_tex * roughness;

    // Normal mapping
    vec3 normal_map = texture(texture_normal, base_uv).rgb * 2.0 - 1.0;
    normal_map.xy *= normal_scale;
    NORMAL_MAP = normal_map;

    // Compute fade effect only if enabled
    float fade = 1.0;
    if (enable_fade) {
        float dist = length((MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz);
        fade = clamp(1.0 - (dist - fade_start_distance) / fade_length, 0.0, 1.0);
    }

    // Apply fading to alpha
    ALPHA = albedo.a * albedo_tex.a * fade;
}
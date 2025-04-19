shader_type spatial;
//specular_schlick_ggx for shaded in place of unshaded
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

// Wind sway parameters
uniform bool enable_wind = false;
uniform int wind_type = 0; // 0: Smooth, 1: Gusty, 2: Wave, 3: Directional

uniform float wind_strength : hint_range(0.0, 2.0) = 0.9;
uniform float wind_speed : hint_range(0.0, 5.0) = 1.0;
uniform float wind_frequency : hint_range(0.0, 10.0) = 2.0;
uniform vec2 wind_direction = vec2(1.0, 0.0);

// Distance fade settings
uniform bool enable_fade = false; // Toggle for enabling/disabling fade effect
uniform float fade_start_distance : hint_range(0.0, 200.0) = 50.0; // Distance at which fading starts
uniform float fade_length : hint_range(1.0, 50.0) = 20.0; // Distance over which fading occurs

void vertex() {
    if (enable_wind) {
        float sway_intensity = 1.0 - COLOR.r; // Control wind intensity with vertex color
        if (wind_type == 0) { // Smooth wind
            VERTEX.y += sin(VERTEX.x * wind_frequency + TIME * wind_speed) * wind_strength * sway_intensity;
        } else if (wind_type == 1) { // Gusty wind
            float turbulence = sin(TIME * wind_speed * 0.7) * cos(TIME * wind_speed * 1.3);
            VERTEX.y += sin(VERTEX.x * wind_frequency + TIME * wind_speed) * turbulence * wind_strength * sway_intensity;
        } else if (wind_type == 2) { // Wave-like wind
            VERTEX.y += sin(VERTEX.x * wind_frequency + TIME * wind_speed) * cos(VERTEX.z * wind_frequency + TIME * wind_speed) * wind_strength * sway_intensity;
        } else if (wind_type == 3) { // Directional wind
            VERTEX.xz += wind_direction * sin(TIME * wind_speed) * wind_strength * sway_intensity;
        }
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
    float fade = 1.0; // Default fully visible
    if (enable_fade) {
        float dist = length(VERTEX);
        fade = clamp(1.0 - (dist - fade_start_distance) / fade_length, 0.0, 1.0);
    }

    // Apply fading to alpha
    ALPHA = albedo.a * albedo_tex.a * fade;
}

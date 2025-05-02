shader_type canvas_item;

uniform sampler2D[16] texs;
uniform vec4[16] speeds;
uniform int texCount;

varying vec2 WORLD_SPACE;

void vertex() {
	WORLD_SPACE = (MODEL_MATRIX * vec4(VERTEX, 0, 1)).xy;
}

vec4 hsv(vec4 col) {
	float h = mod(col.x, 6);
	float s = col.y;
	float v = col.z;
	float a = col.w;

	float r, g, b;
	r = clamp(abs(h-3.0)-1.0, 0, 1);
	g = clamp(2.0-abs(h-2.0), 0, 1);
	b = clamp(2.0-abs(h-4.0), 0, 1);

	return vec4(mix(vec3(1, 1, 1), vec3(r, g, b), s) * v, a);
}

void fragment() {
	float v = 0.0;
	float w = 0.0;
	float bAlpha = 0.0;
	for (int i = 0; i < texCount; i++) {
		v += (0.5 - abs(
			texture(texs[0],
				mod((floor(WORLD_SPACE.xy / 1.0) * 1.0 + speeds[i].xy * TIME) / 128.0 / speeds[i].w, 1)
			).x - 0.5)) * speeds[i].z;
	}
	v = floor(v * 8.0) / 32.0;
	COLOR = hsv(vec4(v + (WORLD_SPACE.x + WORLD_SPACE.y) / 16.0 + TIME * 10.0, 0.5 - v, 1.0, texture(TEXTURE, UV).a));
	//COLOR.a *= max(0, length(SCREEN_UV - 0.5) - 0.5) * 2.0;

	//COLOR = vec4(WORLD_SPACE, 1, 1);
}


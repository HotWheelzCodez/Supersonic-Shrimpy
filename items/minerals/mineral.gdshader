shader_type canvas_item;

uniform sampler2D[16] texs;
uniform vec4[16] speeds;
uniform int texCount;

uniform float alpha;
uniform float overlay;

varying vec2 WORLD_SPACE;


#define tex texture

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

void vertex() {
	WORLD_SPACE = (MODEL_MATRIX * floor(vec4(VERTEX, 0, 1))).xy;
}

void fragment() {
	float v = 0.0;
	float w = 0.0;
	vec2 pixel = TEXTURE_PIXEL_SIZE;
	vec2 pX = vec2(pixel.x, 0);
	vec2 pY = vec2(0, pixel.y);


	float border = max(
	max(tex(TEXTURE, UV+pX).a,
		tex(TEXTURE, UV-pX).a),
	max(tex(TEXTURE, UV+pY).a,
		tex(TEXTURE, UV-pY).a));

	COLOR = tex(TEXTURE, UV);

	if (border > 0.0 && (COLOR.a <= 0.0 || overlay > 0.0)) {
		for (int i = 0; i < texCount; i++) {
			v += (0.5 - abs(
				texture(texs[0],
					mod((floor(WORLD_SPACE.xy / 1.0) * 1.0 + speeds[i].xy * TIME) / 128.0 / speeds[i].w, 1)
				).x - 0.5)) * speeds[i].z;
		}
		v = floor(v * 8.0) / 32.0;
		COLOR = mix(
			COLOR,
			hsv(vec4(v + (WORLD_SPACE.x + WORLD_SPACE.y) / 16.0 + TIME * 10.0, 0.5 - v, 1.0, 1.0)),
			COLOR.a > 0.0 ? overlay : alpha
		);
	}

	//COLOR.a *= max(0, length(SCREEN_UV - 0.5) - 0.5) * 2.0;

	//COLOR = vec4(WORLD_SPACE, 1, 1);
}

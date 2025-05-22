shader_type canvas_item;

uniform sampler2D[16] texs;
uniform vec4[16] speeds;
uniform int texCount;

uniform float alpha;
uniform vec2 sliceN1;
uniform vec2 sliceN2;
uniform vec2 sliceP;
uniform float dist;

varying vec2 WORLD_SPACE;

void vertex() {
	VERTEX += VERTEX / abs(max(VERTEX.x, VERTEX.y)) * dist / TEXTURE_PIXEL_SIZE * 2.0;
	UV = VERTEX * TEXTURE_PIXEL_SIZE + 0.5;
	WORLD_SPACE = (MODEL_MATRIX * vec4(VERTEX, 0, 1)).xy;
}

int splitSide(vec2 orig, vec2 norm, vec2 uv) {
	return dot(uv - orig, norm) > 0.0 ? 1 : 0;
}

int part(vec2 uv) {
	return splitSide(sliceP, sliceN1, uv) + splitSide(sliceP, sliceN2, uv) * 2;
}

vec4 tex(sampler2D texturee, vec2 ouv) {
	vec2 s = vec2(textureSize(texturee, 0));
	ouv = floor(ouv * s) / s;
	int p = part(ouv);
	vec2 uv = ouv + floor((
		- (normalize(sliceN1) * dist * ((p & 1) != 0 ? 1.0 : -1.0))
		- (normalize(sliceN2) * dist * ((p & 2) != 0 ? 1.0 : -1.0))
		) * s) / s;
	bool same = p == part(uv);

	return same ? texture(texturee, uv) : vec4(0, 0, 0, 0);
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
	vec2 pixel = TEXTURE_PIXEL_SIZE;
	vec2 pX = vec2(pixel.x, 0);
	vec2 pY = vec2(0, pixel.y);


	float border = max(
	max(tex(TEXTURE, UV+pX).a,
		tex(TEXTURE, UV-pX).a),
	max(tex(TEXTURE, UV+pY).a,
		tex(TEXTURE, UV-pY).a));

	COLOR = tex(TEXTURE, UV);

	if (border > 0.0 && COLOR.a <= 0.0) {
		for (int i = 0; i < texCount; i++) {
			v += (0.5 - abs(
				texture(texs[0],
					mod((WORLD_SPACE + speeds[i].xy * TIME) / 128.0 / speeds[i].w, 1)
				).x - 0.5)) * speeds[i].z;
		}
		v = floor(v * 8.0) / 32.0;
		COLOR = hsv(vec4(v + (WORLD_SPACE.x + WORLD_SPACE.y) / 16.0 + TIME * 10.0, 0.5 - v, 1.0, 1.0));
	}
	COLOR.a *= alpha;

	//COLOR.a *= max(0, length(SCREEN_UV - 0.5) - 0.5) * 2.0;

	//COLOR = vec4(WORLD_SPACE, 1, 1);
}

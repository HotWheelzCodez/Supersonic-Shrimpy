#pragma language glsl3

/*
uniform sampler2D[16] texs;
uniform vec4[16] speeds;
uniform int texCount;
*/

vec4 effect(vec4 color, Image texture, vec2 texture_coords, vec2 screen_coords) {
	/*
	float v = 0.0;
	float w = 0.0;
	for (int i = 0; i < texCount; i++) {
		v += (0.5 - abs(
			texture(texs[0],
				mod((floor(WORLD_SPACE.xy / 1.0) * 1.0 + speeds[i].xy * TIME) / 128.0 / speeds[i].w, 1)
			).x - 0.5)) * speeds[i].z;
	}
	v = floor(v * 4.0) / 4.0;
	COLOR = mix(vec4(0.0, 0.7, 1.0, 0.0), vec4(1.0, 1.0, 1.0, 0.2), v);
	//COLOR.a *= max(0, length(SCREEN_UV - 0.5) - 0.5) * 2.0;
	*/

	return vec4(0, 0, 1, 1);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}

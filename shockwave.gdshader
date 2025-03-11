shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE: hint_screen_texture;
uniform vec3[16] centers;

varying vec3 WORLD_SPACE;
void vertex() {
	WORLD_SPACE = (MODEL_MATRIX * vec4(VERTEX, 0, 1)).xyz;
}

void fragment() {
	vec2 uv = SCREEN_UV / SCREEN_PIXEL_SIZE;
	for (int i = 0; i < 16; i++) {
		vec2 dir = normalize(WORLD_SPACE.xy - centers[i].xy);
		float x = length(WORLD_SPACE.xy - centers[i].xy);
		float h = centers[i].z * 32.0;

		float v = pow(2, -pow(x-h, 2)) * (x-h) * 20.0 / h;

		uv -= dir * v * v;
	}

	COLOR = texture(SCREEN_TEXTURE, uv * SCREEN_PIXEL_SIZE);
	//COLOR.r = -v;
	//COLOR.g = v;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}

shader_type canvas_item;

uniform vec2 origin;
uniform vec2 normal;
uniform float scale = 1;

varying vec2 WORLD_SPACE;

void vertex() {
	WORLD_SPACE = (MODEL_MATRIX * floor(vec4(VERTEX, 0, 1))).xy;
}

void fragment() {
	vec2 dif = floor(WORLD_SPACE) - origin;
	float y = dot(dif, normal);
	float x = dot(dif, vec2(-normal.y, normal.x));
	if (y < min((x*x)/64.0/scale-2.0*scale, 0.0)) {
		discard;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}

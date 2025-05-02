shader_type canvas_item;

uniform vec2 origin;
uniform vec2 normal;

varying vec2 WORLD_SPACE;

void vertex() {
	WORLD_SPACE = (MODEL_MATRIX * vec4(VERTEX, 0, 1)).xy;
}

void fragment() {
	if (dot(WORLD_SPACE - origin, normal) < 0.0) {
		discard;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}

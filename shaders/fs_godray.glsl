#version 330

in vec2 fragmentPosition;	      // fragment position in screen space
in vec2 uv;						  // interpolated texture coordinates

uniform sampler2D uScreenTexture; // input texture (1st pass render target)
uniform vec2 uLightPositionScreen;

out vec3 outputColor;

void main()
{
	outputColor = texture(uScreenTexture, uv).xyz;
	if(uLightPositionScreen.x < 0 || uLightPositionScreen.x > 1 || uLightPositionScreen.y < 0 || uLightPositionScreen.y > 1){
		return;
	}

	float decay = 0.98;
	float density = 3;

	vec2 toFragmentVec = uv - uLightPositionScreen;
	toFragmentVec /= (100 * density);
	vec2 mapUv = uv;
	float  illuminationDecay = 1;
	for(int i = 0; i < 100; i++){
		mapUv -= toFragmentVec;
		vec3 t = texture(uScreenTexture, mapUv).xyz;
		outputColor += t * illuminationDecay * vec3(1f, 0.5F, 0.1F);
		illuminationDecay *= decay;
	}
}
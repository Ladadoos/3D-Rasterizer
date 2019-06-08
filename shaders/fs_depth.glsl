#version 330

in vec4 fragmentPosition;
in vec2 uv;

uniform sampler2D uTextureMap;	 // diffuse texture
uniform vec3 uLightPosition;

void main()
{
	if(texture(uTextureMap, uv).a < 0.5){
		discard;
	}

	vec3 fromLight = fragmentPosition.xyz - uLightPosition;
	float dist = length(fromLight) / 1000;
	gl_FragDepth = dist;
}
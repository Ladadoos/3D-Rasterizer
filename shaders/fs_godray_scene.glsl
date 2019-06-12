#version 330
 
const int LightCount = 1; //Number of lights in our scene

in vec2 uv;						// interpolated texture coordinates

uniform sampler2D uTextureMap;	 // diffuse texture
uniform vec3 uLightColor[LightCount];
uniform int uIsLightTarget;

layout (location = 0) out vec4 outputFragColor;

void main()
{
	vec4 textureColor = texture(uTextureMap, uv);
	if(textureColor.a < 0.5){
		discard;
	}

	if(uIsLightTarget != -1){
		outputFragColor = vec4(uLightColor[uIsLightTarget], 1);
	}
}
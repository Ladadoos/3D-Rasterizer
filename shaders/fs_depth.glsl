/*#version 330

in vec4 fragmentPosition;

uniform vec3 uLightPosition;

out float FragColor;

void main()
{
	vec3 fromLight = fragmentPosition.xyz - uLightPosition;
	float dist = length(fromLight) / 1000;
	FragColor = dist;
}*/


#version 330

in vec4 fragmentPosition;

uniform vec3 uLightPosition;

void main()
{
	vec3 fromLight = fragmentPosition.xyz - uLightPosition;
	float dist = length(fromLight) / 1000;
	gl_FragDepth = dist;
}
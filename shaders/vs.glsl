#version 330
 
// shader input
in vec2 vUV;				// vertex uv coordinate
in vec3 vNormal;			// untransformed vertex normal
in vec3 vPosition;			// untransformed vertex position

// shader output
out vec4 normal;			// transformed vertex normal
out vec2 uv;		
out vec4 pos;
out vec4 posLightSpace;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform mat4 lightSpacematrix;

// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = projection * view * model * vec4(vPosition, 1.0);

	// forward normal and uv coordinate; will be interpolated over triangle
	normal = model * vec4( vNormal, 0.0f );
	pos = model * vec4( vPosition, 1.0f );
	posLightSpace = lightSpacematrix * vec4(pos.xyz, 1.0f);
	uv = vUV;
}
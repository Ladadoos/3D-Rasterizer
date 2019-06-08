#version 330

in vec3 iPosition; 
in vec2 iUV;				// vertex uv coordinate

out vec4 fragmentPosition;
out vec2 uv;

uniform mat4 uModel; // model matrix
uniform mat4 uViewProjection; // view * projection of light
 
void main()
{
	uv = iUV;
	fragmentPosition = uModel * vec4(iPosition, 1);
	gl_Position = uViewProjection * fragmentPosition;
}
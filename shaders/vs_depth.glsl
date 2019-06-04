#version 330

in vec3 iPosition; 

out vec4 fragmentPosition;

uniform mat4 uModel; // model matrix
uniform mat4 uViewProjection; // view * projection of light
 
void main()
{
	fragmentPosition = uModel * vec4(iPosition, 1);
	gl_Position = uViewProjection * fragmentPosition;
}
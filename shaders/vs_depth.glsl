#version 330

in vec3 iPosition; 

uniform mat4 uModel; // model matrix
uniform mat4 uViewProjection; // view * projection of light
 
void main()
{
	gl_Position = uViewProjection * uModel * vec4(iPosition, 1);
}
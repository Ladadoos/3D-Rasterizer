#version 330
in vec3 vPosition; // untransformed vertex position

uniform mat4 model; // model matrix
uniform mat4 viewProjection; // view * projection of light
 
void main()
{
	gl_Position = viewProjection * model * vec4(vPosition, 1.0);
}
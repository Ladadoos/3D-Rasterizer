#version 330
 
in vec3 iPosition;			// untransformed vertex position
in vec2 iUV;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec2 uv;

void main()
{
	uv = iUV;
	gl_Position = uProjection * uView * uModel * vec4(iPosition, 1);
}
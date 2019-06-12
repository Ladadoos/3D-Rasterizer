#version 330

in vec2 iUV;				
in vec3 iPosition;			

out vec2 uv;				
out vec2 fragmentPosition;				

// vertex shader
void main()
{
	uv = iUV;
	fragmentPosition = vec2(iPosition) * 0.5 + vec2(0.5, 0.5);
	gl_Position = vec4(iPosition, 1);
}
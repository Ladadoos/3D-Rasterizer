#version 330 core

in vec3 iPosition;

uniform mat4 viewProjection;

out vec3 textureCoords;

void main()
{
    textureCoords = iPosition;
    gl_Position = viewProjection * vec4(iPosition, 1);
}  
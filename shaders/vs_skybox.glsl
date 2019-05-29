#version 330 core
in vec3 vPosition;
out vec3 textureCoords;
uniform mat4 viewProjection;

void main()
{
    textureCoords = vPosition;
    gl_Position = viewProjection * vec4(vPosition, 1.0);
}  
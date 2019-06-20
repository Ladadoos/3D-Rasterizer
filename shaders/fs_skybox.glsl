#version 330 core

in vec3 textureCoords;

uniform samplerCube uSkyboxCubeMap;

layout (location = 0) out vec4 outputColor;

void main()
{    
    outputColor = vec4(texture(uSkyboxCubeMap, textureCoords).rgb, 0);
}
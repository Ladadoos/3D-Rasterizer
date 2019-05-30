#version 330 core

in vec3 textureCoords;

uniform samplerCube uSkyboxCubeMap;

out vec4 outputColor;

void main()
{    
    outputColor = texture(uSkyboxCubeMap, textureCoords);
}
#version 330 core
out vec4 outputColor;
in vec3 textureCoords;
uniform samplerCube skyboxCubeMap;

void main()
{    
    outputColor = texture(skyboxCubeMap, textureCoords);
}
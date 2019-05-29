#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 pos;    
in vec4 posLightSpace;

uniform sampler2D pixels;		// texture sampler
uniform sampler2D depthpixels;  // depth texture
uniform vec3 cameraPosition;
uniform vec3 ambientLightColor;
uniform vec3 lightColor;
uniform vec3 lightPosition;

// shader output
out vec4 outputColor;

const float ambientStrenght = 0.9;
const float specularStrength = 0.8;
const float shininess = 64;

float ShadowCalculation(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir)
{
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
   
    //transform to 0,1 range
    projCoords = projCoords * 0.5 + 0.5;

    float closestDepth = texture(depthpixels, projCoords.xy).x; 

    // get depth of current fragment from light's perspective
    float currentDepth = projCoords.z;

	float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);  
	float shadow = currentDepth - bias > closestDepth  ? 1.0 : 0.0;  

	return shadow;
}

// fragment shader
void main()
{
   vec3 ambient = ambientStrenght * ambientLightColor;

   vec3 norm = normalize(normal.xyz);
   vec3 toLightDir = lightPosition - pos.xyz;
   float toLightDist = length(toLightDir);
   float lightAttenuation = 1 / (toLightDist * toLightDist) * 10175;
   toLightDir *= (1 / toLightDist); //normalize
   float diff = max(dot(norm.xyz, toLightDir), 0);
   vec3 diffuse = diff * lightColor;

   vec3 toCameraDir = normalize(cameraPosition - pos.xyz);
   vec3 reflectDir = reflect(-toLightDir, norm);
   float spec = pow(max(dot(-toCameraDir, reflectDir), 0), shininess);
   vec3 specular = specularStrength * spec * lightColor;

   float shadow = ShadowCalculation(posLightSpace, norm, toLightDir);
   vec3 lighting = (ambient + (1 - shadow) * (diffuse + specular)) * lightAttenuation;

   outputColor = texture( pixels, uv ) * vec4(lighting, 1);
}
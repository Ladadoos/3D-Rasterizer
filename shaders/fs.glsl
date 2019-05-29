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

float GetShadowFactor(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir)
{
    vec3 projectionCoords = fragPosLightSpace.xyz / fragPosLightSpace.w; //perspective division
    projectionCoords = projectionCoords * 0.5 + 0.5;
	if(projectionCoords.z > 1.0){ // check if fragment is outside light frustum
		return 0;
	}     
    float currentDepth = projectionCoords.z;
	float bias = clamp(0.005 * tan(acos(dot(normal, lightDir))), 0, 0.01); //dynamic bias based on slope
	vec2 texelSize = 1.0 / textureSize(depthpixels, 0);
	float shadow = 0;
	for(int x = -1; x <= 1; ++x)
	{
		for(int y = -1; y <= 1; ++y)
		{
			float depth = texture(depthpixels, projectionCoords.xy + vec2(x, y) * texelSize).x; 
			shadow += currentDepth - bias > depth ? 1 : 0;        
		}    
	}
	shadow /= 9;
	return shadow;
}

// fragment shader
void main()
{
   vec3 ambient = ambientStrenght * ambientLightColor;

   vec3 norm = normalize(normal.xyz);
   vec3 toLightDir = lightPosition - pos.xyz;
   float toLightDist = length(toLightDir);
   float lightAttenuation = 1.0 / (toLightDist * toLightDist) * 4500;
   toLightDir *= (1.0 / toLightDist); //normalize
   float diff = max(dot(norm.xyz, toLightDir), 0);
   vec3 diffuse = diff * lightColor;

   vec3 toCameraDir = normalize(cameraPosition - pos.xyz);
   vec3 reflectDir = reflect(-toLightDir, norm);
   float spec = pow(max(dot(-toCameraDir, reflectDir), 0), shininess);
   vec3 specular = specularStrength * spec * lightColor;

   float shadow = GetShadowFactor(posLightSpace, norm, toLightDir);
   vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * lightAttenuation;

   outputColor = texture( pixels, uv ) * vec4(lighting, 1);
}
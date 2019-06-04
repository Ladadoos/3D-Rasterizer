#version 330
 
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 fragmentPosition;       
in vec3 lightposition;
in vec3 cameraposition;
in mat3 tbnMatrix;

uniform int uUseNormalMap;
uniform sampler2D uTextureMap;	 // diffuse texture
uniform samplerCube uDepthCube;     // depth texture
uniform sampler2D uNormalMap;    // normal texture
uniform vec3 uAmbientLightColor;
uniform vec3 uLightColor;

out vec4 outputColor;

const float ambientStrenght = 0.9;
const float specularStrength = 0.8;
const float shininess = 16;
const float lightBrightness = 8500;

float GetShadowFactor()
{
	vec3 fragToLight = fragmentPosition.xyz - lightposition;
	float closestDepth = texture(uDepthCube, fragToLight).r;
	closestDepth *= 1000;
	float currentDepth = length(fragToLight);
	float bias = 0.005;
	return currentDepth - bias > closestDepth ? 1 : 0;  
}

void main()
{
	vec3 norm = normal.xyz;
	if(uUseNormalMap == 1){
		vec3 normalFromMap = texture(uNormalMap, uv).rgb;
		normalFromMap = normalFromMap * 2.0 - 1.0; //convert to range -1..1
		normalFromMap = tbnMatrix * normalFromMap; //convert coordinate system
		norm = normalize(normalFromMap.xyz);
	}

	vec3 ambient = ambientStrenght * uAmbientLightColor;
	vec3 toLightDir = lightposition - fragmentPosition.xyz;
	float toLightDist = length(toLightDir);
	float lightAttenuation = 1.0 / (toLightDist * toLightDist);
	toLightDir *= (1.0 / toLightDist); //normalize
	float diff = max(dot(norm.xyz, toLightDir), 0);
	vec3 diffuse = diff * uLightColor;

	vec3 toCameraDir = normalize(cameraposition - fragmentPosition.xyz);
	vec3 reflectDir = reflect(toLightDir, norm);
	float spec = pow(max(dot(-toCameraDir, reflectDir), 0), shininess);
	vec3 specular = specularStrength * spec * uLightColor;

	float shadowFactor = GetShadowFactor();
	vec3 lighting = (ambient + (1.0 - shadowFactor) * (diffuse + specular)) * lightAttenuation * lightBrightness;
	outputColor = texture(uTextureMap, uv) * vec4(lighting, 1);
}
#version 330
 
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 fragmentPosition;       
in vec4 posLightSpace;
in vec3 lightposition;
in vec3 cameraposition;
in mat3 tbnMatrix;

uniform int uUseNormalMap;
uniform sampler2D uTextureMap;	 // diffuse texture
uniform sampler2D uDepthMap;     // depth texture
uniform sampler2D uNormalMap;    // normal texture
uniform vec3 uAmbientLightColor;
uniform vec3 uLightColor;

out vec4 outputColor;

const float ambientStrenght = 0.9;
const float specularStrength = 0.8;
const float shininess = 16;
const float lightBrightness = 8500;

float GetShadowFactor(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir)
{
    vec3 projectionCoords = fragPosLightSpace.xyz / fragPosLightSpace.w; //perspective division
    projectionCoords = projectionCoords * 0.5 + 0.5; // range 0..1
	if(projectionCoords.z > 1.0){ // check if fragment is outside light frustum
		return 0;
	}     
    float currentDepth = projectionCoords.z;
	float bias = clamp(0.005 * tan(acos(dot(normal, lightDir))), 0, 0.01); //dynamic bias based on slope
	vec2 texelSize = 1.0 / textureSize(uDepthMap, 0);
	float shadow = 0;
	for(int x = -1; x <= 1; ++x) //check surrounding depthmap texels and take average for smooth shadows
	{
		for(int y = -1; y <= 1; ++y)
		{
			float depth = texture(uDepthMap, projectionCoords.xy + vec2(x, y) * texelSize).x; 
			shadow += currentDepth - bias > depth ? 1 : 0;        
		}    
	}
	return (shadow /= 9);
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

	float shadowFactor = GetShadowFactor(posLightSpace, norm, toLightDir);
	vec3 lighting = (ambient + (1.0 - shadowFactor) * (diffuse + specular)) * lightAttenuation * lightBrightness;
	outputColor = texture(uTextureMap, uv) * vec4(lighting, 1);
}
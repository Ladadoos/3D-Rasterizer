#version 330
 
const int PointLightsCount = 2;
const int SpotLightsCount = 1;

in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 fragmentPosition;       
in mat3 tbnMatrix;
flat in int useNormalMap;

uniform sampler2D uTextureMap;	 // diffuse texture
uniform sampler2D uNormalMap;    // normal texture

uniform samplerCube uDepthCube[PointLightsCount];     // depth texture
uniform vec3 uPointLightColor[PointLightsCount];
uniform vec3 uPointLightPosition[PointLightsCount];
uniform float uPointLightBrightness[PointLightsCount];

uniform samplerCube uDepthMap[SpotLightsCount];     // depth texture
uniform vec3 uSpotLightColor[SpotLightsCount];
uniform vec3 uSpotLightPosition[SpotLightsCount];
uniform float uSpotLightBrightness[SpotLightsCount];
uniform vec3 uSpotLightDirection[SpotLightsCount];
uniform float uSpotLightCutoff[SpotLightsCount];

uniform vec3 uCameraPosition;
uniform float uShininess;
uniform vec3 uAmbientLightColor;
uniform int uIsLightTarget;

layout (location = 0) out vec4 outputFragColor;
layout (location = 1) out vec4 outputBrightnessColor;

const float ambientStrenght = 0.9;
const float specularStrength = 0.8;

float GetPointLightShadow(vec3 norm, int lightIndex, vec3 toLightDirection)
{
	vec3 fragToLight = fragmentPosition.xyz - uPointLightPosition[lightIndex];
	float currentDepth = length(fragToLight);
	float bias = max(0.025 * (1.0 - dot(norm, toLightDirection)), 0.2);  
	float accumulatedShadow = 0;
	for(int x = -1; x <= 1; x++){
		for(int y = -1; y <= 1; y++){
			for(int z = -1; z <= 1; z++){
				vec3 delta = vec3(x, y, z);
				float closestDepth = texture(uDepthCube[lightIndex], fragToLight + delta).r;
				closestDepth *= 1000;
				accumulatedShadow += currentDepth - bias > closestDepth ? 1 : 0;
			}
		}
	}
	//float bias = clamp(0.05 * tan(acos(dot(norm, toLightDirection))), 0, 0.1);
	return (accumulatedShadow / 27);
}

float GetSpotLightShadow(vec3 norm, int lightIndex, vec3 toLightDirection)
{
	vec3 fragToLight = fragmentPosition.xyz - uSpotLightPosition[lightIndex];
	float currentDepth = length(fragToLight);
	float bias = max(0.025 * (1.0 - dot(norm, toLightDirection)), 0.2);  
	/*float accumulatedShadow = 0;
	for(int x = -1; x <= 1; x++){
		for(int y = -1; y <= 1; y++){
			for(int z = -1; z <= 1; z++){
				vec3 delta = vec3(x, y, z);
				float closestDepth = texture(uDepthCube[lightIndex], fragToLight + delta).r;
				closestDepth *= 1000;
				accumulatedShadow += currentDepth - bias > closestDepth ? 1 : 0;
			}
		}
	}*/
	float closestDepth = texture(uDepthMap[lightIndex], fragToLight).r;
	closestDepth *= 1000;
	return currentDepth - bias > closestDepth ? 1 : 0;
	//float bias = clamp(0.05 * tan(acos(dot(norm, toLightDirection))), 0, 0.1);
	//return (accumulatedShadow / 27);
}

void CalculateBrightness(){
    if(outputFragColor.r > 0.9 || outputFragColor.b > 0.9 || outputFragColor.g > 0.9){
        outputBrightnessColor = vec4(outputFragColor.rgb, 1.0);
    }else{
        outputBrightnessColor = vec4(0.0, 0.0, 0.0, 1.0);
	}
}

void main()
{
	vec4 textureColor = texture(uTextureMap, uv);
	if(textureColor.a < 0.5){
		discard;
	}

	if(uIsLightTarget != -1){
		if(uIsLightTarget >= 100){
			outputFragColor = vec4(uSpotLightColor[uIsLightTarget - 100], 1);
		}else{
			outputFragColor = vec4(uPointLightColor[uIsLightTarget], 1);
		}	
		CalculateBrightness();
		return;
	}

	vec3 norm = normal.xyz;
	if(useNormalMap == 1){
		vec3 normalFromMap = texture(uNormalMap, uv).rgb;
		normalFromMap = normalFromMap * 2.0 - 1.0; //convert to range -1..1
		normalFromMap = tbnMatrix * normalFromMap; //convert coordinate system
		norm = normalize(normalFromMap.xyz);
	}

	vec3 ambient = ambientStrenght * uAmbientLightColor;
	vec3 toCameraDir = normalize(uCameraPosition - fragmentPosition.xyz);

	vec3 lighting = ambient;
	for(int i = 0; i < PointLightsCount; i++){
		vec3 toLightDirection = uPointLightPosition[i] - fragmentPosition.xyz;
		float toLightDistance = length(toLightDirection);
		float lightAttenuation = 1.0 / (toLightDistance * toLightDistance);
		toLightDirection *= (1.0 / toLightDistance); //normalize
		float diff = max(dot(norm.xyz, toLightDirection), 0);
		vec3 diffuse = diff * uPointLightColor[i];

		vec3 reflectDir = reflect(toLightDirection, norm);
		float spec = pow(max(dot(-toCameraDir, reflectDir), 0), uShininess);
		vec3 specular = specularStrength * spec * uPointLightColor[i];

		float shadowFactor = GetPointLightShadow(norm, i, toLightDirection);
		lighting += (1.0 - shadowFactor) * (diffuse + specular) * lightAttenuation * uPointLightBrightness[i];
	}

	for(int i = 0; i < SpotLightsCount; i++){
		vec3 toLightDirection = uSpotLightPosition[i] - fragmentPosition.xyz;
		float toLightDistance = length(toLightDirection);
		float lightAttenuation = 1.0 / (toLightDistance * toLightDistance);
		toLightDirection *= (1.0 / toLightDistance); //normalize
		float diff = max(dot(norm.xyz, toLightDirection), 0);
		vec3 diffuse = diff * uSpotLightColor[i];

		vec3 reflectDir = reflect(toLightDirection, norm);
		float spec = pow(max(dot(-toCameraDir, reflectDir), 0), uShininess);
		vec3 specular = specularStrength * spec * uSpotLightColor[i];

		float theta = dot(toLightDirection, normalize(uSpotLightDirection[i]));
		if(theta > uSpotLightCutoff[i]){
			float shadow = GetSpotLightShadow(norm, i, toLightDirection);
			lighting += (1.0 - shadow) * (diffuse + specular) * lightAttenuation * uSpotLightBrightness[i];
		}
	}

	outputFragColor = textureColor * vec4(lighting, 1.0);
	CalculateBrightness();
}
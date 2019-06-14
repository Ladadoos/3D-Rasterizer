#version 330
 
const int LightCount = 2; //Number of lights in our scene

in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 fragmentPosition;       
in mat3 tbnMatrix;
flat in int useNormalMap;

uniform sampler2D uTextureMap;	 // diffuse texture
uniform sampler2D uNormalMap;    // normal texture
uniform samplerCube uLocalEnvironmentMap;
uniform samplerCube uDepthCube[LightCount];     // depth texture
uniform float uShininess;
uniform vec3 uAmbientLightColor;
uniform vec3 uLightColor[LightCount];
uniform vec3 uCameraPosition;
uniform vec3 uLightPosition[LightCount];
uniform float uLightBrightness[LightCount];
uniform int uIsLightTarget;
uniform int uMaterialType;

layout (location = 0) out vec4 outputFragColor;
layout (location = 1) out vec4 outputBrightnessColor;

const float ambientStrenght = 0.9;
const float specularStrength = 0.8;

float GetShadowFactor(vec3 norm, int lightIndex, vec3 toLightDirection)
{
	vec3 fragToLight = fragmentPosition.xyz - uLightPosition[lightIndex];
	float currentDepth = length(fragToLight);
	//float bias = clamp(0.05 * tan(acos(dot(norm, toLightDirection))), 0, 0.1);
	float bias = max(0.025 * (1.0 - dot(norm, toLightDirection)), 0.75);  
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
	return (accumulatedShadow / 27);
}

void CalculateBrightness(){
	float brightness = dot(outputFragColor.rgb, vec3(0.2126, 0.7152, 0.0722));
    if(brightness > 1.75){
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
		outputFragColor = vec4(uLightColor[uIsLightTarget], 1);
		CalculateBrightness();
		return;
	}

	vec3 norm = normal.xyz;
	if(useNormalMap == 1){
		vec3 normalFromMap = texture(uNormalMap, uv).rgb;
		normalFromMap = normalFromMap * 2.0 - 1.0; //convert to range -1..1
		normalFromMap = tbnMatrix * normalFromMap; //convert coordinate system
		norm = normalize(normalFromMap.xyz);
	}else{
		norm = normalize(norm);
	}

	vec3 ambient = ambientStrenght * uAmbientLightColor;
	vec3 toCameraDir = normalize(uCameraPosition - fragmentPosition.xyz);

	vec3 lighting = ambient;
	for(int i = 0; i < LightCount; i++){
		vec3 toLightDirection = uLightPosition[i] - fragmentPosition.xyz;
		float toLightDistance = length(toLightDirection);
		float lightAttenuation = 1.0 / (toLightDistance * toLightDistance);
		toLightDirection *= (1.0 / toLightDistance); //normalize
		float diff = max(dot(norm.xyz, toLightDirection), 0);
		vec3 diffuse = diff * uLightColor[i];

		vec3 reflectDir = reflect(toLightDirection, norm);
		float spec = pow(max(dot(-toCameraDir, reflectDir), 0), uShininess);
		vec3 specular = specularStrength * spec * uLightColor[i];

		float shadowFactor = GetShadowFactor(norm, i, toLightDirection);
		lighting += (1.0 - shadowFactor) * (diffuse + specular) * lightAttenuation * uLightBrightness[i];
	}

	if(uMaterialType == 1){ //See SurfaceTexture.cs
		vec3 reflected = reflect(-toCameraDir, norm);
		textureColor = texture(uLocalEnvironmentMap, reflected);
	}else if(uMaterialType == 2){
	    const float ratio = 1.00 / 1.42;
		vec3 refracted = refract(-toCameraDir, norm, ratio);
		textureColor = texture(uLocalEnvironmentMap, refracted);
	}

	outputFragColor = textureColor * vec4(lighting, 1.0);
	CalculateBrightness();
}
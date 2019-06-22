#version 330

in vec2 fragmentPosition;	      // fragment position in screen space
in vec2 uv;						  // interpolated texture coordinates

uniform sampler2D uScreenTexture; // input texture (1st pass render target)
uniform sampler2D uBloomBlurTexture;   // texture with only the brightest parts of the uScreenTexture
uniform sampler2D uDepthTexture;

out vec4 outputColor;

const bool applyFog = false;
const float fogIntensity = 5;

const int KernelWidth = 5;
const int KernelHeight = 5;

const float edgeKernel[25] = float[](
	1, 1, 1, 1, 1,
	1, 1, 1, 1, 1,
	1, 1, -24, 1, 1,
	1, 1, 1, 1, 1,
	1, 1, 1, 1, 1
);

const float sharpKernel[25] = float[](
	0, 0, -1, 0, 0,
	0, -1, -1, -1, 0,
	-1, -1, 12, -1, -1,
	0, -1, -1, -1, 0,
	0, 0, -1, 0, 0
);

vec4 applyKernelEffect(sampler2D sampleTexture, float[25] kernel){
	vec4 finalColor = vec4(0);
	vec2 offset = 1.0 / textureSize(sampleTexture, 0);
	int ki = 0;
	int b1 = KernelWidth / 2; 
	int b2 = KernelHeight / 2;
	for(int x = -b1; x <= b1; x++){
		for(int y = -b2; y <= b2; y++){
			finalColor += texture(sampleTexture, uv + vec2(offset.x * x, offset.y * y)) * kernel[ki];
			ki++;
		}
	}
	return finalColor;
}

vec3 reinhardToneMapping(vec3 color){
	const float gamma = 2.2;
	const float exposure = 1;
	color *= exposure/(1.0 + color / exposure);
	color = pow(color, vec3(1.0 / gamma));
	return color;
}

vec4 standard(){
    vec4 hdrColor = texture(uScreenTexture, uv);  
	hdrColor.rgb += texture(uBloomBlurTexture, uv).rgb;
	if(applyFog){hdrColor += vec4(texture(uDepthTexture, uv).rgb * fogIntensity, 0);}
	return vec4(reinhardToneMapping(hdrColor.rgb), 1);
}

vec4 chromaticAbberation(vec4 color, float distToCenter, vec2 centerToFrag){
	centerToFrag *= -1;
	const float redOffset = 0.02;
	const float greenOffset = 0.025;
	const float blueOffset = 0.03;
	float dr = uv.s + redOffset * distToCenter * centerToFrag.x; 
	float dg = uv.s + greenOffset * distToCenter * centerToFrag.x; 
	float db = uv.s + blueOffset * distToCenter * centerToFrag.x; 
	float br = uv.t + redOffset * distToCenter * centerToFrag.y; 
	float bg = uv.t + greenOffset * distToCenter * centerToFrag.y; 
	float bb = uv.t + blueOffset * distToCenter * centerToFrag.y; 
	float newRed = (texture(uScreenTexture, vec2(dr, br)).r + color.r) / 2;
	float newGreen = (texture(uScreenTexture, vec2(dg, bg)).g + color.g) / 2;
	float newBlue = (texture(uScreenTexture, vec2(db, bb)).b + color.b) / 2;
	return vec4(newRed, newGreen, newBlue, 1);
}

vec4 invert(vec4 color){
	return vec4(1 - color.r, 1 - color.g, 1 - color.b, 1);
}

vec4 vignette(vec4 color, float distToCenter){
	const float innerDistance = 0.6;
	const float outerDistance = 0.8;
	const float blendFactor = 0.725;
	float vignette = smoothstep(outerDistance, innerDistance, distToCenter);
	return vec4(mix(color.rgb, color.rgb * vignette, blendFactor), 1);
}   

void main()
{
	vec4 color = vec4(0);
	color = standard();

	vec2 texSize = textureSize(uScreenTexture, 0);
	vec2 relativeToCenter = gl_FragCoord.xy / texSize - 0.5F;
	float distToCenter = length(relativeToCenter);

	//Uncomment the lines as you wish to create the special effects.
	color = chromaticAbberation(color, distToCenter, relativeToCenter);
		color = vignette(color, distToCenter);
	//color = invert(color);
	//color += applyKernelEffect(uScreenTexture, edgeKernel);
	//color += applyKernelEffect(uScreenTexture, sharpKernel);
	outputColor = color;
}
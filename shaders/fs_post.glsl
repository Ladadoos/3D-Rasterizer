#version 330

in vec2 fragmentPosition;	      // fragment position in screen space
in vec2 uv;						  // interpolated texture coordinates

uniform sampler2D uScreenTexture; // input texture (1st pass render target)
uniform sampler2D uBlurTexture;   // texture with only the brightest parts of the uScreenTexture
uniform sampler2D uDepthTexture;

out vec3 outputColor;

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

vec3 applyKernelEffect(sampler2D sampleTexture, float[25] kernel){
	vec3 finalColor = vec3(0);
	vec2 offset = 1.0 / textureSize(sampleTexture, 0);
	int ki = 0;
	int b1 = KernelWidth / 2; 
	int b2 = KernelHeight / 2;
	for(int x = -b1; x <= b1; x++){
		for(int y = -b2; y <= b2; y++){
			finalColor += vec3(texture(sampleTexture, uv + vec2(offset.x * x, offset.y * y))) * kernel[ki];
			ki++;
		}
	}
	return finalColor;
}

const float gamma = 2.2;
const float exposure = 1;
vec3 reinhardToneMapping(vec3 color){
	color *= exposure/(1.0 + color / exposure);
	color = pow(color, vec3(1.0 / gamma));
	return color;
}

vec3 standard(){
    vec3 hdrColor = texture(uScreenTexture, uv).rgb;  
	hdrColor += texture(uBlurTexture, uv).rgb;
	if(applyFog){hdrColor += texture(uDepthTexture, uv).rgb * fogIntensity;}
	return reinhardToneMapping(hdrColor);
}

vec3 chromaticAbberation(vec3 color){
	float dr = uv.s + 0.01; 
	if(dr > 1){dr = uv.s;}

	float dg = uv.s + 0.02; 
	if(dg > 1){dg = uv.s;}

	float db = uv.s + 0.03; 
	if(db > 1){db = uv.s;}

	return vec3(
      (texture(uScreenTexture, vec2(dr, uv.t)).r + color.r) / 2, 
      (texture(uScreenTexture, vec2(dg, uv.t)).g + color.g) / 2,
      (texture(uScreenTexture, vec2(db, uv.t)).b + color.b) / 2
	 );
}

vec3 invert(vec3 color){
	return vec3(1 - color.r, 1 - color.g, 1 - color.b);
}

vec3 vignette(vec3 color){
	vec2 relativeToCenter = gl_FragCoord.xy / textureSize(uScreenTexture, 0) - 0.5F; //screen coordinates
	float vig = smoothstep(0.6F, 0.4F, length(relativeToCenter));
	color.rgb = mix(color, color * vig, 0.5F);
	return color;
}   

void main()
{
	vec3 color = vec3(0);
	color = standard();

	//Uncomment the lines as you wish to create the special effects.
	//color = chromaticAbberation(color);
	//color = invert(color);
	//color += applyKernelEffect(uScreenTexture, edgeKernel);
	//color += applyKernelEffect(uScreenTexture, sharpKernel);
	color = vignette(color);
	outputColor = color;
}
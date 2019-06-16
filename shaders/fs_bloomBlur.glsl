#version 330

in vec2 fragmentPosition;	      // fragment position in screen space
in vec2 uv;						  // interpolated texture coordinates

uniform sampler2D uScreenTexture; 

out vec3 outputColor;

const int KernelWidth = 5;
const int KernelHeight = 5;

const float blurKernel[25] = float[](
	0.003765,	0.015019,	0.023792,	0.015019,	0.003765,
	0.015019,	0.059912,	0.094907,	0.059912,	0.015019,
	0.023792,	0.094907,	0.150342,	0.094907,	0.023792,
	0.015019,	0.059912,	0.094907,	0.059912,	0.015019,
	0.003765,	0.015019,	0.023792,	0.015019,	0.003765
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


void main()
{
	outputColor = applyKernelEffect(uScreenTexture, blurKernel);
}
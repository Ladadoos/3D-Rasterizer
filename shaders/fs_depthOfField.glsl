#version 330

in vec2 fragmentPosition;	    
in vec2 uv;					

uniform sampler2D uScreenTexture;
uniform sampler2D uDepthTexture;
uniform sampler2D uBlurTextureOne;
uniform sampler2D uBlurTextureTwo;
uniform sampler2D uBlurTextureThree;

out vec4 outputColor;

const float rangeOne = 0.1;
const float rangeTwo = 0.2;
const float rangeThree = 0.3;

void main()
{
	float currentDepth = texture(uDepthTexture, uv).r;
	float targetDepth = texture(uDepthTexture, vec2(0.5, 0.5)).r;

	float depth = abs(targetDepth - currentDepth);
	if(depth < rangeOne){
		vec4 originalColor = texture(uScreenTexture, uv);
		vec4 blurColor = texture(uBlurTextureOne, uv);
		outputColor = mix(originalColor, blurColor, depth / rangeOne);
	}else if(depth > rangeOne && depth < rangeTwo){
		vec4 blurColorTwo = texture(uBlurTextureTwo, uv);
		vec4 blurColorThree = texture(uBlurTextureThree, uv);
		outputColor = mix(blurColorTwo, blurColorThree, depth / rangeThree);
	}else{
		outputColor = texture(uBlurTextureThree, uv);
	}
}
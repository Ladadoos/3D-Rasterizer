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

void main()
{
	float currentDepth = texture(uDepthTexture, uv).r;	
	float targetDepth = texture(uDepthTexture, vec2(0.5, 0.5)).r;
	float deltaDepth = abs(targetDepth - currentDepth);

	if(deltaDepth <= rangeOne){
		vec4 originalColor = texture(uScreenTexture, uv);
		vec4 blurColorOne = texture(uBlurTextureOne, uv);
		outputColor = mix(originalColor, blurColorOne, deltaDepth / rangeOne);
	}else if(deltaDepth > rangeOne && deltaDepth <= rangeTwo){
		vec4 blurColorOne = texture(uBlurTextureOne, uv);
		vec4 blurColorTwo = texture(uBlurTextureTwo, uv);
		outputColor = mix(blurColorOne, blurColorTwo, (deltaDepth - rangeOne) / (rangeTwo - rangeOne));
	}else{				
		vec4 blurColorTwo = texture(uBlurTextureTwo, uv);
		vec4 blurColorThree = texture(uBlurTextureThree, uv);
		outputColor = mix(blurColorTwo, blurColorThree, min((deltaDepth - rangeTwo) / (1 - rangeTwo), 1));
	}
}
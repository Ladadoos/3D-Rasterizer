#version 330

in vec2 fragmentPosition;	    
in vec2 uv;					

uniform sampler2D uScreenTexture;

uniform int uKernelWidth;
uniform int uHorizontalPass;

out vec4 outputColor;

const float sqrt2pi = 2.50662827463;
float gaussian(int x, float mu, float s){
	float p1 = -0.5 * pow(((x - mu) / s), 2);
	float p2 = s * sqrt2pi;
	return exp(p1) / p2;
}

void applyGaussianBlur(){
	vec2 offset = 1.0 / textureSize(uScreenTexture, 0);
	int halfWidth = uKernelWidth / 2;
	outputColor = vec4(0);
	int j = 0;
	float sumWeights = 0;
	if(uHorizontalPass == 0){
		for(int i = -halfWidth; i <= halfWidth; i++){
			float gaussian = gaussian(j++, halfWidth, 1);
			sumWeights += gaussian;
			outputColor += texture(uScreenTexture, uv + vec2(0, offset.y * i)) * gaussian;
		}
	}else{
		for(int i = -halfWidth; i <= halfWidth; i++){
			float gaussian = gaussian(j++, halfWidth, 1);
			sumWeights += gaussian;
			outputColor += texture(uScreenTexture, uv + vec2(offset.x * i, 0)) * gaussian;
		}
	}
	//divide by sum weights to fix darkening/brightening of image
	outputColor /= sumWeights;
}

void applyBoxBlur(){
	vec2 offset = 1.0 / textureSize(uScreenTexture, 0);
	int halfWidth = uKernelWidth / 2;
	outputColor = vec4(0);
	if(uHorizontalPass == 0){
		for(int i = -halfWidth; i <= halfWidth; i++){
			outputColor += texture(uScreenTexture, uv + vec2(0, offset.y * i));
		}
	}else{
		for(int i = -halfWidth; i <= halfWidth; i++){
			outputColor += texture(uScreenTexture, uv + vec2(offset.x * i, 0));
		}
	}
	outputColor /= uKernelWidth;
}

void main()
{
	applyGaussianBlur();
}
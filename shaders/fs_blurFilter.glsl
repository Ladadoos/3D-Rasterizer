#version 330

in vec2 fragmentPosition;	    
in vec2 uv;					

uniform sampler2D uScreenTexture; 
uniform int uKernelWidth;
uniform int uHorizontalPass;

out vec3 outputColor;

const float sqrt2pi = 2.50662827463;
float gaussian(int x, float mu, float s){
	float p1 = -0.5 * pow(((x - mu) / s), 2);
	float p2 = s * sqrt2pi;
	return exp(p1) / p2;
}

void applyGaussianBlur(){
	vec2 offset = 1.0 / textureSize(uScreenTexture, 0);
	int halfWidth = uKernelWidth / 2;
	outputColor = vec3(0);
	int j = 0;
	if(uHorizontalPass == 0){
		for(int i = -halfWidth; i <= halfWidth; i++){
			outputColor += vec3(texture(uScreenTexture, uv + vec2(0, offset.y * i))) * gaussian(j++, halfWidth, 1);
		}
	}else{
		for(int i = -halfWidth; i <= halfWidth; i++){
			outputColor += vec3(texture(uScreenTexture, uv + vec2(offset.x * i, 0))) * gaussian(j++, halfWidth, 1);
		}
	}
}

void applyBoxBlur(){
	vec2 offset = 1.0 / textureSize(uScreenTexture, 0);
	int halfWidth = uKernelWidth / 2;
	outputColor = vec3(0);
	if(uHorizontalPass == 0){
		for(int i = -halfWidth; i <= halfWidth; i++){
			outputColor += vec3(texture(uScreenTexture, uv + vec2(0, offset.y * i)));
		}
	}else{
		for(int i = -halfWidth; i <= halfWidth; i++){
			outputColor += vec3(texture(uScreenTexture, uv + vec2(offset.x * i, 0)));
		}
	}
	outputColor /= uKernelWidth;
}

void main()
{
	applyGaussianBlur();
}
#version 330

in vec2 fragmentPosition;	      // fragment position in screen space
in vec2 uv;						  // interpolated texture coordinates

uniform sampler2D uScreenTexture; // input texture (1st pass render target)

out vec3 outputColor;

void standard(){
	outputColor = texture(uScreenTexture, uv).rgb;
}

void chromaticAbberation(){
	outputColor = vec3 ( 
      texture(uScreenTexture, vec2(uv.s + 0.01, uv.t)).r, 
      texture(uScreenTexture, vec2(uv.s - 0.02, uv.t)).g,
      texture(uScreenTexture, vec2(uv.s - 0.06, uv.t)).b);
}

void vignette(){
	vec3 color = texture(uScreenTexture, uv).rgb;
	vec2 relativeToCenter = gl_FragCoord.xy / textureSize(uScreenTexture, 0) - 0.5F; //screen coordinates
	float vig = smoothstep(0.6F, 0.5F, length(relativeToCenter));
	color.rgb = mix(color, color * vig, 0.25F);
	outputColor = color;
}   

void main()
{
	standard();
}
#version 330

// shader input
in vec2 P;						// fragment position in screen space
in vec2 uv;						// interpolated texture coordinates
uniform sampler2D pixels;		// input texture (1st pass render target)

// shader output
out vec3 outputColor;

void standard(){
	outputColor = texture( pixels, uv ).rgb;
}

void chromaticAbberation(){
	outputColor = vec3 ( 
      texture(pixels, vec2(uv.s + 0.01, uv.t)).r, 
      texture(pixels, vec2(uv.s - 0.02, uv.t)).g,
      texture(pixels, vec2(uv.s - 0.06, uv.t)).b);
}

void vignette(){
	vec3 color = texture( pixels, uv ).rgb;
	vec2 relativeToCenter = gl_FragCoord.xy / textureSize(pixels, 0) - 0.5F; //screen coordinates
	float vig = smoothstep(0.6F, 0.5F, length(relativeToCenter));
	color.rgb = mix(color, color * vig, 0.25F);
	outputColor = color;
}   

void main()
{
	vignette();
}
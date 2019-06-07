#version 330

in vec2 fragmentPosition;	      // fragment position in screen space
in vec2 uv;						  // interpolated texture coordinates

uniform sampler2D uScreenTexture; // input texture (1st pass render target)
uniform sampler2D uBlurTexture;

out vec3 outputColor;

const float blurKernel[25] = float[](
	0.003765,	0.015019,	0.023792,	0.015019,	0.003765,
	0.015019,	0.059912,	0.094907,	0.059912,	0.015019,
	0.023792,	0.094907,	0.150342,	0.094907,	0.023792,
	0.015019,	0.059912,	0.094907,	0.059912,	0.015019,
	0.003765,	0.015019,	0.023792,	0.015019,	0.003765
);

vec3 getBloom(){
	vec3 bloom = vec3(0);
	vec2 tex_offset = 1.0 / textureSize(uBlurTexture, 0);
	int k = 0;
	for(int i = -2; i <= 2; i++){
		for(int j = -2; j <= 2; j++){
			bloom += vec3(texture(uBlurTexture, uv + vec2(tex_offset.x * i, tex_offset.y * j))) * blurKernel[k];
			k++;
		}
	}
	return bloom;
}

vec3 standard(){
	const float gamma = 2.2;
    vec3 hdrColor = texture(uScreenTexture, uv).rgb;  
	hdrColor += getBloom();
	float exposure = 2f;
    vec3 mapped = vec3(1.0) - exp(-hdrColor * exposure);   // exposure tone mapping
    mapped = pow(mapped, vec3(1.0 / gamma));    // gamma correction 
    return mapped;
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
	color.rgb = mix(color, color * vig, 0.25F);
	return color;
}   

void main()
{
	vec3 color = vec3(0);
	color = standard();

	//color = chromaticAbberation(color);
	//color = invert(color);
	//color = vignette(color);

	outputColor = color;
}
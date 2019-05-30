#version 330
 
// shader input
in vec2 vUV;				// vertex uv coordinate
in vec3 vNormal;			// untransformed vertex normal
in vec3 vPosition;			// untransformed vertex position
in vec3 vTangent;
in vec3 vBitangent;

// shader output
out vec4 normal;			// transformed vertex normal
out vec2 uv;		
out vec4 pos;
out vec4 posLightSpace;
out vec3 lightposition;
out vec3 cameraposition;

uniform vec3 cameraPosition;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 lightPosition;
uniform mat4 lightSpacematrix;

out mat3 tbnMatrix;

// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = projection * view * model * vec4(vPosition, 1.0);

	// forward normal and uv coordinate; will be interpolated over triangle
	normal = model * vec4( vNormal, 0.0f );
	pos = model * vec4( vPosition, 1.0f );
	posLightSpace = lightSpacematrix * vec4(pos.xyz, 1.0f);
	uv = vUV;
	lightposition = lightPosition;
	cameraposition = cameraPosition;

	//Form 3 basis for a coordinate system at the triangles surface for normal mapping
	//vec3 vBitangent = cross(vNormal, vTangent);
	vec3 tangent = normalize(vec3(model * vec4(vTangent, 0)));
	vec3 bitangent = normalize(vec3(model * vec4(vBitangent, 0)));
	vec3 normal = normalize(vec3(model * vec4(vNormal, 0)));
	tbnMatrix = mat3(tangent, bitangent, normal);
}
#version 330
 
in vec2 iUV;				// vertex uv coordinate
in vec3 iNormal;			// untransformed vertex normal
in vec3 iPosition;			// untransformed vertex position
in vec3 iTangent;           // untransformed vertex tangent
in vec3 iBitangent;         // untransformed vertex bitangent

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform vec3 uCameraPosition;
uniform vec3 uLightPosition;
uniform mat4 uLightSpacematrix;

out vec4 normal;			// transformed vertex normal
out vec2 uv;		        
out vec4 fragmentPosition;
out vec4 posLightSpace;
out vec3 lightposition;
out vec3 cameraposition;
out mat3 tbnMatrix;

void main()
{
	gl_Position = uProjection * uView * uModel * vec4(iPosition, 1.0);

	normal = normalize(uModel * vec4(iNormal, 0));
	fragmentPosition = uModel * vec4(iPosition, 1);
	posLightSpace = uLightSpacematrix * vec4(fragmentPosition.xyz, 1.0f);
	uv = iUV;
	lightposition = uLightPosition;
	cameraposition = uCameraPosition;

	//Form the 3 basis for a coordinate system at the triangle's surface for normal mapping
	vec3 tangent = normalize(vec3(uModel * vec4(iTangent, 0)));
	vec3 bitangent = normalize(vec3(uModel * vec4(iBitangent, 0)));
	tbnMatrix = mat3(tangent, bitangent, normal.xyz);
}
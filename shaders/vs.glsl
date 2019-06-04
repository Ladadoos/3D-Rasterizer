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

out vec4 normal;			// transformed vertex normal
out vec2 uv;		        
out vec4 fragmentPosition;
out vec3 lightposition;
out vec3 cameraposition;
out mat3 tbnMatrix;

void main()
{
	fragmentPosition = uModel * vec4(iPosition, 1);
	gl_Position = uProjection * uView * fragmentPosition;

	normal = normalize(uModel * vec4(iNormal, 0));
	uv = iUV;
	lightposition = uLightPosition;
	cameraposition = uCameraPosition;

	//Form the 3 basis for a coordinate system at the triangle's surface for normal mapping
	vec3 tangent = normalize(vec3(uModel * vec4(iTangent, 0)));
	vec3 bitangent = normalize(vec3(uModel * vec4(iBitangent, 0)));
	tbnMatrix = mat3(tangent, bitangent, normal.xyz);
}
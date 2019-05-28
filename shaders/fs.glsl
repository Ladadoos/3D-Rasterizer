#version 330
 
// shader input
in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec4 pos;                    
uniform sampler2D pixels;		// texture sampler
uniform vec3 cameraPosition;
uniform vec3 ambientLightColor;
uniform vec3 lightColor;
uniform vec3 lightPosition;

// shader output
out vec4 outputColor;

const float ambientStrenght = 0.9;
const float specularStrength = 0.5;
const float shininess = 64;

// fragment shader
void main()
{
   // outputColor = texture( pixels, uv ) + 0.5f * vec4( normal.xyz, 1 ) + vec4(lightPosition, 1);

   vec3 ambient = ambientStrenght * ambientLightColor;

   vec3 norm = normalize(normal.xyz);
   vec3 toLightDir = lightPosition - pos.xyz;
   float toLightDist = length(toLightDir);
   float lightAttenuation = 1 / (toLightDist) * 25;
   toLightDir *= (1 / toLightDist); //normalize
   float diff = max(dot(norm.xyz, toLightDir), 0);
   vec3 diffuse = diff * lightColor;

   vec3 toCameraDir = normalize(cameraPosition - pos.xyz);
   vec3 reflectDir = reflect(-toLightDir, norm);
   float spec = pow(max(dot(-toCameraDir, reflectDir), 0), shininess);
   vec3 specular = specularStrength * spec * lightColor;

   outputColor = texture( pixels, uv ) * vec4((ambient + diffuse + specular) * lightAttenuation, 1);
}
using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class ModelShader : Shader
    {
        public int attribute_position;
        public int attribute_normal;
        public int attribute_uv;
        public int attribute_tangent;
        public int attribute_bitangent;

        public int uniform_textureMap;
        public int uniform_depthMap;
        public int uniform_normalMap;
        public int uniform_useNormalMap;

        public int uniform_modelMatrix;
        public int uniform_viewMatrix;
        public int uniform_projectionMatrix;

        public int uniform_ambientlightcolor;
        public int uniform_camPos;
        public int uniform_lightSpaceMatrix;
        public int uniform_lightcolor;
        public int uniform_lightposition;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs.glsl";
            fragmentFile = "../../shaders/fs.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_position = GL.GetAttribLocation(programID, "iPosition");
            attribute_normal = GL.GetAttribLocation(programID, "iNormal");
            attribute_uv = GL.GetAttribLocation(programID, "iUV");
            attribute_tangent = GL.GetAttribLocation(programID, "iTangent");
            attribute_bitangent = GL.GetAttribLocation(programID, "iBitangent");

            uniform_textureMap = GL.GetUniformLocation(programID, "uTextureMap");
            uniform_depthMap = GL.GetUniformLocation(programID, "uDepthMap");
            uniform_normalMap = GL.GetUniformLocation(programID, "uNormalMap");
            uniform_useNormalMap = GL.GetUniformLocation(programID, "uUseNormalMap");

            uniform_modelMatrix = GL.GetUniformLocation(programID, "uModel");
            uniform_viewMatrix = GL.GetUniformLocation(programID, "uView");
            uniform_projectionMatrix = GL.GetUniformLocation(programID, "uProjection");

            uniform_ambientlightcolor = GL.GetUniformLocation(programID, "uAmbientLightColor");
            uniform_camPos = GL.GetUniformLocation(programID, "uCameraPosition");
            uniform_lightSpaceMatrix = GL.GetUniformLocation(programID, "uLightSpacematrix");
            uniform_lightcolor = GL.GetUniformLocation(programID, "uLightColor");
            uniform_lightposition = GL.GetUniformLocation(programID, "uLightPosition");

            System.Console.WriteLine("ModelShader locations: " + attribute_position + " / " +
                                                        attribute_normal + " / " +
                                                        attribute_uv + " / " +
                                                        uniform_textureMap + " / " +
                                                        uniform_modelMatrix + " / " +
                                                        uniform_viewMatrix + " / " +
                                                        uniform_projectionMatrix + " / " +
                                                        uniform_ambientlightcolor + " / " +
                                                        uniform_camPos + " / " +
                                                        uniform_lightSpaceMatrix + " / " +
                                                        uniform_depthMap + " / " +
                                                        uniform_lightcolor + " / " +
                                                        uniform_lightposition + " / " + 
                                                        uniform_normalMap + " / " +
                                                        attribute_tangent + " / " + 
                                                        attribute_bitangent + " / " + 
                                                        uniform_useNormalMap
            );
        }
    }
}

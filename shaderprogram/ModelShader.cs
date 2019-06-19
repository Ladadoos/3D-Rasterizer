using OpenTK.Graphics.OpenGL;
using System;

namespace Rasterizer
{
    public class ModelShader : Shader
    {
        public int attribute_position;
        public int attribute_normal;
        public int attribute_uv;
        public int attribute_tangent;
        public int attribute_bitangent;

        public int uniform_textureMap;
        public int[] uniform_depthCubes = new int[Consts.LightsCount];
        public int uniform_normalMap;
        public int uniform_useNormalMap;
        public int uniform_shininess;

        public int uniform_modelMatrix;
        public int uniform_viewMatrix;
        public int uniform_projectionMatrix;

        public int uniform_ambientLightColor;
        public int uniform_cameraPosition;
        public int[] uniform_lightColor = new int[Consts.LightsCount];
        public int[] uniform_lightPosition = new int[Consts.LightsCount];
        public int[] uniform_lightBrightness = new int[Consts.LightsCount];

        public int uniform_isLightTarget;
        public int uniform_materialType;
        public int uniform_localEnvironmentMap;

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
            uniform_normalMap = GL.GetUniformLocation(programID, "uNormalMap");
            uniform_useNormalMap = GL.GetUniformLocation(programID, "uUseNormalMap");

            uniform_shininess = GL.GetUniformLocation(programID, "uShininess");

            uniform_modelMatrix = GL.GetUniformLocation(programID, "uModel");
            uniform_viewMatrix = GL.GetUniformLocation(programID, "uView");
            uniform_projectionMatrix = GL.GetUniformLocation(programID, "uProjection");

            uniform_ambientLightColor = GL.GetUniformLocation(programID, "uAmbientLightColor");
            uniform_cameraPosition = GL.GetUniformLocation(programID, "uCameraPosition");
            
            uniform_isLightTarget = GL.GetUniformLocation(programID, "uIsLightTarget");
            uniform_materialType = GL.GetUniformLocation(programID, "uMaterialType");
            uniform_localEnvironmentMap = GL.GetUniformLocation(programID, "uLocalEnvironmentMap");

            Console.WriteLine("ModelShader locations: " + attribute_position + " / " +
                                                        attribute_normal + " / " +
                                                        attribute_uv + " / " +
                                                        uniform_textureMap + " / " +
                                                        uniform_modelMatrix + " / " +
                                                        uniform_viewMatrix + " / " +
                                                        uniform_projectionMatrix + " / " +
                                                        uniform_ambientLightColor + " / " +
                                                        uniform_cameraPosition + " / " +
                                                        uniform_normalMap + " / " +
                                                        attribute_tangent + " / " +
                                                        attribute_bitangent + " / " +
                                                        uniform_useNormalMap + " / " +
                                                        uniform_shininess + " / " +
                                                        uniform_localEnvironmentMap + " / " +
                                                        uniform_materialType
            );

            Console.Write("    |>");
            for (int i = 0; i < Consts.LightsCount; i++)
            {
                uniform_depthCubes[i] = GL.GetUniformLocation(programID, "uDepthCube[" + i + "]");
                Console.Write(" / " + uniform_depthCubes[i]);
            }
            Console.Write(" -");
            for (int i = 0; i < Consts.LightsCount; i++)
            {
                uniform_lightColor[i] = GL.GetUniformLocation(programID, "uLightColor[" + i + "]");
                uniform_lightPosition[i] = GL.GetUniformLocation(programID, "uLightPosition[" + i + "]");
                uniform_lightBrightness[i] = GL.GetUniformLocation(programID, "uLightBrightness[" + i + "]");
                Console.Write(" / " + uniform_lightColor[i]);
                Console.Write(" / " + uniform_lightPosition[i]);
                Console.Write(" / " + uniform_lightBrightness[i]);
            }
            Console.Write("\n");
        }
    }
}

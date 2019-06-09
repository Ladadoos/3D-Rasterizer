using OpenTK.Graphics.OpenGL;
using System;

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
        public int[] uniform_depthCubes = new int[Consts.PointLightsCount];
        public int[] uniform_depthMaps = new int[Consts.SpotLightsCount];
        public int uniform_normalMap;
        public int uniform_useNormalMap;
        public int uniform_shininess;

        public int uniform_modelMatrix;
        public int uniform_viewMatrix;
        public int uniform_projectionMatrix;

        public int uniform_ambientLightColor;
        public int uniform_cameraPosition;
        public int[] uniform_pointLightColor = new int[Consts.PointLightsCount];
        public int[] uniform_pointLightPosition = new int[Consts.PointLightsCount];
        public int[] uniform_pointLightBrightness = new int[Consts.PointLightsCount];

        public int[] uniform_spotLightColor = new int[Consts.SpotLightsCount];
        public int[] uniform_spotLightPosition = new int[Consts.SpotLightsCount];
        public int[] uniform_spotLightBrightness = new int[Consts.SpotLightsCount];
        public int[] uniform_spotLightDirection = new int[Consts.SpotLightsCount];
        public int[] uniform_spotLightCutoff = new int[Consts.SpotLightsCount];

        public int uniform_isLightTarget;

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
            for (int i = 0; i < Consts.PointLightsCount; i++)
            {
                uniform_depthCubes[i] = GL.GetUniformLocation(programID, "uDepthCube[" + i + "]");
                Console.Write(uniform_depthCubes[i]);
            }
            for (int i = 0; i < Consts.SpotLightsCount; i++)
            {
                uniform_depthMaps[i] = GL.GetUniformLocation(programID, "uDepthMap[" + i + "]");
                Console.Write(uniform_depthMaps[i]);
            }
            
            uniform_shininess = GL.GetUniformLocation(programID, "uShininess");

            uniform_modelMatrix = GL.GetUniformLocation(programID, "uModel");
            uniform_viewMatrix = GL.GetUniformLocation(programID, "uView");
            uniform_projectionMatrix = GL.GetUniformLocation(programID, "uProjection");

            uniform_ambientLightColor = GL.GetUniformLocation(programID, "uAmbientLightColor");
            uniform_cameraPosition = GL.GetUniformLocation(programID, "uCameraPosition");
            for (int i = 0; i < Consts.PointLightsCount; i++)
            {
                uniform_pointLightColor[i] = GL.GetUniformLocation(programID, "uPointLightColor[" + i + "]");
                uniform_pointLightPosition[i] = GL.GetUniformLocation(programID, "uPointLightPosition[" + i + "]");
                uniform_pointLightBrightness[i] = GL.GetUniformLocation(programID, "uPointLightBrightness[" + i + "]");
                Console.Write(uniform_pointLightColor[i] + "/" );
                Console.Write(uniform_pointLightPosition[i] + "/");
                Console.Write(uniform_pointLightBrightness[i] + "/");
            }
            for (int i = 0; i < Consts.SpotLightsCount; i++)
            {
                uniform_spotLightColor[i] = GL.GetUniformLocation(programID, "uSpotLightColor[" + i + "]");
                uniform_spotLightPosition[i] = GL.GetUniformLocation(programID, "uSpotLightPosition[" + i + "]");
                uniform_spotLightBrightness[i] = GL.GetUniformLocation(programID, "uSpotLightBrightness[" + i + "]");
                uniform_spotLightDirection[i] = GL.GetUniformLocation(programID, "uSpotLightDirection[" + i + "]");
                uniform_spotLightCutoff[i] = GL.GetUniformLocation(programID, "uSpotLightCutoff[" + i + "]");
                Console.Write(uniform_spotLightColor[i] + "/");
                Console.Write(uniform_spotLightPosition[i] + "/");
                Console.Write(uniform_spotLightBrightness[i] + "/");
                Console.Write(uniform_spotLightDirection[i] + "/");
                Console.Write(uniform_spotLightCutoff[i]);
            }

            uniform_isLightTarget = GL.GetUniformLocation(programID, "uIsLightTarget");

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
                                                        uniform_shininess
            );
        }
    }
}

using OpenTK.Graphics.OpenGL;
using System;

namespace Rasterizer
{
    class SkyboxShader : Shader
    {
        public int uniform_viewProjectionMatrix;
        public int uniform_skyboxCubeMap;
        public int attribute_position;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_skybox.glsl";
            fragmentFile = "../../shaders/fs_skybox.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_position = GL.GetAttribLocation(programID, "iPosition");
            uniform_viewProjectionMatrix = GL.GetUniformLocation(programID, "viewProjection");
            uniform_skyboxCubeMap = GL.GetUniformLocation(programID, "uSkyboxCubeMap");

            Console.WriteLine("SkyboxShader locations: " + attribute_position + " / " + uniform_viewProjectionMatrix
                 + " / " + uniform_skyboxCubeMap);
        }
    }
}

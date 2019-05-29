using OpenTK.Graphics.OpenGL;
using System;

namespace Template
{
    class SkyboxShader : Shader
    {
        public int uniform_viewProjectionMatrix;
        public int uniform_skyboxCubeMap;
        public int attribute_vpos;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_skybox.glsl";
            fragmentFile = "../../shaders/fs_skybox.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            uniform_viewProjectionMatrix = GL.GetUniformLocation(programID, "viewProjection");
            uniform_skyboxCubeMap = GL.GetUniformLocation(programID, "skyboxCubeMap");

            Console.WriteLine("SkyboxShader locations: " + attribute_vpos + " / " + uniform_viewProjectionMatrix
                 + " / " + uniform_skyboxCubeMap);
        }
    }
}

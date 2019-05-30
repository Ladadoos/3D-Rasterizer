using System;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class DepthShader : Shader
    {
        public int uniform_modelMatrix;
        public int uniform_viewProjectionMatrix;
        public int attribute_position;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_depth.glsl";
            fragmentFile = "../../shaders/fs_depth.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_position = GL.GetAttribLocation(programID, "iPosition");
            uniform_modelMatrix = GL.GetUniformLocation(programID, "uModel");
            uniform_viewProjectionMatrix = GL.GetUniformLocation(programID, "uViewProjection");

            Console.WriteLine("DepthShader locations: " + attribute_position + " / " +
                         uniform_modelMatrix + " / " + uniform_viewProjectionMatrix);
        }
    }
}

using OpenTK.Graphics.OpenGL;
using System;

namespace Template
{
    public class PostProcessingShader : Shader
    {
        public int attribute_position;
        public int attribute_uv;
        public int uniform_screenTexture;
        public int uniform_blurTexture;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_post.glsl";
            fragmentFile = "../../shaders/fs_post.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_position = GL.GetAttribLocation(programID, "iPosition");
            attribute_uv = GL.GetAttribLocation(programID, "iUV");
            uniform_screenTexture = GL.GetUniformLocation(programID, "uScreenTexture");
            uniform_blurTexture = GL.GetUniformLocation(programID, "uBlurTexture");

            Console.WriteLine("PostProcessing locations: " + attribute_position + " / " +
                         attribute_uv + " / " + uniform_screenTexture + " / " + uniform_blurTexture);
        }
    }
}

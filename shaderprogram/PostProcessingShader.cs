using OpenTK.Graphics.OpenGL;
using System;

namespace Rasterizer
{
    public class PostProcessingShader : Shader
    {
        public int attribute_position;
        public int attribute_uv;
        public int uniform_screenTexture;
        public int uniform_blurTexture;
        public int uniform_depthTexture;

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
            uniform_blurTexture = GL.GetUniformLocation(programID, "uBloomBlurTexture");
            uniform_depthTexture = GL.GetUniformLocation(programID, "uDepthTexture");

            Console.WriteLine(GetType().Name + " locations: " + attribute_position + " / " +
                         attribute_uv + " / " + uniform_screenTexture + " / " + uniform_blurTexture
                         + " / " + uniform_depthTexture);
        }
    }
}

using OpenTK.Graphics.OpenGL;
using System;

namespace Rasterizer
{
    public class BlurFilterShader : PostProcessingShader
    {
        public int uniform_kernelWidth;
        public int uniform_isHorizontalPass;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_post.glsl";
            fragmentFile = "../../shaders/fs_blurFilter.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            base.GetAllVariableLocations();

            uniform_kernelWidth = GL.GetUniformLocation(programID, "uKernelWidth");
            uniform_isHorizontalPass = GL.GetUniformLocation(programID, "uHorizontalPass");
            Console.WriteLine("\t" + GetType().Name + " EXT locations: " + uniform_kernelWidth + " / " + uniform_isHorizontalPass);
        }
    }
}

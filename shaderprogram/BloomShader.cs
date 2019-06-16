using OpenTK.Graphics.OpenGL;
using System;

namespace Template
{
    public class BloomShader : PostProcessingShader
    {
        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_gaussianBlur.glsl";
            fragmentFile = "../../shaders/fs_bloomBlur.glsl";
        }
    }
}

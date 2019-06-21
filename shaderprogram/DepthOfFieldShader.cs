using OpenTK.Graphics.OpenGL;
using System;

namespace Rasterizer
{
    public class DepthOfFieldShader : PostProcessingShader
    {
        public int uniform_blurTextureOne;
        public int uniform_blurTextureTwo;
        public int uniform_blurTextureThree;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_post.glsl";
            fragmentFile = "../../shaders/fs_depthOfField.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            base.GetAllVariableLocations();

            uniform_blurTextureOne = GL.GetUniformLocation(programID, "uBlurTextureOne");
            uniform_blurTextureTwo = GL.GetUniformLocation(programID, "uBlurTextureTwo");
            uniform_blurTextureThree = GL.GetUniformLocation(programID, "uBlurTextureThree");
            Console.WriteLine("\t" + GetType().Name + " EXT locations: " + uniform_blurTextureOne + " / " + uniform_blurTextureTwo
                + " / " + uniform_blurTextureThree);
        }
    }
}

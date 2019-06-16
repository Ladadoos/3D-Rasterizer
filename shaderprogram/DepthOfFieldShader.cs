using OpenTK.Graphics.OpenGL;
using System;

namespace Template
{
    public class DepthOfFieldShader : PostProcessingShader
    {
        public int uniform_focalDistance;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_gaussianBlur.glsl";
            fragmentFile = "../../shaders/fs_depthOfFieldBlur.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            base.GetAllVariableLocations();

            uniform_focalDistance = GL.GetUniformLocation(programID, "uFocalDistance");

            Console.WriteLine("     DepthOfField locations: " + uniform_focalDistance);
        }
    }
}

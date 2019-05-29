using OpenTK.Graphics.OpenGL;
using System;

namespace Template
{
    public class PostProcessingShader : Shader
    {
        public int attribute_vpos;
        public int attribute_vuvs;
        public int uniform_pixels;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_post.glsl";
            fragmentFile = "../../shaders/fs_post.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            attribute_vuvs = GL.GetAttribLocation(programID, "vUV");
            uniform_pixels = GL.GetUniformLocation(programID, "pixels");

            Console.WriteLine("PostProcessing locations: " + attribute_vpos + " / " +
                         attribute_vuvs + " / " + uniform_pixels);
        }
    }
}

using System;
using OpenTK.Graphics.OpenGL;

namespace Rasterizer
{
    public class DepthShader : Shader
    {
        public int attribute_position;

        public int uniform_modelMatrix;
        public int uniform_viewProjectionMatrix;

        public int uniform_lightPosition;
        public int uniform_textureMap;
        public int attribute_uv;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_depth.glsl";
            fragmentFile = "../../shaders/fs_depth.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_position = GL.GetAttribLocation(programID, "iPosition");
            attribute_uv = GL.GetAttribLocation(programID, "iUV");

            uniform_modelMatrix = GL.GetUniformLocation(programID, "uModel");
            uniform_viewProjectionMatrix = GL.GetUniformLocation(programID, "uViewProjection");

            uniform_lightPosition = GL.GetUniformLocation(programID, "uLightPosition");
            uniform_textureMap = GL.GetUniformLocation(programID, "uTextureMap");

            Console.WriteLine("DepthShader locations: " + attribute_position + " / " +
                         uniform_modelMatrix + " / " + uniform_viewProjectionMatrix + " / " + uniform_lightPosition
                         + " / " + attribute_uv);
        }
    }
}

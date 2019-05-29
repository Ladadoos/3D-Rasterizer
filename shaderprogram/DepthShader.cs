using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class DepthShader : Shader
    {
        public int uniform_modelMatrix;
        public int uniform_viewProjectionMatrix;
        public int attribute_vpos;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_depth.glsl";
            fragmentFile = "../../shaders/fs_depth.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            uniform_modelMatrix = GL.GetUniformLocation(programID, "model");
            uniform_viewProjectionMatrix = GL.GetUniformLocation(programID, "viewProjection");

            Console.WriteLine("DepthShader locations: " + attribute_vpos + " / " +
                         uniform_modelMatrix + " / " + uniform_viewProjectionMatrix);
        }
    }
}

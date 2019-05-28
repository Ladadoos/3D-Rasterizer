using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class ModelShader : Shader
    {
        public int attribute_vpos;
        public int attribute_vnrm;
        public int attribute_vuvs;
        public int uniform_mview;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs.glsl";
            fragmentFile = "../../shaders/fs.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            attribute_vnrm = GL.GetAttribLocation(programID, "vNormal");
            attribute_vuvs = GL.GetAttribLocation(programID, "vUV");
            uniform_mview = GL.GetUniformLocation(programID, "transform");
            Console.WriteLine(uniform_mview);
        }
    }
}

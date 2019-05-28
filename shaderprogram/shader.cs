using System;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    public abstract class Shader
    {
        // data members
        public int programID, vsID, fsID;
        protected string vertexFile, fragmentFile;

        // constructor
        public Shader()
        {
            DefineShaderDirectories();

            // compile shaders
            programID = GL.CreateProgram();
            Load(vertexFile, ShaderType.VertexShader, programID, out vsID);
            Load(fragmentFile, ShaderType.FragmentShader, programID, out fsID);
            GL.LinkProgram(programID);
            Console.WriteLine(GL.GetProgramInfoLog(programID));

            // get locations of shader parameters
            GetAllVariableLocations();
        }

        protected abstract void GetAllVariableLocations();

        protected abstract void DefineShaderDirectories();

        // loading shaders
        void Load(String filename, ShaderType type, int program, out int ID)
        {
            // source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
            ID = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename)) GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program, ID);
            Console.WriteLine(GL.GetShaderInfoLog(ID));
        }
    }
}

using System;
using System.IO;
using OpenTK;
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
            string log = GL.GetProgramInfoLog(programID);
            if(log != string.Empty)
            {
                Console.WriteLine(log);
            }
         
            // get locations of shader parameters
            GetAllVariableLocations();
        }

        protected abstract void GetAllVariableLocations();

        protected abstract void DefineShaderDirectories();

        public void Bind()
        {
            GL.UseProgram(programID);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public void LoadFloat(int location, float value)
        {    
            GL.Uniform1(location, value);     
        }

        public void LoadInt32(int location, int value)
        {
            GL.Uniform1(location, value);
        }

        public void LoadVector3(int location, Vector3 vector)
        {
            GL.Uniform3(location, vector);
        }

        public void LoadVector2(int location, Vector2 vector)
        {
            GL.Uniform2(location, vector);
        }

        public void LoadBoolean(int location, bool value)
        {
            GL.Uniform1(location, value ? 1 : 0);
        }

        public void LoadMatrix(int location, Matrix4 matrix)
        {
            GL.UniformMatrix4(location, false, ref matrix);
        }

        // loading shaders
        void Load(String filename, ShaderType type, int program, out int ID)
        {
            // source: http://neokabuto.blogspot.nl/2013/03/opentk-tutorial-2-drawing-triangle.html
            ID = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename)) GL.ShaderSource(ID, sr.ReadToEnd());
            GL.CompileShader(ID);
            GL.AttachShader(program, ID);
            string log = GL.GetShaderInfoLog(ID);
            if(log != string.Empty)
            {
                Console.WriteLine(log);
            }
        }
    }
}

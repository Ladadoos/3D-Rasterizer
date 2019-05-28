using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class ModelShader : Shader
    {
        public int attribute_vpos;
        public int attribute_vnrm;
        public int attribute_vuvs;
        public int uniform_pixels;
        public int uniform_mview;
        public int uniform_ambientlightcolor;
        public int uniform_mviewproj;
        public int uniform_camPos;

        public int uniform_lightcolor;
        public int uniform_lightposition;

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
            uniform_pixels = GL.GetUniformLocation(programID, "pixels");
            uniform_mview = GL.GetUniformLocation(programID, "transform");
            uniform_mviewproj = GL.GetUniformLocation(programID, "viewproj");
            uniform_ambientlightcolor = GL.GetUniformLocation(programID, "ambientLightColor");
            uniform_lightcolor = GL.GetUniformLocation(programID, "lightColor");
            uniform_lightposition = GL.GetUniformLocation(programID, "lightPosition");
            uniform_camPos = GL.GetUniformLocation(programID, "cameraPosition");
        }
    }
}

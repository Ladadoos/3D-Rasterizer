using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class ModelShader : Shader
    {
        public int attribute_vpos;
        public int attribute_vnrm;
        public int attribute_vuvs;
        public int uniform_pixels;
        public int uniform_modelMatrix;
        public int uniform_viewMatrix;
        public int uniform_projectionMatrix;
        public int uniform_ambientlightcolor;
        public int uniform_camPos;
        public int uniform_lightSpaceMatrix;
        public int uniform_depthpixels;

        public int uniform_lightcolor;
        public int uniform_lightposition;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs.glsl";
            fragmentFile = "../../shaders/fs.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            uniform_modelMatrix = GL.GetUniformLocation(programID, "model");
            uniform_viewMatrix = GL.GetUniformLocation(programID, "view");
            uniform_projectionMatrix = GL.GetUniformLocation(programID, "projection");

            attribute_vpos = GL.GetAttribLocation(programID, "vPosition");
            attribute_vnrm = GL.GetAttribLocation(programID, "vNormal");
            attribute_vuvs = GL.GetAttribLocation(programID, "vUV");
            uniform_depthpixels = GL.GetUniformLocation(programID, "depthpixels");
            uniform_pixels = GL.GetUniformLocation(programID, "pixels");
            uniform_ambientlightcolor = GL.GetUniformLocation(programID, "ambientLightColor");
            uniform_lightcolor = GL.GetUniformLocation(programID, "lightColor");
            uniform_lightposition = GL.GetUniformLocation(programID, "lightPosition");
            uniform_camPos = GL.GetUniformLocation(programID, "cameraPosition");
            uniform_lightSpaceMatrix = GL.GetUniformLocation(programID, "lightSpacematrix");

            System.Console.WriteLine("ModelShader locations: " + attribute_vpos + " / " +
                                                        attribute_vnrm + " / " +
                                                        attribute_vuvs + " / " +
                                                        uniform_pixels + " / " +
                                                        uniform_modelMatrix + " / " +
                                                        uniform_viewMatrix + " / " +
                                                        uniform_projectionMatrix + " / " +
                                                        uniform_ambientlightcolor + " / " +
                                                        uniform_camPos + " / " +
                                                        uniform_lightSpaceMatrix + " / " +
                                                        uniform_depthpixels + " / " +
                                                        uniform_lightcolor + " / " +
                                                        uniform_lightposition
            );
        }
    }
}

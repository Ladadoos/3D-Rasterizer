using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class ModelShader : Shader
    {
        public int attribute_vPosition;
        public int attribute_vNormal;
        public int attribute_vUV;
        public int attribute_vTangent;
        public int attribute_vBitangent;
        public int uniform_pixels;
        public int uniform_modelMatrix;
        public int uniform_viewMatrix;
        public int uniform_projectionMatrix;
        public int uniform_ambientlightcolor;
        public int uniform_camPos;
        public int uniform_lightSpaceMatrix;
        public int uniform_depthpixels;
        public int uniform_normalpixels;

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

            attribute_vPosition = GL.GetAttribLocation(programID, "vPosition");
            attribute_vNormal = GL.GetAttribLocation(programID, "vNormal");
            attribute_vUV = GL.GetAttribLocation(programID, "vUV");
            attribute_vTangent = GL.GetAttribLocation(programID, "vTangent");
            uniform_depthpixels = GL.GetUniformLocation(programID, "depthpixels");
            uniform_pixels = GL.GetUniformLocation(programID, "pixels");
            uniform_normalpixels = GL.GetUniformLocation(programID, "normalPixels");
            uniform_ambientlightcolor = GL.GetUniformLocation(programID, "ambientLightColor");
            uniform_lightcolor = GL.GetUniformLocation(programID, "lightColor");
            uniform_lightposition = GL.GetUniformLocation(programID, "lightPosition");
            uniform_camPos = GL.GetUniformLocation(programID, "cameraPosition");
            uniform_lightSpaceMatrix = GL.GetUniformLocation(programID, "lightSpacematrix");

            attribute_vBitangent = GL.GetAttribLocation(programID, "vBitangent");
            System.Console.WriteLine("ModelShader locations: " + attribute_vPosition + " / " +
                                                        attribute_vNormal + " / " +
                                                        attribute_vUV + " / " +
                                                        uniform_pixels + " / " +
                                                        uniform_modelMatrix + " / " +
                                                        uniform_viewMatrix + " / " +
                                                        uniform_projectionMatrix + " / " +
                                                        uniform_ambientlightcolor + " / " +
                                                        uniform_camPos + " / " +
                                                        uniform_lightSpaceMatrix + " / " +
                                                        uniform_depthpixels + " / " +
                                                        uniform_lightcolor + " / " +
                                                        uniform_lightposition + " / " + 
                                                        uniform_normalpixels + " / " +
                                                        attribute_vTangent + " / " + 
                                                        attribute_vBitangent
            );
        }
    }
}

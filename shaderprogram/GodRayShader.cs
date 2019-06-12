using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class GodRayShader : PostProcessingShader
    {
        public int uniform_lightPositionScreen;

        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_godray.glsl";
            fragmentFile = "../../shaders/fs_godray.glsl";
        }

        protected override void GetAllVariableLocations()
        {
            base.GetAllVariableLocations();
            uniform_lightPositionScreen = GL.GetUniformLocation(programID, "uLightPositionScreen");
        }
    }
}

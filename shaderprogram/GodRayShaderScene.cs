using OpenTK.Graphics.OpenGL;

namespace Template
{
    public class GodRayShaderScene : ModelShader
    {
        protected override void DefineShaderDirectories()
        {
            vertexFile = "../../shaders/vs_godray_scene.glsl";
            fragmentFile = "../../shaders/fs_godray_scene.glsl";
        }
    }
}

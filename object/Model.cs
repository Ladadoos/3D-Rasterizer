using OpenTK;

namespace Template
{
    class Model : GameObject
    {
        public Model(Mesh mesh, SurfaceTexture texture, Vector3 position, Vector3 rotationInAngle, Vector3 scale)
            : base(mesh, texture, position, rotationInAngle, scale)
        {

        }
    }
}

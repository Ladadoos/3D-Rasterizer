using OpenTK;

namespace Template
{
    class Light : GameObject
    {
        public Vector3 color;

        public Light(Mesh mesh, SurfaceTexture texture, Vector3 position, Vector3 rotationInAngle, Vector3 scale)
            : base(mesh, texture, position, rotationInAngle, scale)
        {

        }
    }
}

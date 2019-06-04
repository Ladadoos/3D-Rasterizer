using OpenTK;

namespace Template
{
    public abstract class Light : GameObject
    {
        public Vector3 color;
        public Matrix4 projectionMatrix;

        public Light(Mesh mesh, SurfaceTexture texture, Vector3 position, Vector3 rotationInAngle, Vector3 scale)
            : base(mesh, texture, position, rotationInAngle, scale)
        {

        }
    }
}

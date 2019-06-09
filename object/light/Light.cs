using OpenTK;

namespace Template
{
    public abstract class Light : GameObject
    {
        public int id;
        public Vector3 color;
        public float brightness;
        public Matrix4 projectionMatrix;
        public Matrix4[] viewMatrices;

        public Light(Mesh mesh, SurfaceTexture texture, Vector3 position, Vector3 rotationInAngle, Vector3 scale)
            : base(mesh, texture, position, rotationInAngle, scale)
        {

        }
    }
}

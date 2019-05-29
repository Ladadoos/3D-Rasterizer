using OpenTK;

namespace Template
{
    class Light : GameObject
    {
        public Vector3 color;

        public Light(Mesh mesh, Vector3 position, Vector3 rotationInAngle, Vector3 scale)
            : base(mesh, position, rotationInAngle, scale)
        {

        }
    }
}

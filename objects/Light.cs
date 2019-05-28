using OpenTK;

namespace Template
{
    class Light : GameObject
    {
        public Vector3 color;

        public Light(string meshFile, Vector3 position, Vector3 rotationInAngle, Vector3 scale)
            : base(meshFile, position, rotationInAngle, scale)
        {

        }
    }
}

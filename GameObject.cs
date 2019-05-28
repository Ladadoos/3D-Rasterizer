using OpenTK;

namespace Template
{
    /*abstract class GameObject
    {
        public Vector3 position;
        public Vector3 rotationInAngle;
        public Vector3 scale;

        public GameObject(Vector3 position, Vector3 rotationInAngle, Vector3 scale)
        {
            this.position = position;
            this.rotationInAngle = rotationInAngle;
            this.scale = scale;
        }

        public Matrix4 GetTransformationMatrix()
        {
            Matrix4 scaleMatrix = new Matrix4(scale.X, 0, 0, 0, 0, scale.Y, 0, 0, 0, 0, scale.Z, 0, 0, 0, 0, 1);
            Matrix4 transformationmatrix = scaleMatrix *
                Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotationInAngle.X)) *
                Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationInAngle.Y)) *
                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotationInAngle.Z)) *
                Matrix4.CreateTranslation(position);
            return transformationmatrix;
        }

        public abstract void Render();
    }*/
}

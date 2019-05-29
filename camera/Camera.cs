using OpenTK;

namespace Template
{
    abstract class Camera
    {
        public Vector3 position;
        public float pitch, yaw;
        private Matrix4 projectionMatrix;

        protected Vector3 forward, right;
        protected float movementSpeed = 50;

        public Camera(Vector3 position)
        {
            this.position = position;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        public abstract Matrix4 GetViewMatrix();

        public abstract void ProcessInput(OpenTKApp app, float deltaTime);
    }
}

using OpenTK;

namespace Template
{
    abstract class Camera
    {
        public ViewFrustum frustum;
        public Vector3 position;
        public float pitch, yaw;
        public Vector3 forward, right;

        private Matrix4 projectionMatrix;
        protected float movementSpeed = 50;

        public Camera(Vector3 position)
        {
            this.position = position;
            frustum = new ViewFrustum(1.2F, 1.3F, 0.1F, 1000);
            projectionMatrix = frustum.CreateProjectionMatrix();
        }

        public Matrix4 GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        public void CalculateFrustumPlanes()
        {
            frustum.UpdateFrustumPoints(this);
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, position + forward, Vector3.UnitY);
        }

        public abstract void ProcessInput(OpenTKApp app, float deltaTime);
    }
}

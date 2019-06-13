using OpenTK;

namespace Template
{
    abstract class Camera
    {
        public static Matrix4[] GetSurroundViews(Vector3 position)
        {
            Matrix4[] viewMatrices = new Matrix4[6];
            viewMatrices[0] = Matrix4.LookAt(position, position + new Vector3(1, 0, 0), new Vector3(0, -1, 0));
            viewMatrices[1] = Matrix4.LookAt(position, position + new Vector3(-1, 0, 0), new Vector3(0, -1, 0));
            viewMatrices[2] = Matrix4.LookAt(position, position + new Vector3(0, 1, 0), new Vector3(0, 0, 1));
            viewMatrices[3] = Matrix4.LookAt(position, position + new Vector3(0, -1, 0), new Vector3(0, 0, -1));
            viewMatrices[4] = Matrix4.LookAt(position, position + new Vector3(0, 0, 1), new Vector3(0, -1, 0));
            viewMatrices[5] = Matrix4.LookAt(position, position + new Vector3(0, 0, -1), new Vector3(0, -1, 0));
            return viewMatrices;
        }

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

        public abstract bool ProcessInput(OpenTKApp app, float deltaTime);
    }
}

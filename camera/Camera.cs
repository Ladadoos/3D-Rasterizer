using OpenTK;
using System;

namespace Template
{
    abstract class Camera
    {
        public ViewFrustum frustum;
        public Vector3 position;
        public float pitch, yaw;

        private Matrix4 projectionMatrix;

        public Vector3 forward, right;
        protected float movementSpeed = 50;

        public Camera(Vector3 position)
        {
            this.position = position;
            frustum = new ViewFrustum(1.2F, 1.3F, 0.1F, 1000);
            projectionMatrix = frustum.GetFieldOfView();
        }

        public Matrix4 GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        public void UpdateFrustumPoints()
        {
            frustum.UpdateFrustumPoints(this);
        }

        public abstract Matrix4 GetViewMatrix();

        public abstract void ProcessInput(OpenTKApp app, float deltaTime);
    }
}

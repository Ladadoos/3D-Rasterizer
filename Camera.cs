using OpenTK;
using OpenTK.Input;
using System;

namespace Template
{
    class Camera
    {
        public Vector3 position;
        public float rotationAngle;

        private float movementSpeed = 50;
        private float zoomSpeed = 100;
        private float rotationSpeed = 150;

        private int prevMouseWheelValue = 0;

        private Matrix4 projectionMatrix;

        public Camera(Vector3 position)
        {
            this.position = position;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.CreateTranslation(position) *
                   Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(rotationAngle)) *
                   Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(45));
        }

        public Matrix4 GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        public void ProcessInput(float deltaTime)
        {
            var keyboard = Keyboard.GetState();

            float rad = MathHelper.DegreesToRadians(rotationAngle);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            Vector3 right = new Vector3(cos, 0, sin);
            Vector3 forward = Vector3.Cross(right, new Vector3(0, 1, 0));
            if (keyboard[Key.W])
            {
                position += movementSpeed * deltaTime * forward;
            } else if (keyboard[Key.S])
            {
                position -= movementSpeed * deltaTime * forward;
            }
            if (keyboard[Key.A])
            {
                position += movementSpeed * deltaTime * right;
            } else if (keyboard[Key.D])
            {
                position -= movementSpeed * deltaTime * right;
            }
            if (keyboard[Key.Q])
            {
                rotationAngle += (float)(rotationSpeed * deltaTime);
            } else if (keyboard[Key.E])
            {
                rotationAngle -= (float)(rotationSpeed * deltaTime);
            }

            var mouse = Mouse.GetState();
            if (mouse.Wheel != prevMouseWheelValue)
            {
                int delta = mouse.Wheel - prevMouseWheelValue;
                prevMouseWheelValue = mouse.Wheel;
                position.Y += (float)(delta * zoomSpeed * deltaTime);
            }
        }
    }
}

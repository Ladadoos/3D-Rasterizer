using OpenTK;
using OpenTK.Input;
using System;

namespace Template
{
    class FPSCamera : Camera
    {
        private float cameraSensitivity = 0.1F;
        private Vector2 previousMousePosition;

        public FPSCamera(Vector3 position) : base(position)
        {
            previousMousePosition = new Vector2(1,1);
        }

        public override void ProcessInput(OpenTKApp app, float deltaTime)
        {
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            Vector2 currentMousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
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

            if (app.Focused && currentMousePosition != previousMousePosition)
            {
                float deltaX = (previousMousePosition.X - Mouse.GetState().X) * cameraSensitivity;
                float deltaY = (previousMousePosition.Y - Mouse.GetState().Y) * cameraSensitivity;
                yaw -= deltaX;
                pitch += deltaY;
                if (pitch > 89.0f) { pitch = 89.0f; }
                if (pitch < -89.0f) { pitch = -89.0f; }
                if (yaw > 360) { yaw -= 360; }
                if (yaw < 0) { yaw += 360; }
                double pitchRad = pitch * Math.PI / 180;
                double yawRad = yaw * Math.PI / 180;
                double cosPitch = Math.Cos(pitchRad);
                forward.X = (float)(cosPitch * Math.Cos(yawRad));
                forward.Y = (float)(Math.Sin(pitchRad));
                forward.Z = (float)(cosPitch * Math.Sin(yawRad));
                forward.Normalize();
                right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, forward));
                previousMousePosition = currentMousePosition;
                Mouse.SetPosition(app.Bounds.Left + app.Bounds.Width / 2, app.Bounds.Top + app.Bounds.Height / 2);
            }
        }
    }
}

using OpenTK;
using OpenTK.Input;
using System;

namespace Template
{
    class TopDownCamera : Camera
    {
        private int prevMouseWheelValue = 0;
        private float zoomSpeed = 100;
        private float rotationSpeed = 150;

        public TopDownCamera(Vector3 position, float screenWidth, float screenHeight) : base(position, screenWidth, screenHeight)
        {
            pitch = -45F;
        }

        public override bool ProcessInput(OpenTKApp app, float deltaTime)
        {
            bool inputGiven = false;
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            if (keyboard[Key.W])
            {
                Vector3 offset = movementSpeed * deltaTime * forward;
                offset.Y = 0;
                position += offset;
                inputGiven = true;
            } else if (keyboard[Key.S])
            {
                Vector3 offset = movementSpeed * deltaTime * forward;
                offset.Y = 0;
                position -= offset;
                inputGiven = true;
            }

            if (keyboard[Key.A])
            {
                position += movementSpeed * deltaTime * right;
                inputGiven = true;
            } else if (keyboard[Key.D])
            {
                position -= movementSpeed * deltaTime * right;
                inputGiven = true;
            }

            if (keyboard[Key.Q])
            {
                yaw -= rotationSpeed * deltaTime;
                inputGiven = true;
            } else if (keyboard[Key.E])
            {
                yaw += rotationSpeed * deltaTime;
                inputGiven = true;
            }

            if (mouse.Wheel != prevMouseWheelValue)
            {
                int delta = mouse.Wheel - prevMouseWheelValue;
                prevMouseWheelValue = mouse.Wheel;
                position.Y += delta * zoomSpeed * deltaTime;
                inputGiven = true;
            }

            if (inputGiven)
            {
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
            }

            return inputGiven;
        }
    }
}

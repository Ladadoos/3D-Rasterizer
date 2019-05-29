using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class TopDownCamera : Camera
    {
        private int prevMouseWheelValue = 0;
        private float zoomSpeed = 100;
        private float rotationSpeed = 150;

        public TopDownCamera(Vector3 position) : base(position)
        {
            pitch = 45;
        }

        public override Matrix4 GetViewMatrix()
        {
                return Matrix4.CreateTranslation(position) *
                       Matrix4.CreateFromAxisAngle(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(yaw)) *
                       Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), MathHelper.DegreesToRadians(pitch));
        }

        public override void ProcessInput(OpenTKApp app, float deltaTime)
        {
            var keyboard = Keyboard.GetState();

            float rad = MathHelper.DegreesToRadians(yaw);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);
            right = new Vector3(cos, 0, sin);
            forward = Vector3.Cross(right, new Vector3(0, 1, 0));

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
                yaw += rotationSpeed * deltaTime;
            } else if (keyboard[Key.E])
            {
                yaw -= rotationSpeed * deltaTime;
            }

            var mouse = Mouse.GetState();
            if (mouse.Wheel != prevMouseWheelValue)
            {
                int delta = mouse.Wheel - prevMouseWheelValue;
                prevMouseWheelValue = mouse.Wheel;
                position.Y += delta * zoomSpeed * deltaTime;
            }
        }
    }
}

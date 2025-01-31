﻿using OpenTK;
using OpenTK.Input;
using System;

namespace Rasterizer
{
    class FPSCamera : Camera
    {
        private float cameraSensitivity = 0.1F;
        private Vector2 previousMousePosition;

        public FPSCamera(Vector3 position, float screenWidth, float screenHeight) : base(position, screenWidth, screenHeight)
        {
            previousMousePosition = new Vector2(1,1);
        }

        public override bool ProcessInput(OpenTKApp app, float deltaTime)
        {
            bool inputGiven = false;
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            Vector2 currentMousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            if (keyboard[Key.W])
            {
                position += movementSpeed * deltaTime * forward;
                inputGiven = true;
            } else if (keyboard[Key.S])
            {
                position -= movementSpeed * deltaTime * forward;
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
                inputGiven = true;
            }

            return inputGiven;
        }
    }
}

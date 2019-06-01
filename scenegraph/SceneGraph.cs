using OpenTK;
using System;
using System.Collections.Generic;

namespace Template
{
    class SceneGraph
    {
        public List<GameObject> objects = new List<GameObject>();
        public List<Light> lights = new List<Light>();

        private Matrix4 lightViewMatrix, lightProjectionMatrix, lightSpaceMatrix;

        private int rendered = 0;

        public void CalculateMatrices()
        {
            Matrix4.CreateOrthographicOffCenter(-75, 75, -75, 75, 1, 1000, out lightProjectionMatrix);
            lightViewMatrix = Matrix4.LookAt(lights[0].position, Vector3.Zero, new Vector3(0, 1, 0));
            lightSpaceMatrix = lightViewMatrix * lightProjectionMatrix;


        }

        public void RenderDepthMap(Camera camera, DepthShader shader)
        {
            foreach(GameObject obj in objects)
            {
                Matrix4 globalTransform = obj.GetGlobalTransformationMatrix();
                obj.RenderDepth(shader, globalTransform, lightSpaceMatrix);
            }
        }

        public void RenderScene(Camera camera, ModelShader shader, DepthMap depthMap)
        {
            rendered = 0;

            shader.Bind();
            shader.LoadVector3(shader.uniform_ambientlightcolor, new Vector3(0.2F, 0.2F, 0.9F));
            shader.LoadVector3(shader.uniform_lightcolor, lights[0].color);
            shader.LoadVector3(shader.uniform_lightposition, lights[0].position);
            shader.LoadVector3(shader.uniform_camPos, camera.position);
            shader.Unbind();

            Matrix4 viewMatrix = camera.GetViewMatrix();
            Matrix4 projMatrix = camera.GetProjectionMatrix();

            foreach (GameObject obj in objects)
            {
                Matrix4 globalTransform = obj.GetGlobalTransformationMatrix();
                Vector4 center = new Vector4(obj.mesh.hitboxCenter, 1) * globalTransform;
                Vector3 scale = globalTransform.ExtractScale();
                float radius = Math.Max(scale.X, Math.Max(scale.Y, scale.Z));

                if (camera.frustum.IsSphereInFrustum(center.Xyz, obj.mesh.hitboxRadius * radius))
                {
                    obj.RenderScene(shader, globalTransform, viewMatrix, projMatrix, lightSpaceMatrix, depthMap);
                    rendered++;
                }
            }
            Console.WriteLine("Objects rendered:" + rendered);
        }
    }
}

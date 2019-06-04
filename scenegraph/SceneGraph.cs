using OpenTK;
using System;
using System.Collections.Generic;

namespace Template
{
    class SceneGraph
    {
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<PointLight> lights = new List<PointLight>();
        private List<GameObject> toRenderObjects = new List<GameObject>();

        private int rendered = 0;

        public void UpdateScene(Camera camera)
        {
            rendered = 0;
            toRenderObjects.Clear();

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();

                if(gameObject.mesh == null) { continue; }

                Vector4 center  = new Vector4(gameObject.mesh.hitboxCenter, 1) * gameObject.globalTransform;
                Vector3 scale = gameObject.globalTransform.ExtractScale();
                float radius = Math.Max(scale.X, Math.Max(scale.Y, scale.Z));

                if (camera.frustum.IsSphereInFrustum(center.Xyz, gameObject.mesh.hitboxRadius * radius))
                {
                    rendered++;
                    toRenderObjects.Add(gameObject);
                }
            }

            Console.WriteLine("Objects rendered:" + rendered);
        }

        public void RenderDepthMap(Camera camera, DepthShader shader)
        {
            foreach (PointLight light in lights)
            {
                light.depthCube.Bind();
                for (int i = 0; i < 6; i++)
                {
                    light.depthCube.SetRenderSide(i);
                    foreach (GameObject gameObject in toRenderObjects)
                    {
                        if(gameObject is Light) { continue; }
                        gameObject.RenderDepth(shader, gameObject.globalTransform, light.viewMatrices[i] * light.projectionMatrix, light.globalTransform.ExtractTranslation());
                    }
                }
                light.depthCube.Unbind();
            }
        }

        public void RenderScene(Camera camera, ModelShader shader, CubeDepthMap cubeDepthMap)
        {
            Matrix4 viewMatrix = camera.GetViewMatrix();
            Matrix4 projMatrix = camera.GetProjectionMatrix();

            shader.Bind();
            shader.LoadVector3(shader.uniform_ambientlightcolor, new Vector3(0.1F, 0.1F, 0.7F));
            shader.LoadVector3(shader.uniform_lightcolor, lights[0].color);
            shader.LoadVector3(shader.uniform_lightposition, lights[0].globalTransform.ExtractTranslation());
            shader.LoadVector3(shader.uniform_camPos, camera.position);
            shader.LoadMatrix(shader.uniform_viewMatrix, viewMatrix);
            shader.LoadMatrix(shader.uniform_projectionMatrix, projMatrix);
            shader.Unbind();

            foreach (GameObject gameObject in toRenderObjects)
            {
                gameObject.RenderScene(shader, gameObject.globalTransform, viewMatrix, projMatrix, cubeDepthMap);
            }
        }
    }
}

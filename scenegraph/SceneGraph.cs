using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    class SceneGraph
    {
        public List<GameObject> gameObjects = new List<GameObject>();

        private List<PointLight> lights = new List<PointLight>();
        private List<GameObject> toRenderObjects = new List<GameObject>();
        private int rendered = 0;

        public void AddLight(PointLight light)
        {
            light.id = lights.Count;
            lights.Add(light);
        }

        public void UpdateScene(Camera camera)
        {
            rendered = 0;

            toRenderObjects.Clear();
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();

                if(gameObject.mesh == null)
                {
                    continue;
                }

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
                        if(gameObject is Light)
                        {
                            continue;
                        }
                        gameObject.RenderToDepth(shader, gameObject.globalTransform, light.viewMatrices[i] * light.projectionMatrix, light.globalTransform.ExtractTranslation());
                    }
                }
                light.depthCube.Unbind();
            }
        }

        public void RenderScene(Camera camera, ModelShader shader, CubeDepthMap[] cubeDepthMaps)
        {
            shader.Bind();
            shader.LoadVector3(shader.uniform_ambientLightColor, new Vector3(0.1F));
            for(int i = 0; i < lights.Count; i++)
            {
                shader.LoadVector3(shader.uniform_lightColor[i], lights[i].color);
                shader.LoadFloat(shader.uniform_lightBrightness[i], lights[i].brightness);
                shader.LoadVector3(shader.uniform_lightPosition[i], lights[i].globalTransform.ExtractTranslation());

                GL.Uniform1(shader.uniform_depthCubes[i], 2 + i);
                GL.ActiveTexture(TextureUnit.Texture2 + i);
                GL.BindTexture(TextureTarget.TextureCubeMap, cubeDepthMaps[i].cubeDepthMapId);
            }
            shader.LoadVector3(shader.uniform_cameraPosition, camera.position);
            shader.LoadMatrix(shader.uniform_viewMatrix, camera.GetViewMatrix());
            shader.LoadMatrix(shader.uniform_projectionMatrix, camera.GetProjectionMatrix());
            shader.Unbind();

            foreach (GameObject gameObject in toRenderObjects)
            {
                gameObject.RenderToScene(shader);
            }
        }
    }
}

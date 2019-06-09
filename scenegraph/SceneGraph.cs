using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    class SceneGraph
    {
        public List<GameObject> gameObjects = new List<GameObject>();

        private List<PointLight> pointLights = new List<PointLight>();
        private List<SpotLight> spotLights = new List<SpotLight>();
        private List<GameObject> toRenderObjects = new List<GameObject>();
        private int rendered = 0;

        public void AddPointLight(PointLight light)
        {
            light.id = pointLights.Count;
            pointLights.Add(light);
        }

        public void AddSpotLight(SpotLight light)
        {
            light.id = spotLights.Count;
            spotLights.Add(light);
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

           // Console.WriteLine("Objects rendered:" + rendered);
        }

        public void RenderDepthMap(Camera camera, DepthShader shader)
        {
            foreach (PointLight light in pointLights)
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
                        gameObject.RenderToDepth(shader, light.viewMatrices[i] * light.projectionMatrix, light.globalTransform.ExtractTranslation());
                    }
                }
                light.depthCube.Unbind();
            }

            foreach (SpotLight light in spotLights)
            {
                light.depthMap.Bind();
                foreach (GameObject gameObject in toRenderObjects)
                {
                    if (gameObject is Light)
                    {
                        continue;
                    }
                    gameObject.RenderToDepth(shader, light.viewMatrices[0] * light.projectionMatrix, light.globalTransform.ExtractTranslation());
                }
                light.depthMap.Unbind();
            }
        }

        public void RenderScene(Camera camera, ModelShader shader)
        {
            shader.Bind();
            shader.LoadVector3(shader.uniform_ambientLightColor, new Vector3(0.05f));
            shader.LoadVector3(shader.uniform_cameraPosition, camera.position);
            shader.LoadMatrix(shader.uniform_viewMatrix, camera.GetViewMatrix());
            shader.LoadMatrix(shader.uniform_projectionMatrix, camera.GetProjectionMatrix());

            for (int i = 0; i < pointLights.Count; i++)
            {
                shader.LoadVector3(shader.uniform_pointLightColor[i], pointLights[i].color);
                shader.LoadFloat(shader.uniform_pointLightBrightness[i], pointLights[i].brightness);
                shader.LoadVector3(shader.uniform_pointLightPosition[i], pointLights[i].globalTransform.ExtractTranslation());

                GL.Uniform1(shader.uniform_depthCubes[i], 2 + i);
                GL.ActiveTexture(TextureUnit.Texture2 + i);
                GL.BindTexture(TextureTarget.TextureCubeMap, pointLights[i].depthCube.cubeDepthMapId);
            }

            for (int i = 0; i < spotLights.Count; i++)
            {
                shader.LoadVector3(shader.uniform_spotLightColor[i], spotLights[i].color);
                shader.LoadFloat(shader.uniform_spotLightBrightness[i], spotLights[i].brightness);
                shader.LoadVector3(shader.uniform_spotLightPosition[i], spotLights[i].globalTransform.ExtractTranslation());

                shader.LoadVector3(shader.uniform_spotLightDirection[i], spotLights[i].direction);
                shader.LoadFloat(shader.uniform_spotLightCutoff[i], (float)Math.Cos(spotLights[i].cutOffAngle));

                GL.Uniform1(shader.uniform_depthMaps[i], 2 + pointLights.Count + i);
                GL.ActiveTexture(TextureUnit.Texture2 + i + pointLights.Count);
                GL.BindTexture(TextureTarget.Texture2D, spotLights[i].depthMap.depthMapId);
            }

            shader.Unbind();

            foreach (GameObject gameObject in toRenderObjects)
            {
                gameObject.RenderToScene(shader);
            }
        }
    }
}

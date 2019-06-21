using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Rasterizer
{
    class SceneGraph
    {
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<PointLight> lights = new List<PointLight>();

        private List<GameObject> toRenderGameObjects = new List<GameObject>();

        private int renderedPreviousFrame = 0;
        private int renderedThisFrame = 0;

        private Matrix4 pointLightProjection;

        public SceneGraph()
        {
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), 1, 1, 1000, out pointLightProjection);
        }

        public void AddLight(PointLight light)
        {
            light.id = lights.Count;
            lights.Add(light);
        }

        public void UpdateScene(Camera camera)
        {
            renderedThisFrame = 0;

            toRenderGameObjects.Clear();
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
                    renderedThisFrame++;
                    toRenderGameObjects.Add(gameObject);
                }
            }

            if(renderedThisFrame != renderedPreviousFrame)
            {
                Console.WriteLine("Objects rendered this frame:" + renderedThisFrame);
            }
            renderedPreviousFrame = renderedThisFrame;
        }

        public void EndUpdateScene()
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.EndUpdate();
            }
        }

        public void UpdateEnvironmentMaps(ModelShader modelShader, Skybox skybox)
        {
            modelShader.Bind();
            modelShader.LoadMatrix(modelShader.uniform_projectionMatrix, pointLightProjection);
            modelShader.Unbind();

            foreach (GameObject source in toRenderGameObjects)
            {
                if (source.texture.materialType == MaterialType.Diffuse)
                {
                    continue;              
                }

                Vector3 position = source.globalTransform.ExtractTranslation();
                position.Y += 5;
                Matrix4[] viewMatrices = Camera.GetSurroundViews(position);

                modelShader.Bind();
                modelShader.LoadVector3(modelShader.uniform_cameraPosition, position);
                modelShader.Unbind();

                source.texture.environmentCubeMap.Bind();
                for (int i = 0; i < 6; i++)
                {
                    source.texture.environmentCubeMap.SetRenderSide(i);

                    modelShader.Bind();
                    modelShader.LoadMatrix(modelShader.uniform_viewMatrix, viewMatrices[i]);
                    modelShader.Unbind();

                    Matrix4 viewProjMatrix = viewMatrices[i].ClearTranslation() * pointLightProjection;
                    skybox.Render(viewProjMatrix);

                    foreach (GameObject gameObject in gameObjects)
                    {
                        if (gameObject.Equals(source))
                        {
                            continue;
                        }

                        gameObject.RenderToScene(modelShader);
                    }
                }

                source.texture.environmentCubeMap.Unbind();
            }
        }

        public void RenderDepthMap(DepthShader depthShader)
        {
            foreach (PointLight light in lights)
            {
                light.depthCube.Bind();
                for (int i = 0; i < 6; i++)
                {
                    light.depthCube.SetRenderSide(i);
                    foreach (GameObject gameObject in toRenderGameObjects)
                    {
                        if(gameObject is Light)
                        {
                            continue;
                        }
                        gameObject.RenderToDepth(depthShader, light.viewMatrices[i] * light.projectionMatrix, light.globalTransform.ExtractTranslation());
                    }
                }
                light.depthCube.Unbind();
            }
        }

        public void PrepareLightingInScene(ModelShader modelShader)
        {
            modelShader.LoadVector3(modelShader.uniform_ambientLightColor, new Vector3(0.001f));

            for (int i = 0; i < lights.Count; i++)
            {
                modelShader.LoadVector3(modelShader.uniform_lightColor[i], lights[i].color);
                modelShader.LoadFloat(modelShader.uniform_lightBrightness[i], lights[i].brightness);
                modelShader.LoadVector3(modelShader.uniform_lightPosition[i], lights[i].globalTransform.ExtractTranslation());
                modelShader.LoadTexture(modelShader.uniform_depthCubes[i], 3 + i, lights[i].depthCube.cubeDepthMapId, TextureTarget.TextureCubeMap);
            }
        }

        public void RenderScene(Camera camera, ModelShader modelShader)
        {
            modelShader.Bind();
            modelShader.LoadVector3(modelShader.uniform_cameraPosition, camera.position);
            modelShader.LoadMatrix(modelShader.uniform_viewMatrix, camera.GetViewMatrix());
            modelShader.LoadMatrix(modelShader.uniform_projectionMatrix, camera.GetProjectionMatrix());
            PrepareLightingInScene(modelShader);
            modelShader.Unbind();

            foreach (GameObject gameObject in toRenderGameObjects)
            {
                gameObject.RenderToScene(modelShader);
            }
        }
    }
}

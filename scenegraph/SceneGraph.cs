using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    class SceneGraph
    {
        public List<GameObject> gameObjects = new List<GameObject>();

        public List<PointLight> lights = new List<PointLight>();
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

           // Console.WriteLine("Objects rendered:" + rendered);
        }

        public void UpdateEnvironmentMaps(Camera camera, ModelShader modelShader, Skybox skybox)
        {
            Matrix4 projectionMatrix;
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), 1, 1, 1000, out projectionMatrix);

            modelShader.Bind();
            modelShader.LoadMatrix(modelShader.uniform_projectionMatrix, projectionMatrix);
            modelShader.Unbind();

            foreach (GameObject source in gameObjects)
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
                modelShader.LoadMatrix(modelShader.uniform_projectionMatrix, projectionMatrix);
                modelShader.Unbind();

                source.texture.environmentCubeMap.Bind();
                for (int i = 0; i < 6; i++)
                {
                    source.texture.environmentCubeMap.SetRenderSide(i);

                    modelShader.Bind();
                    modelShader.LoadMatrix(modelShader.uniform_viewMatrix, viewMatrices[i]);
                    modelShader.Unbind();

                    Matrix4 viewProjMatrix = viewMatrices[i].ClearTranslation() * projectionMatrix;
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

        public void RenderDepthMap(Camera camera, DepthShader depthShader)
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
                        gameObject.RenderToDepth(depthShader, light.viewMatrices[i] * light.projectionMatrix, light.globalTransform.ExtractTranslation());
                    }
                }
                light.depthCube.Unbind();
            }
        }

        public void RenderScene(Camera camera, ModelShader modelShader)
        {
            modelShader.Bind();
            modelShader.LoadVector3(modelShader.uniform_ambientLightColor, new Vector3(0.001f));
            modelShader.LoadVector3(modelShader.uniform_cameraPosition, camera.position);
            modelShader.LoadMatrix(modelShader.uniform_viewMatrix, camera.GetViewMatrix());
            modelShader.LoadMatrix(modelShader.uniform_projectionMatrix, camera.GetProjectionMatrix());

            for (int i = 0; i < lights.Count; i++)
            {
                modelShader.LoadVector3(modelShader.uniform_lightColor[i], lights[i].color);
                modelShader.LoadFloat(modelShader.uniform_lightBrightness[i], lights[i].brightness);
                modelShader.LoadVector3(modelShader.uniform_lightPosition[i], lights[i].globalTransform.ExtractTranslation());

                GL.Uniform1(modelShader.uniform_depthCubes[i], 3 + i);
                GL.ActiveTexture(TextureUnit.Texture0 + 3 + i);
                GL.BindTexture(TextureTarget.TextureCubeMap, lights[i].depthCube.cubeDepthMapId);
            }
            modelShader.Unbind();

            foreach (GameObject gameObject in toRenderObjects)
            {
                gameObject.RenderToScene(modelShader);
            }
        }
    }
}

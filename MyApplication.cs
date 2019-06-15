using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Template
{
    class Consts
    {
        public static int LightsCount = 2;
    }

    class MyApplication
    {
        private bool applyPostProcessing = true;

        public Surface screen;                     // background surface for printing etc.
        private Model dragon, sphere1, sphere2, centerBox, towerBoxBig, towerBoxSmall;
        private Model floorBottom, glassBall, shinyBall, tower;
        private float a = 0;
        private ScreenQuad quad = new ScreenQuad(); // screen filling quad for post processing
        public static Camera camera;
        private SceneGraph sceneGraph = new SceneGraph();
        private Skybox skybox = new Skybox();
        private PointLight skylight, light2;

        //Frame buffer objects
        private RenderTarget gaussianBlurFBO;
        private RenderTarget screenFBO;

        //Shaders
        private DepthShader depthShader = new DepthShader();
        private ModelShader modelShader = new ModelShader();
        private PostProcessingShader postProcessingShader = new PostProcessingShader();
        private GaussianBlurShader blurShader = new GaussianBlurShader();

        //Assets
        private List<Mesh> meshesAsset = new List<Mesh>();
        private List<SurfaceTexture> texturesAsset = new List<SurfaceTexture>();

        public void Initialize()
        {
            Random random = new Random();

            meshesAsset.Add(new Mesh("../../assets/dragon.obj")); //0
            meshesAsset.Add(new Mesh("../../assets/teapot.obj")); //1
            meshesAsset.Add(new Mesh("../../assets/floor.obj")); //2
            meshesAsset.Add(new Mesh("../../assets/sphere.obj")); //3
            meshesAsset.Add(new Mesh("../../assets/cube.obj")); //4
            meshesAsset.Add(new Mesh("../../assets/palm.obj")); //5
            meshesAsset.Add(new Mesh("../../assets/grass.obj")); //6
            meshesAsset.Add(new Mesh("../../assets/tower.obj")); //7

            int cms = 512; //cube map size for reflections/refractions
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/wood.jpg"), null, 4, MaterialType.Diffuse)); //0
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseGray.png"), null, 8, MaterialType.Reflective, new CubeTexture(cms, cms))); //1
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/floor.png"), new Texture("../../assets/floorNormal.png"), 8, MaterialType.Diffuse)); //2
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseGreen.png"), null, 2, MaterialType.Diffuse)); //3
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/grass.png"), null, 2, MaterialType.Diffuse)); //4
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/dirt.png"), new Texture("../../assets/dirtnormal.png"), 16, MaterialType.Diffuse)); //5
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/gunMetalGray.jpg"), null, 1, MaterialType.Diffuse)); //6
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/stoneDiffuse.jpg"), new Texture("../../assets/stoneNormal.jpg"), 16, MaterialType.Diffuse)); //7
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseBlue.png"), null, 16, MaterialType.Dieletric, new CubeTexture(cms, cms))); //8
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/flower.png"), null, 2, MaterialType.Diffuse)); //9
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/flower2.png"), null, 2, MaterialType.Diffuse)); //10

            dragon = new Model(meshesAsset[0], texturesAsset[6], new Vector3(80, -5, 80), new Vector3(0, 65, 0), new Vector3(7));
            sphere1 = new Model(meshesAsset[3], texturesAsset[0], new Vector3(2, 0, 0), Vector3.Zero, new Vector3(0.5F));
            sphere2 = new Model(meshesAsset[3], texturesAsset[0], new Vector3(0, -0.2F, 1.5F), Vector3.Zero, new Vector3(0.3F, 0.3F, 0.3F));
            centerBox = new Model(meshesAsset[4], texturesAsset[0], new Vector3(40, 45, 0), Vector3.Zero, new Vector3(25));
            towerBoxBig = new Model(meshesAsset[4], texturesAsset[7], new Vector3(-70, 12, -70), new Vector3(0, 45, 0), new Vector3(23));
            towerBoxSmall = new Model(meshesAsset[4], texturesAsset[7], new Vector3(-70, 28, -70), new Vector3(0, 15, 0), new Vector3(12));
            glassBall = new Model(meshesAsset[3], texturesAsset[8], new Vector3(70, 28, -70), new Vector3(0, 15, 0), new Vector3(16));
            shinyBall = new Model(meshesAsset[3], texturesAsset[1], new Vector3(-70, 28, 70), new Vector3(0, 15, 0), new Vector3(16));
            tower = new Model(meshesAsset[7], texturesAsset[2], new Vector3(-90, 25, 0), new Vector3(0, 15, 0), new Vector3(9));

            for (int i = 0; i < 50; i++)
            {
                if (random.Next(8) == 1)
                {
                    int texture = random.Next(2) == 1 ? 9 : 10;
                    sceneGraph.gameObjects.Add(new Model(
                        meshesAsset[6],
                        texturesAsset[texture],
                        new Vector3(random.Next(-150, 150), -0.2F, random.Next(-150, 150)),
                        new Vector3(0, random.Next(360), 0),
                        new Vector3(14 + random.Next(10)))
                    );
                } else
                {
                    sceneGraph.gameObjects.Add(new Model(
                        meshesAsset[6],
                        texturesAsset[4],
                        new Vector3(random.Next(-150, 150), -0.2F, random.Next(-150, 150)),
                        new Vector3(0, random.Next(360), 0),
                        new Vector3(12 + random.Next(10)))
                    );
                }
            }

            floorBottom = new Model(meshesAsset[2], texturesAsset[5], new Vector3(0, 40, 0), Vector3.Zero, new Vector3(20));

            skylight = new PointLight(meshesAsset[3], texturesAsset[3], new Vector3(300, 400, 0), Vector3.Zero, Vector3.One);
            skylight.color = new Vector3(1f, 0.5F, 0.1F); skylight.brightness = 70000;
            skylight.CreateDepth(new CubeDepthMap(1024, 1024));
            light2 = new PointLight(meshesAsset[3], texturesAsset[3], new Vector3(0, 155, 0), Vector3.Zero, new Vector3(4));
            light2.color = new Vector3(1f, 0.5F, 0.1F); light2.brightness = 80000;
            light2.CreateDepth(new CubeDepthMap(512, 512));

            screenFBO = new RenderTarget(3, screen.width, screen.height);
            gaussianBlurFBO = new RenderTarget(1, screen.width / 2, screen.height / 2);

            camera = new FPSCamera(new Vector3(-100, 150, 0), screen.width, screen.height);

            centerBox.AddChild(sphere2);
            sphere2.AddChild(sphere1);

            sceneGraph.gameObjects.Add(dragon);
            sceneGraph.gameObjects.Add(skylight);
            sceneGraph.gameObjects.Add(light2);
            sceneGraph.gameObjects.Add(sphere1);
            sceneGraph.gameObjects.Add(centerBox);
            sceneGraph.gameObjects.Add(towerBoxBig);
            sceneGraph.gameObjects.Add(towerBoxSmall);
            sceneGraph.gameObjects.Add(floorBottom);
            sceneGraph.gameObjects.Add(sphere2);
            sceneGraph.gameObjects.Add(glassBall);
            sceneGraph.gameObjects.Add(shinyBall);
            sceneGraph.gameObjects.Add(tower);

            sceneGraph.AddLight(skylight);
            sceneGraph.AddLight(light2);

            selectedLight = light2;
        }

        private PointLight selectedLight;
        private int action = 0;
        public void Tick(OpenTKApp app, float deltaTime)
        {
            var keyboard = Keyboard.GetState();

            for (int i = 0; i < sceneGraph.lights.Count; i++)
            {
                if (keyboard[Key.Number0 + i])
                {
                    selectedLight = sceneGraph.lights[i];
                    break;
                }
            }

            if (keyboard[Key.Plus]) { action = 0; }
            if (keyboard[Key.Minus]) { action = 1; }

            float delta = 0.01F;
            if (keyboard[Key.R])
            {
                selectedLight.color.X += action == 1 ? -delta : delta;
            }
            if (keyboard[Key.G])
            {
                selectedLight.color.Y += action == 1 ? -delta : delta;
            }
            if (keyboard[Key.B])
            {
                selectedLight.color.Z += action == 1 ? -delta : delta;
            }
        }

        public void RenderGL(OpenTKApp app, float deltaTime)
        {
            a += 30 * deltaTime;
            if (a > 360) { a -= 360; }
            float cos = (float)Math.Cos(MathHelper.DegreesToRadians(a));
            float sin = (float)Math.Sin(MathHelper.DegreesToRadians(a));
            sphere2.rotationInAngle.Y += deltaTime * 100;
            centerBox.rotationInAngle.Y += deltaTime * 50;
            centerBox.position.Y += sin / 10;
            light2.position.X = 155 * cos;
            light2.position.Z = 155 * sin * cos;

            if (camera.ProcessInput(app, deltaTime))
            {
                camera.CalculateFrustumPlanes();
            }
            sceneGraph.UpdateScene(camera);
            sceneGraph.RenderDepthMap(camera, depthShader);
            sceneGraph.UpdateEnvironmentMaps(camera, modelShader, skybox);

            if (applyPostProcessing)
            {
                //Do first render pass
                GL.Viewport(0, 0, screenFBO.width, screenFBO.height);
                screenFBO.Bind();
                GL.Clear(ClearBufferMask.DepthBufferBit);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                skybox.Render(camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix());
                sceneGraph.RenderScene(camera, modelShader);
                screenFBO.Unbind();

                //Apply gaussian blur to highlight texture from first render pass
                GL.Viewport(0, 0, gaussianBlurFBO.width, gaussianBlurFBO.height);
                gaussianBlurFBO.Bind();
                GL.Clear(ClearBufferMask.DepthBufferBit);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                blurShader.Bind();
                GL.Uniform1(blurShader.uniform_screenTexture, 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, screenFBO.GetTargetTextureId(1));
                blurShader.Unbind();

                quad.Render(blurShader);
                for (int i = 0; i < 1; i++)
                {
                    GL.Clear(ClearBufferMask.DepthBufferBit);

                    blurShader.Bind();
                    GL.Uniform1(blurShader.uniform_screenTexture, 0);
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, gaussianBlurFBO.GetTargetTextureId(0));
                    blurShader.Unbind();

                    quad.Render(blurShader);
                }
                gaussianBlurFBO.Unbind();

                //Render the final state of post processing
                GL.Viewport(0, 0, screenFBO.width, screenFBO.height);
                postProcessingShader.Bind();
                GL.Uniform1(postProcessingShader.uniform_screenTexture, 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, screenFBO.GetTargetTextureId(0));

                GL.Uniform1(postProcessingShader.uniform_blurTexture, 1);
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, gaussianBlurFBO.GetTargetTextureId(0));

                GL.Uniform1(postProcessingShader.uniform_depthTexture, 2);
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, screenFBO.GetTargetTextureId(2));
                postProcessingShader.Unbind();

                quad.Render(postProcessingShader);
            } else
            {
                GL.Viewport(0, 0, screen.width, screen.height);
                skybox.Render(camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix());
                sceneGraph.RenderScene(camera, modelShader);
            }
        }
    }
}
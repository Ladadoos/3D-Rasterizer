using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;

namespace Template
{
    class Consts
    {
        public static int LightsCount = 1;
    }

    class MyApplication
    {
        public Surface screen;                  // background surface for printing etc.
        Model dragon, sphere1, sphere2, centerBox, towerBoxBig, towerBoxSmall;
        Model floorBottom, glassBall, shinyBall;
        float a = 0;
        RenderTarget screenFBO;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        public static Camera camera;
        SceneGraph sceneGraph;
        CubeTexture skyboxTexture;
        Skybox skybox;
        PointLight skylight, light2;

        RenderTarget godRayTarget;
        RenderTarget godRayTargetScene;

        //Shaders
        private DepthShader depthShader = new DepthShader();
        private ModelShader modelShader = new ModelShader();
        private PostProcessingShader postProcessingShader = new PostProcessingShader();
        private SkyboxShader skyboxShader = new SkyboxShader();
        private GodRayShader godRayShader = new GodRayShader();
        private GodRayShaderScene godRayShaderScene = new GodRayShaderScene();

        //Assets
        private List<Mesh> meshesAsset = new List<Mesh>();
        private List<SurfaceTexture> texturesAsset = new List<SurfaceTexture>();

        // initialize
        public void Init()
        {
            Random random = new Random();

            sceneGraph = new SceneGraph();

            meshesAsset.Add(new Mesh("../../assets/dragon.obj")); //0
            meshesAsset.Add(new Mesh("../../assets/teapot.obj")); //1
            meshesAsset.Add(new Mesh("../../assets/floor.obj")); //2
            meshesAsset.Add(new Mesh("../../assets/sphere.obj")); //3
            meshesAsset.Add(new Mesh("../../assets/cube.obj")); //4
            meshesAsset.Add(new Mesh("../../assets/palm.obj")); //5
            meshesAsset.Add(new Mesh("../../assets/grass.obj")); //6

            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/wood.jpg"), null, 4, MaterialType.Diffuse)); //0
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseGray.png"), null, 8, MaterialType.Reflective, new CubeTexture(256, 256))); //1
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/floor.png"), new Texture("../../assets/floorNormal.png"), 8, MaterialType.Diffuse)); //2
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseGreen.png"), null, 2, MaterialType.Diffuse)); //3
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/grass.png"), null, 2, MaterialType.Diffuse)); //4
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/dirt.png"), new Texture("../../assets/dirtnormal.png"), 16, MaterialType.Diffuse)); //5
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/gunMetalGray.jpg"), null, 0.5F, MaterialType.Diffuse)); //6
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/stoneDiffuse.jpg"), new Texture("../../assets/stoneNormal.jpg"), 16, MaterialType.Diffuse)); //7
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseBlue.png"), null, 16, MaterialType.Dieletric, new CubeTexture(256, 256))); //8

            dragon = new Model(meshesAsset[0], texturesAsset[6], new Vector3(80, 0, 80), new Vector3(0, 65, 0), new Vector3(7));
            sphere1 = new Model(meshesAsset[3], texturesAsset[0], new Vector3(3, 0, 0), Vector3.Zero, new Vector3(0.5F));
            sphere2 = new Model(meshesAsset[3], texturesAsset[0], new Vector3(0, -0.2F, 2), Vector3.Zero, new Vector3(0.3F, 0.3F, 0.3F));
            centerBox = new Model(meshesAsset[4], texturesAsset[0], new Vector3(0, 45, 0), Vector3.Zero, new Vector3(25));
            towerBoxBig = new Model(meshesAsset[4], texturesAsset[7], new Vector3(-70, 12, -70), new Vector3(0, 45, 0), new Vector3(23));
            towerBoxSmall = new Model(meshesAsset[4], texturesAsset[7], new Vector3(-70, 28, -70), new Vector3(0, 15, 0), new Vector3(12));
            glassBall = new Model(meshesAsset[3], texturesAsset[8], new Vector3(70, 28, -70), new Vector3(0, 15, 0), new Vector3(16));
            shinyBall = new Model(meshesAsset[3], texturesAsset[1], new Vector3(-70, 28, 70), new Vector3(0, 15, 0), new Vector3(16));

            for (int i = 0; i < 50; i++)
            {
                sceneGraph.gameObjects.Add(new Model(
                    meshesAsset[6],
                    texturesAsset[4],
                    new Vector3(random.Next(-150, 150), -0.2F, random.Next(-150, 150)),
                    new Vector3(0, random.Next(360), 0),
                    new Vector3(12 + random.Next(16)))
                );
            }

            floorBottom = new Model(meshesAsset[2], texturesAsset[5], new Vector3(0, 40, 0), Vector3.Zero, new Vector3(20));

            // skylight = new PointLight(meshesAsset[3], texturesAsset[3], new Vector3(300, 400, 0), Vector3.Zero, Vector3.One);
            // skylight.color = new Vector3(1f, 0.5F, 0.1F); skylight.brightness = 70000;
            // skylight.CreateDepth(new CubeDepthMap(1024, 1024));

            light2 = new PointLight(meshesAsset[3], texturesAsset[3], new Vector3(0, 105, 0), Vector3.Zero, new Vector3(1));
            light2.color = new Vector3(1f, 0.5F, 0.1F); light2.brightness = 10000;
            light2.CreateDepth(new CubeDepthMap(512, 512));

            // create the render target
            screenFBO = new RenderTarget(2, screen.width, screen.height);
            godRayTarget = new RenderTarget(1, screen.width, screen.height);
            godRayTargetScene = new RenderTarget(1, screen.width, screen.height);

            quad = new ScreenQuad();
            camera = new FPSCamera(new Vector3(-100, 100, 0));

            centerBox.AddChild(sphere2);
            sphere2.AddChild(sphere1);

            sceneGraph.gameObjects.Add(dragon);
            //sceneGraph.gameObjects.Add(skylight);
            sceneGraph.gameObjects.Add(light2);
            sceneGraph.gameObjects.Add(sphere1);
            sceneGraph.gameObjects.Add(centerBox);
            sceneGraph.gameObjects.Add(towerBoxBig);
            sceneGraph.gameObjects.Add(towerBoxSmall);
            sceneGraph.gameObjects.Add(floorBottom);
            sceneGraph.gameObjects.Add(sphere2);
            sceneGraph.gameObjects.Add(glassBall);
            sceneGraph.gameObjects.Add(shinyBall);

            // sceneGraph.AddLight(skylight);
            sceneGraph.AddLight(light2);

            skyboxTexture = new CubeTexture(new string[]{ "../../assets/right2.png", "../../assets/left2.png", "../../assets/top2.png",
                "../../assets/bottom2.png", "../../assets/front2.png", "../../assets/back2.png" });
            skybox = new Skybox();
        }

        public void OnWindowResize(int width, int height)
        {
            screenFBO.width = width;
            screenFBO.height = height;
        }

        public void Tick(OpenTKApp app, float deltaTime)
        {

        }

        // tick for OpenGL rendering code
        public void RenderGL(OpenTKApp app, float deltaTime)
        {
            camera.ProcessInput(app, deltaTime);

            // update rotation
            a += 15 * deltaTime;
            if (a > 360) { a -= 360; }
            float cos = (float)Math.Cos(MathHelper.DegreesToRadians(a));
            float sin = (float)Math.Sin(MathHelper.DegreesToRadians(a));

            sphere2.rotationInAngle.Y += deltaTime * 100;
            centerBox.rotationInAngle.Y += deltaTime * 50;
            centerBox.position.Y += sin / 5;
            light2.position.X = 155 * cos;
            light2.position.Z = 155 * sin;

            camera.CalculateFrustumPlanes();
            sceneGraph.UpdateScene(camera);
            sceneGraph.RenderDepthMap(camera, depthShader);
            sceneGraph.UpdateEnvironmentMaps(camera, modelShader, skyboxShader, skybox, skyboxTexture);

            GL.Viewport(0, 0, screenFBO.width, screenFBO.height);

            screenFBO.Bind();
            Matrix4 viewProjMatrix = camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix();
            skybox.Render(skyboxShader, skyboxTexture.cubeMapId, viewProjMatrix);
            sceneGraph.RenderScene(camera, modelShader);
            screenFBO.Unbind();

            Vector4 clipSpacePos = new Vector4(sceneGraph.lights[0].globalTransform.ExtractTranslation(), 1)
                                     * camera.GetViewMatrix() * camera.GetProjectionMatrix();
            Vector3 ndcSpacePos = clipSpacePos.Xyz / clipSpacePos.W;
            Vector2 screenSpace = ((ndcSpacePos.Xy + new Vector2(1)) / 2);

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            GL.ClearColor(new Color4(0.01f, 0.01f, 0.01f, 1));
            GL.Clear(ClearBufferMask.ColorBufferBit);

            godRayTargetScene.Bind();
            sceneGraph.RenderScene(camera, godRayShaderScene);
            godRayTargetScene.Unbind();

            godRayShader.Bind();
            godRayShader.LoadVector2(godRayShader.uniform_lightPositionScreen, screenSpace);
            godRayShader.Unbind();

            godRayTarget.Bind();
            quad.Render(godRayShader, godRayTargetScene.GetTargetTextureId(0), screenFBO.GetTargetTextureId(0), screenFBO.GetTargetTextureId(0));
            godRayTarget.Unbind();

            quad.Render(postProcessingShader, screenFBO.GetTargetTextureId(0), screenFBO.GetTargetTextureId(1), godRayTarget.GetTargetTextureId(0));
        }
    }
}
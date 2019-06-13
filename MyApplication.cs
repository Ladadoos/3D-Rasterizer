using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    class Consts
    {
        public static int LightsCount = 2;
    }

    class MyApplication
    {
        public Surface screen;                  // background surface for printing etc.
        Model dragon, sphere1, sphere2, centerBox, towerBoxBig, towerBoxSmall;
        Model floorBottom, glassBall, shinyBall;
        float a = 0;
        ScreenQuad quad;                        // screen filling quad for post processing
        public static Camera camera;
        SceneGraph sceneGraph;
        CubeTexture skyboxTexture;
        Skybox skybox;
        PointLight skylight, light2;

        //Frame buffer objects
        private RenderTarget gaussianBlurFBO;
        private RenderTarget screenFBO; 

        //Shaders
        private DepthShader depthShader = new DepthShader();
        private ModelShader modelShader = new ModelShader();
        private PostProcessingShader postProcessingShader = new PostProcessingShader();
        private SkyboxShader skyboxShader = new SkyboxShader();
        private GaussianBlurShader blurShader = new GaussianBlurShader();

        //Assets
        private List<Mesh> meshesAsset = new List<Mesh>();
        private List<SurfaceTexture> texturesAsset = new List<SurfaceTexture>();

        public void Initialize()
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

            skylight = new PointLight(meshesAsset[3], texturesAsset[3], new Vector3(300, 400, 0), Vector3.Zero, Vector3.One);
            skylight.color = new Vector3(1f, 0.5F, 0.1F); skylight.brightness = 70000;
            skylight.CreateDepth(new CubeDepthMap(1024, 1024));

            light2 = new PointLight(meshesAsset[3], texturesAsset[3], new Vector3(0, 105, 0), Vector3.Zero, new Vector3(4));
            light2.color = new Vector3(1f, 0.5F, 0.1F); light2.brightness = 100000;
            light2.CreateDepth(new CubeDepthMap(512, 512));

            // create the render target
            screenFBO = new RenderTarget(2, screen.width, screen.height);
            gaussianBlurFBO = new RenderTarget(1, screen.width, screen.height);

            quad = new ScreenQuad();
            camera = new FPSCamera(new Vector3(-100, 100, 0));

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

            sceneGraph.AddLight(skylight);
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
            // update rotation
            a += 30 * deltaTime;
            if (a > 360) { a -= 360; }
            float cos = (float)Math.Cos(MathHelper.DegreesToRadians(a));
            float sin = (float)Math.Sin(MathHelper.DegreesToRadians(a));
            sphere2.rotationInAngle.Y += deltaTime * 100;
            centerBox.rotationInAngle.Y += deltaTime * 50;
            centerBox.position.Y += sin / 10;

            light2.position.X = 155 * cos;
            light2.position.Z = 155 * sin * cos;

            if(camera.ProcessInput(app, deltaTime))
            {
                camera.CalculateFrustumPlanes();
            }           
            sceneGraph.UpdateScene(camera);
            sceneGraph.RenderDepthMap(camera, depthShader);
            sceneGraph.UpdateEnvironmentMaps(camera, modelShader, skyboxShader, skybox, skyboxTexture);

            GL.Viewport(0, 0, screenFBO.width, screenFBO.height);
            screenFBO.Bind();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Matrix4 viewProjMatrix = camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix();
            skybox.Render(skyboxShader, skyboxTexture.cubeMapId, viewProjMatrix);
            sceneGraph.RenderScene(camera, modelShader);
            screenFBO.Unbind();

            GL.Viewport(0, 0, gaussianBlurFBO.width, gaussianBlurFBO.height);
            gaussianBlurFBO.Bind();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            quad.Render(blurShader, screenFBO.GetTargetTextureId(1), screenFBO.GetTargetTextureId(0));
            gaussianBlurFBO.Unbind();
            for (int i = 0; i < 3; i++)
            {
                gaussianBlurFBO.Bind();
                GL.Clear(ClearBufferMask.DepthBufferBit);
                quad.Render(blurShader, gaussianBlurFBO.GetTargetTextureId(0), screenFBO.GetTargetTextureId(0));
                gaussianBlurFBO.Unbind();
            }

            quad.Render(postProcessingShader, screenFBO.GetTargetTextureId(0), gaussianBlurFBO.GetTargetTextureId(0));
        }
    }
}
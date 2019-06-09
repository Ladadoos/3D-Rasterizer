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
        Model floorBottom;
        float a = 0;
        RenderTarget screenFBO;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        public static Camera camera;
        SceneGraph sceneGraph;
        bool useRenderTarget = true;
        CubeTexture skyboxTexture;
        Skybox skybox;
        PointLight skylight, light2;

        //Shaders
        private DepthShader depthShader = new DepthShader();
        private ModelShader modelShader = new ModelShader();
        private PostProcessingShader postProcessingShader = new PostProcessingShader();
        private SkyboxShader skyboxShader = new SkyboxShader();

        //Assets
        private List<Mesh> meshesAsset = new List<Mesh>();
        private List<SurfaceTexture> texturesAsset = new List<SurfaceTexture>();

        // initialize
        public void Init()
        {
            Random random = new Random();

            sceneGraph = new SceneGraph();

            meshesAsset.Add(new Mesh("../../assets/dragon.obj"));
            meshesAsset.Add(new Mesh("../../assets/teapot.obj"));
            meshesAsset.Add(new Mesh("../../assets/floor.obj"));
            meshesAsset.Add(new Mesh("../../assets/sphere.obj"));
            meshesAsset.Add(new Mesh("../../assets/cube.obj"));
            meshesAsset.Add(new Mesh("../../assets/palm.obj"));
            meshesAsset.Add(new Mesh("../../assets/grass.obj"));

            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/wood.jpg"), null, 4)); 
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseGray.png"), null, 8));
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/floor.png"), new Texture("../../assets/floorNormal.png"), 8));
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseGreen.png"), null, 2));
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/grass.png"), null, 2));
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/dirt.png"), new Texture("../../assets/dirtnormal.png"), 16));
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/gunMetalGray.jpg"), null, 0.5F));
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/stoneDiffuse.jpg"), new Texture("../../assets/stoneNormal.jpg"), 16));

            dragon = new Model(meshesAsset[0], texturesAsset[6], new Vector3(80, 0, 80), new Vector3(0,65 , 0), new Vector3(5));
            sphere1 = new Model(meshesAsset[3], texturesAsset[0], new Vector3(3, 0, 0), Vector3.Zero, new Vector3(0.5F));
            sphere2 = new Model(meshesAsset[3], texturesAsset[0], new Vector3(0, -0.2F, 2), Vector3.Zero, new Vector3(0.3F, 0.3F, 0.3F));
            centerBox = new Model(meshesAsset[4], texturesAsset[0], new Vector3(0, 45, 0), Vector3.Zero, new Vector3(25));
            towerBoxBig = new Model(meshesAsset[4], texturesAsset[7], new Vector3(-70, 12, -70), new Vector3(0, 45, 0), new Vector3(23));
            towerBoxSmall = new Model(meshesAsset[4], texturesAsset[7], new Vector3(-70, 28, -70), new Vector3(0, 15, 0), new Vector3(12));

            for (int i = 0; i < 125; i++)
            {
                sceneGraph.gameObjects.Add(new Model(
                    meshesAsset[6], 
                    texturesAsset[4], 
                    new Vector3(random.Next(-150, 150), 0, random.Next(-150, 150)), 
                    new Vector3(0, random.Next(360), 0) ,
                    new Vector3(12 + random.Next(16)))
                );
            }

            floorBottom = new Model(meshesAsset[2], texturesAsset[5], new Vector3(0, 40, 0), Vector3.Zero, new Vector3(20));

            skylight = new PointLight(meshesAsset[3], texturesAsset[1], new Vector3(300, 400, 0), Vector3.Zero, Vector3.One);
            skylight.color = new Vector3(1f, 0.5F, 0.1F); skylight.brightness = 70000;
            skylight.CreateDepth(new CubeDepthMap(1024, 1024));

            light2 = new PointLight(meshesAsset[3], texturesAsset[1], new Vector3(0, 105, 0), Vector3.Zero, new Vector3(4));
            light2.color = new Vector3(1f, 0.5F, 0.1F); light2.brightness = 10000;
            light2.CreateDepth(new CubeDepthMap(512, 512));

            // create the render target
            screenFBO = new RenderTarget(2, screen.width, screen.height);
            
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
            camera.ProcessInput(app, deltaTime);

            // update rotation
            a += 15 * deltaTime;
            if (a > 360) { a -= 360; }
            float cos = (float)Math.Cos(MathHelper.DegreesToRadians(a));
            float sin = (float)Math.Sin(MathHelper.DegreesToRadians(a));

            sphere2.rotationInAngle.Y += deltaTime * 100;
            centerBox.rotationInAngle.Y += deltaTime * 50;
            centerBox.position.Y += sin / 5;
           // light2.color.Y += 0.01F ;
           // dragon.rotationInAngle.Y = 100 * cos;

            light2.position.X = (float)(155 * cos);
            light2.position.Z = (float)(155 * sin);

            camera.CalculateFrustumPlanes();
            sceneGraph.UpdateScene(camera);

            if (useRenderTarget)
            {
                sceneGraph.RenderDepthMap(camera, depthShader);

                GL.Viewport(0, 0, screenFBO.width, screenFBO.height);
                screenFBO.Bind();
                Matrix4 viewProjMatrix = camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix();
                skybox.Render(skyboxShader, skyboxTexture.id, viewProjMatrix);
                sceneGraph.RenderScene(camera, modelShader);
                screenFBO.Unbind();

                quad.Render(postProcessingShader, screenFBO.GetTargetTextureId(0), screenFBO.GetTargetTextureId(1));
            } else
            {
                sceneGraph.RenderDepthMap(camera, depthShader);

                GL.Viewport(0, 0, screenFBO.width, screenFBO.height);
                Matrix4 viewProjMatrix = camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix();
                skybox.Render(skyboxShader, skyboxTexture.id, viewProjMatrix);
                sceneGraph.RenderScene(camera, modelShader);
            }
        }
    }
}
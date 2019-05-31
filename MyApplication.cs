using OpenTK;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Template.Mesh;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;                  // background surface for printing etc.
        Model dragon, teapot2, teapot3, floor;
        float a = 0;
        RenderTarget target;                    // intermediate render target
        ScreenQuad quad;                        // screen filling quad for post processing
        public static Camera camera;
        SceneGraph sceneGraph;
        bool useRenderTarget = true;
        DepthMap depthMap;
        CubeTexture skyboxTexture;
        Skybox skybox;
        Light light;

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
            meshesAsset.Add(new Mesh("../../assets/dragon.obj"));
            meshesAsset.Add(new Mesh("../../assets/teapot.obj"));
            meshesAsset.Add(new Mesh("../../assets/floor.obj"));
            meshesAsset.Add(new Mesh("../../assets/sphere.obj"));

            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/wood.jpg"), null)); 
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/diffuseGray.png"), null));
            texturesAsset.Add(new SurfaceTexture(new Texture("../../assets/floor.png"), new Texture("../../assets/floorNormal.png")));

            dragon = new Model(meshesAsset[0], texturesAsset[1], new Vector3(0, -25, 0), Vector3.Zero, new Vector3(7));
            teapot2 = new Model(meshesAsset[1], texturesAsset[0], new Vector3(15, -25, 15), Vector3.Zero, Vector3.One);
            teapot3 = new Model(meshesAsset[1], texturesAsset[0], new Vector3(0, 5, 10), Vector3.Zero, new Vector3(0.25F, 0.25F, 0.25F));
            floor = new Model(meshesAsset[2], texturesAsset[2], new Vector3(0, 13, 0), Vector3.Zero, new Vector3(20, 20, 20));
            light = new Light(meshesAsset[3], texturesAsset[1], new Vector3(25, 65, 25), Vector3.Zero, Vector3.One);
            light.color = new Vector3(1F, 1F, 0.9F);

            // create the render target
            target = new RenderTarget(screen.width, screen.height);
            quad = new ScreenQuad();
            camera = new FPSCamera(new Vector3(0, -15, 0));
            sceneGraph = new SceneGraph();

            sceneGraph.hierarchy = new GraphTree<GameObject>();
            GraphNode<GameObject> root = new GraphNode<GameObject>(dragon);
            root.AddChild(new GraphNode<GameObject>(teapot3));
            sceneGraph.hierarchy.rootNodes.Add(root);
            sceneGraph.hierarchy.rootNodes.Add(new GraphNode<GameObject>(light));
            sceneGraph.hierarchy.rootNodes.Add(new GraphNode<GameObject>(teapot2));
            sceneGraph.hierarchy.rootNodes.Add(new GraphNode<GameObject>(floor));
            sceneGraph.lights.Add(light);

            depthMap = new DepthMap(screen.width, screen.height);

            skyboxTexture = new CubeTexture(new string[]{ "../../assets/right.png", "../../assets/left.png", "../../assets/top.png",
                "../../assets/bottom.png", "../../assets/front.png", "../../assets/back.png" });
            skybox = new Skybox();
        }

        public void OnWindowResize(int width, int height)
        {
            target.width = width;
            target.height = height;
        }

        // tick for background surface
        public void Tick(OpenTKApp app, float deltaTime)
        {
            //screen.Clear(0);
            // screen.Print("ok", 2, 2, 0xffff00);
            camera.ProcessInput(app, deltaTime);
        }

        // tick for OpenGL rendering code
        public void RenderGL(float deltaTime)
        {
            // update rotation
            a += 50f * deltaTime;
            if (a > 360) { a -= 360; }
            dragon.rotationInAngle.Y = a;
            teapot3.rotationInAngle.X = a*2;
            //teapot3.rotationInAngle.Y = a*5;
            light.position.X = (float)(125 * Math.Cos(MathHelper.DegreesToRadians(a)));
            light.position.Z = (float)(125 * Math.Sin(MathHelper.DegreesToRadians(a)));

            camera.UpdateFrustumPoints();
            sceneGraph.PrepareMatrices();
            if (useRenderTarget)
            {
                depthMap.Bind();
                sceneGraph.RenderDepthMap(camera, depthShader);
                depthMap.Unbind();

                target.Bind();
                Matrix4 viewProjMatrix = camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix();
                skybox.Render(skyboxShader, skyboxTexture, viewProjMatrix);
                sceneGraph.RenderScene(camera, modelShader, depthMap);
                target.Unbind();

                quad.Render(postProcessingShader, target.GetTextureID());
            } else
            {
                // render scene directly to the screen
                depthMap.Bind();
                sceneGraph.RenderDepthMap(camera, depthShader);
                depthMap.Unbind();

                Matrix4 viewProjMatrix = camera.GetViewMatrix() * camera.GetProjectionMatrix();
                skybox.Render(skyboxShader, skyboxTexture, viewProjMatrix);
                sceneGraph.RenderScene(camera, modelShader, depthMap);
            }
        }
    }
}
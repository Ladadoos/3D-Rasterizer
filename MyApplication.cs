using OpenTK;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Rasterizer
{
    class Consts
    {
        public static int LightsCount = 2;
    }

    class MyApplication
    {
        private bool enablePostProcessing = true;
        private bool enableDepthOfField = true;
        private bool enableFirstPersonCamera = true;
        private bool enableShadows = true;
        private bool enableBloom = true;

        public Surface screen;      
        private ScreenQuad quad = new ScreenQuad(); 
        private Camera camera;
        private SceneGraph sceneGraph = new SceneGraph();
        private Skybox skybox = new Skybox();
        private PointLight skylight, light2;
        private Texture[] depthOfFieldBlurTextures = new Texture[3];

        //Models
        private Model dragon, sphere1, sphere2, centerBox, towerBoxBig, towerBoxSmall;
        private Model floorBottom, glassBall, shinyBall, tower;

        //Frame buffer objects
        private RenderTarget verBlurFilterFBO, horBlurFilterFBO;
        private RenderTarget depthOfFieldFBO;
        private RenderTarget multisampleScreenFBO, screenFBO;

        //Shaders
        private DepthShader depthShader = new DepthShader();
        private ModelShader modelShader = new ModelShader();
        private PostProcessingShader postProShader = new PostProcessingShader();
        private BlurFilterShader blurFilterShader = new BlurFilterShader();
        private DepthOfFieldShader depthOfFieldShader = new DepthOfFieldShader();

        //Assets
        private List<Mesh> meshesAsset = new List<Mesh>();
        private List<SurfaceTexture> texturesAsset = new List<SurfaceTexture>();

        public void Initialize()
        {
            modelShader.Bind();
            modelShader.LoadBoolean(modelShader.uniform_enableShadows, enableShadows);
            modelShader.Unbind();

            if (enableDepthOfField)
            {
                for (int i = 0; i < 3; i++)
                {
                    depthOfFieldBlurTextures[i] = new Texture(screen.width, screen.height);
                }
            }

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

            for (int i = 0; i < 25; i++)
            {
                if (random.Next(4) == 1)
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

            skylight = new PointLight(meshesAsset[3], texturesAsset[3], new Vector3(300, 400, 0), Vector3.Zero, new Vector3(10));
            skylight.color = new Vector3(1f, 0.5F, 0.1f); skylight.brightness = 70000;
            skylight.CreateDepth(new CubeDepthMap(1024, 1024));
            light2 = new PointLight(meshesAsset[3], texturesAsset[3], new Vector3(0, 155, 0), Vector3.Zero, new Vector3(4));
            light2.color = new Vector3(1f, 0.5F, 0.1F); light2.brightness = 80000;
            light2.CreateDepth(new CubeDepthMap(512, 512));

            screenFBO = new RenderTarget(3, screen.width, screen.height);
            multisampleScreenFBO = new RenderTarget(3, screen.width, screen.height, 2);
            verBlurFilterFBO = new RenderTarget(1, screen.width, screen.height);
            horBlurFilterFBO = new RenderTarget(1, screen.width, screen.height);
            depthOfFieldFBO = new RenderTarget(1, screen.width, screen.height);

            if (enableFirstPersonCamera)
            {
                camera = new FPSCamera(new Vector3(-100, 150, 0), screen.width, screen.height);
            } else
            {
                camera = new TopDownCamera(new Vector3(-100, 150, 0), screen.width, screen.height);
            }
            
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

            selectedLight.color.X = MathHelper.Clamp(selectedLight.color.X, 0.01F, 1);
            selectedLight.color.Y = MathHelper.Clamp(selectedLight.color.Y, 0.01F, 1);
            selectedLight.color.Z = MathHelper.Clamp(selectedLight.color.Z, 0.01F, 1);
        }

        private float a = 0;
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

            if (camera.ProcessInput(app, deltaTime)) //only update frustum if camera moved
            {
                camera.CalculateFrustumPlanes();
            }
            sceneGraph.UpdateScene(camera);
            if (enableShadows)
            {
                sceneGraph.RenderDepthMap(depthShader);
            }           
            sceneGraph.UpdateEnvironmentMaps(modelShader, skybox);

            if (enablePostProcessing)
            {
                //Do first render pass to multisample fbo
                multisampleScreenFBO.Bind();
                GL.Viewport(0, 0, multisampleScreenFBO.width, multisampleScreenFBO.height);
                GL.Clear(ClearBufferMask.DepthBufferBit);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                skybox.Render(camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix());
                GL.ClearTexImage(multisampleScreenFBO.GetTargetTextureId(2), 0, PixelFormat.Rgba, PixelType.Float, new float[] { 1, 1, 1, 1 });
                GL.ClearTexImage(multisampleScreenFBO.GetTargetTextureId(1), 0, PixelFormat.Rgba, PixelType.Float, new float[] { 0, 0, 0, 0 });
                sceneGraph.RenderScene(camera, modelShader);
                multisampleScreenFBO.Unbind();

                //Resolve multisample to normal fbo (non multisample) 
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, multisampleScreenFBO.fbo);
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, screenFBO.fbo);
                for(int i = 0; i < multisampleScreenFBO.colorTextures.Length; i++)
                {
                    GL.ReadBuffer(ReadBufferMode.ColorAttachment0 + i);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0 + i);
                    GL.BlitFramebuffer(0, 0, multisampleScreenFBO.width, multisampleScreenFBO.height, 0, 0,
                        screenFBO.width, screenFBO.height, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
                }
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                if (enableBloom)
                {
                    //Apply blur to texture with only the bright parts from first render pass
                    blurFilterShader.Bind();
                    blurFilterShader.LoadInt32(blurFilterShader.uniform_kernelWidth, 5);
                    for (int i = 0; i < 3; i++)
                    {
                        horBlurFilterFBO.Bind();
                        GL.Viewport(0, 0, horBlurFilterFBO.width, horBlurFilterFBO.height);
                        GL.Clear(ClearBufferMask.DepthBufferBit);
                        blurFilterShader.LoadTexture(blurFilterShader.uniform_screenTexture, 0,
                            i == 0 ? screenFBO.GetTargetTextureId(1) : verBlurFilterFBO.GetTargetTextureId(0));
                        blurFilterShader.LoadBoolean(blurFilterShader.uniform_isHorizontalPass, true);
                        quad.Render(blurFilterShader);

                        verBlurFilterFBO.Bind();
                        GL.Viewport(0, 0, verBlurFilterFBO.width, verBlurFilterFBO.height);
                        GL.Clear(ClearBufferMask.DepthBufferBit);
                        blurFilterShader.LoadTexture(blurFilterShader.uniform_screenTexture, 0, horBlurFilterFBO.GetTargetTextureId(0));
                        blurFilterShader.LoadBoolean(blurFilterShader.uniform_isHorizontalPass, false);
                        quad.Render(blurFilterShader);
                    }
                    blurFilterShader.Unbind();
                    verBlurFilterFBO.Unbind();

                    //Copy the blurred texture with the highlighted parts to its original fbo to free up the blur fbo
                    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, verBlurFilterFBO.fbo);
                    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, screenFBO.fbo);
                    GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment1);
                    GL.BlitFramebuffer(0, 0, verBlurFilterFBO.width, verBlurFilterFBO.height, 0, 0,
                            screenFBO.width, screenFBO.height, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                }

                if (enableDepthOfField)
                {
                    //Copy the normal rendered texture from first render pass to blur fbo
                    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, screenFBO.fbo);
                    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, verBlurFilterFBO.fbo);
                    GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
                    GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
                    GL.BlitFramebuffer(0, 0, screenFBO.width, screenFBO.height, 0, 0,
                            verBlurFilterFBO.width, verBlurFilterFBO.height, ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit, BlitFramebufferFilter.Nearest);
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

                    //Generate three layers of blurred textures by continously storing the current blurred texture and applying extra layer of blur
                    blurFilterShader.Bind();
                    for (int i = 0; i < 3; i++)
                    {
                        blurFilterShader.LoadInt32(blurFilterShader.uniform_kernelWidth, 3 + i * 2);

                        horBlurFilterFBO.Bind();
                        GL.Viewport(0, 0, horBlurFilterFBO.width, horBlurFilterFBO.height);
                        GL.Clear(ClearBufferMask.DepthBufferBit);
                        blurFilterShader.LoadTexture(blurFilterShader.uniform_screenTexture, 0, verBlurFilterFBO.GetTargetTextureId(0));
                        blurFilterShader.LoadBoolean(blurFilterShader.uniform_isHorizontalPass, true);
                        quad.Render(blurFilterShader);

                        verBlurFilterFBO.Bind();
                        GL.Viewport(0, 0, verBlurFilterFBO.width, verBlurFilterFBO.height);
                        GL.Clear(ClearBufferMask.DepthBufferBit);
                        blurFilterShader.LoadTexture(blurFilterShader.uniform_screenTexture, 0, horBlurFilterFBO.GetTargetTextureId(0));
                        blurFilterShader.LoadBoolean(blurFilterShader.uniform_isHorizontalPass, false);
                        quad.Render(blurFilterShader);

                        //Take snapshot of current blurred texture by storing it. Image is being taken from vertical blur fbo
                        GL.BindTexture(TextureTarget.Texture2D, depthOfFieldBlurTextures[i].id);
                        GL.CopyTexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 0, 0, verBlurFilterFBO.width, verBlurFilterFBO.height);
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                    }
                    blurFilterShader.Unbind();
                    verBlurFilterFBO.Unbind();

                    //Apply the depth of field effect on the original rendered texture
                    depthOfFieldShader.Bind();
                    depthOfFieldShader.LoadTexture(depthOfFieldShader.uniform_screenTexture, 0, screenFBO.GetTargetTextureId(0));
                    depthOfFieldShader.LoadTexture(depthOfFieldShader.uniform_depthTexture, 1, screenFBO.GetTargetTextureId(2));
                    depthOfFieldShader.LoadTexture(depthOfFieldShader.uniform_blurTextureOne, 2, depthOfFieldBlurTextures[0].id);
                    depthOfFieldShader.LoadTexture(depthOfFieldShader.uniform_blurTextureTwo, 3, depthOfFieldBlurTextures[1].id);
                    depthOfFieldShader.LoadTexture(depthOfFieldShader.uniform_blurTextureThree, 4, depthOfFieldBlurTextures[2].id);

                    depthOfFieldFBO.Bind();
                    GL.Viewport(0, 0, depthOfFieldFBO.width, depthOfFieldFBO.height);
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                    quad.Render(depthOfFieldShader);
                    depthOfFieldFBO.Unbind();
                    depthOfFieldShader.Unbind();
                }

                //Render the final state of post processing
                GL.Viewport(0, 0, screen.width, screen.height);
                postProShader.Bind();
                postProShader.LoadTexture(postProShader.uniform_screenTexture, 0, 
                    enableDepthOfField ? depthOfFieldFBO.GetTargetTextureId(0) : screenFBO.GetTargetTextureId(0));
                postProShader.LoadTexture(postProShader.uniform_bloomBlurTexture, 1, screenFBO.GetTargetTextureId(1));
                postProShader.LoadTexture(postProShader.uniform_depthTexture, 2, screenFBO.GetTargetTextureId(2));
                quad.Render(postProShader);
                postProShader.Unbind();
            } else
            {
                GL.Viewport(0, 0, screen.width, screen.height);
                skybox.Render(camera.GetViewMatrix().ClearTranslation() * camera.GetProjectionMatrix());
                sceneGraph.RenderScene(camera, modelShader);
            }

            sceneGraph.EndUpdateScene();
        }
    }
}
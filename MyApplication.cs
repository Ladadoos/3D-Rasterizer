using OpenTK;

namespace Template
{
    class MyApplication
	{
		// member variables
		public Surface screen;                  // background surface for printing etc.
        Model teapot1, teapot2, teapot3, floor;
		float a = 0;                            // teapot rotation angle
		ModelShader modelShader;                          // shader to use for rendering
		PostProcessingShader postproc;                        // shader to use for post processing
		Texture wood;                           // texture to use for rendering
		RenderTarget target;                    // intermediate render target
		ScreenQuad quad;                        // screen filling quad for post processing
        public static Camera camera;
        SceneGraph sceneGraph;
		bool useRenderTarget = true;

		// initialize
		public void Init()
		{
            teapot1 = new Model("../../assets/teapot.obj", Vector3.Zero, Vector3.Zero, Vector3.One);
            teapot2 = new Model("../../assets/teapot.obj", new Vector3(0, 0, 20), Vector3.Zero, Vector3.One);
            teapot3 = new Model("../../assets/teapot.obj", new Vector3(0, 0, 10), Vector3.Zero, Vector3.One);
            floor = new Model("../../assets/floor.obj", Vector3.Zero, Vector3.Zero, Vector3.One);
            Light light = new Light(string.Empty, new Vector3(20, 5, 20), Vector3.Zero, Vector3.One);
            light.color = new Vector3(0.8F, 0.8F, 0.8F);

            // create shaders
            modelShader = new ModelShader();
            postproc = new PostProcessingShader();

			// load a texture
			wood = new Texture( "../../assets/wood.jpg" );
			// create the render target
			target = new RenderTarget( screen.width, screen.height );
			quad = new ScreenQuad();
            camera = new Camera(new Vector3(0, -15, 0));
            sceneGraph = new SceneGraph();

            GraphNode<GameObject> root = new GraphNode<GameObject>(teapot1);
            GraphNode<GameObject> child = new GraphNode<GameObject>(teapot2);
            GraphNode<GameObject> child2 = new GraphNode<GameObject>(teapot3);
            child.AddChild(child2);
            root.AddChild(child);          
            sceneGraph.hierarchy = new GraphTree<GameObject>();
            sceneGraph.hierarchy.rootNodes.Add(root);
            sceneGraph.hierarchy.rootNodes.Add(new GraphNode<GameObject>(floor));
            sceneGraph.hierarchy.rootNodes.Add(new GraphNode<GameObject>(light));
            sceneGraph.lights.Add(light);
        }

		// tick for background surface
		public void Tick(float deltaTime)
		{
			screen.Clear( 0 );
			screen.Print( "hello world", 2, 2, 0xffff00 );
            camera.ProcessInput(deltaTime);
		}

		// tick for OpenGL rendering code
		public void RenderGL(float deltaTime)
		{
			// update rotation
			a += 1f * deltaTime;
			if( a > 360) { a -= 360; }
            teapot1.rotationInAngle.Y = a;
            teapot3.rotationInAngle.Y = a;
            teapot2.rotationInAngle.Y = a;

            if ( useRenderTarget )
			{
				// enable render target
				target.Bind();

                // render scene to render target
                sceneGraph.Render(camera, modelShader, wood);

				// render quad
				target.Unbind();
				quad.Render( postproc, target.GetTextureID() );
			}
			else
			{
                // render scene directly to the screen
                sceneGraph.Render(camera, modelShader, wood);
            }
		}
	}
}
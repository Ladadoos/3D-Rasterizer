using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;                  // background surface for printing etc.
		Mesh teapot1, floor, teapot2, teapot3;           // a mesh to draw using OpenGL
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

            teapot1 = new Mesh("../../assets/teapot.obj", Vector3.Zero, Vector3.Zero, new Vector3(0.5F, 0.5F, 0.5F));
            teapot2 = new Mesh("../../assets/teapot.obj", new Vector3(0, 0, 20), Vector3.Zero, new Vector3(0.5F, 0.5F, 0.5F));
            teapot3 = new Mesh("../../assets/teapot.obj", new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0.5F, 0.5F, 0.5F));
            floor = new Mesh("../../assets/floor.obj", Vector3.Zero, Vector3.Zero, new Vector3(4, 4, 4));

            GraphNode<Mesh> root = new GraphNode<Mesh>(teapot1);
            GraphNode<Mesh> child = new GraphNode<Mesh>(teapot2);
            GraphNode<Mesh> child2 = new GraphNode<Mesh>(teapot3);
            child.AddChild(child2);
            root.AddChild(child);          
            sceneGraph.hierarchy = new GraphTree<Mesh>();
            sceneGraph.hierarchy.rootNodes.Add(root);
            sceneGraph.hierarchy.rootNodes.Add(new GraphNode<Mesh>(floor));
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
			a += 50f * deltaTime;
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
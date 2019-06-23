Rasterizer by Dion Kamerman, 6410359

Before I start my readme, I want to thank you guys (TA's and lecturers) for this awesome course. Best one so far and I am sad it needs to come to an end :/

Features I added:
- Player controlled camera:
	- Top down camera: WASD to move, QE to rotate and mousewheel to zoom in and out
	- First person camera: WASD to move and mouse to look around
- Scenegraph as described in the requirements: objects can be children of other objects, which affects their global position. Lights can be added to scengraph too
- Improved shader class. I knew I wanted to get into post processing, so splitting the shader code into seperate classes was a must
- Pointlights using Phong shading model. 
- Smoothed shadows from pointlights using depthcubes. My shadows cause inter-object shadows and self-shadowing
- Colorful skybox using a cubemap
- Option to use normal map on meshes. If no normal map is provided, then the normals from the .obj file are used
- Support for multiple lights.
- Cube mapping (environment maps, shadow maps, skybox etc...)
- Frustum culling by creating bounding spheres around each object and checking if that bounding sphere is inside the view frustum or not
- HDR rendertarget and HDR glow
- Sepperable box and gaussian filter with variable kernel width
- Reflective and dieletric materials with textures using environmental maps that update real time
- Interactive lights:
	- Press numbers 0 to 9 to select light 0 to 9 
	- Press - or + to toggle between adding or subtracting to the color of the current selected light
	- Press r, g or b to change the color of the selected light, depending on the mode (+ or -) currently selected
- Multisample anti-aliasing (MSAA) on frame buffer objects. 
	- The multiple different render targets on the frame buffer object used for the first render pass are set to 2x MSAA. Sample count can be changed.
- Post processing effects:
	- Chromatic abberation
	- Vignette
	- Color invert
	- Fog by using the depth buffer
	- Depth of field with auto focus by using the depth buffer and three different levels of blurred images. Try to move (far) away from the scene and focus on different things. Unfortunately didn't add smooth focus transition though.
	- Small feature to be able to apply non-seperable filter kernels at the last step of post processing
		- Two 5x5 filter kernels added as example
	
A few extra notes:
- In MyApplication.cs there are a couple of toggleable settings. They are all turned on by default. This includes:
	- Enable postprocessing, enable depth of field, enable first person camera, enable shadows and enable bloom
	- You can turn off Depth of field to see the chromatic abberation more clearly.
- In fs_blurFilter.glsl in the main function you can call applyBoxBlur() instead of applyGaussianBlur() to view a box blur. I made the standard blur gaussian because it looks better
- I made the standard camera the first person camera, because only then you can admire the beauty (or I atleast hope you find it beautiful) But as mentioned before, this can be toggled to top down camera (to meet the requirements)
- I also tried to add per-object motion blur using velocity buffer. I got it semi-working at some point, but I wasn't fully satisfied with it so I just left it out.
- Another feature I tried to add was screen space god rays. I added two screenshots of it. They work well enough if the screenshot is taken at a right angle. It's not included in the project because again, I didn't like how it worked enough.
- In fs_post.glsl there is a boolean to toggle fog and to change fog intensity. In the main function of this file you can also toggle (filter kernel) effects.
- The reason the depth buffer isn't encoded in the alpha channel is because I was thinking of actually adding transparent objects, but got side tracked by other features.
- I tested all my features on three different machines and it worked on all three. I hope it works for you too.
- I included some awesome screenshots of my work, if I do say so myself. I am proud of them.
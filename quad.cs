﻿using System;
using OpenTK.Graphics.OpenGL;

namespace Rasterizer
{
    public class ScreenQuad
	{
		// data members
		int vbo_idx = 0, vbo_vert = 0;
		float[] vertices = { -1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, -1, 0, 1, 0, -1, -1, 0, 0, 0 };
		int[] indices = { 0, 1, 2, 3 };

		// initialization; called during first render
		public void Prepare()
		{
			if( vbo_vert == 0 )
			{
				// prepare VBO for quad rendering
				GL.GenBuffers( 1, out vbo_vert );
				GL.BindBuffer( BufferTarget.ArrayBuffer, vbo_vert );
				GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(4 * 5 * 4), vertices, BufferUsageHint.StaticDraw );
				GL.GenBuffers( 1, out vbo_idx );
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, vbo_idx );
				GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(16), indices, BufferUsageHint.StaticDraw );
			}
		}

		// render the mesh using the supplied shader and matrix
		public void Render(PostProcessingShader shader)
		{
			// on first run, prepare buffers
			Prepare();

			// enable position and uv attributes
			GL.EnableVertexAttribArray( shader.attribute_position );
			GL.EnableVertexAttribArray( shader.attribute_uv );

			// bind interleaved vertex data
			GL.EnableClientState( ArrayCap.VertexArray );
			GL.BindBuffer( BufferTarget.ArrayBuffer, vbo_vert );
			GL.InterleavedArrays( InterleavedArrayFormat.T2fV3f, 20, IntPtr.Zero );

			// link vertex attributes to shader parameters 
			GL.VertexAttribPointer( shader.attribute_position, 3, VertexAttribPointerType.Float, false, 20, 0 );
			GL.VertexAttribPointer( shader.attribute_uv, 2, VertexAttribPointerType.Float, false, 20, 3 * 4 );

			// bind triangle index data and render
			GL.BindBuffer( BufferTarget.ElementArrayBuffer, vbo_idx );
			GL.DrawArrays( PrimitiveType.Quads, 0, 4 );
		}
    }
}
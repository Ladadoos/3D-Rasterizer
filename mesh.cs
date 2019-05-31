using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    // mesh and loader based on work by JTalton; http://www.opentk.com/node/642

    public class Mesh
    {
        // data members
        public ObjVertex[] vertices;            // vertex positions, model space
        public ObjTriangle[] triangles;         // triangles (3 vertex indices)
        public ObjQuad[] quads;                 // quads (4 vertex indices)
        int vertexBufferId;                     // vertex buffer
        int triangleBufferId;                   // triangle buffer
        int quadBufferId;                       // quad buffer

        public Vector3 hitboxCenter;
        public float hitboxRadius;

        // constructor
        public Mesh( string fileName)
		{
			MeshLoader loader = new MeshLoader();
			loader.Load( this, fileName );
        }

        // initialization; called during first render
        public void Prepare()
		{
			if( vertexBufferId == 0 )
			{
				// generate interleaved vertex data (uv/normal/position (total 8 floats) per vertex)
				GL.GenBuffers( 1, out vertexBufferId );
				GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBufferId );
				GL.BufferData( BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf( typeof( ObjVertex ) )), vertices, BufferUsageHint.StaticDraw );

				// generate triangle index array
				GL.GenBuffers( 1, out triangleBufferId );
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, triangleBufferId );
				GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf( typeof( ObjTriangle ) )), triangles, BufferUsageHint.StaticDraw );

				// generate quad index array
				GL.GenBuffers( 1, out quadBufferId );
				GL.BindBuffer( BufferTarget.ElementArrayBuffer, quadBufferId );
				GL.BufferData( BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf( typeof( ObjQuad ) )), quads, BufferUsageHint.StaticDraw );
			}
		}

        // render the mesh using the supplied shader and matrix
        public void RenderToDepth(DepthShader shader, Matrix4 transform, Matrix4 viewProjection)
        {
            // on first run, prepare buffers
            Prepare();

            // enable shader
            GL.UseProgram(shader.programID);

            // safety dance
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            // pass transform to vertex shader
            GL.UniformMatrix4(shader.uniform_modelMatrix, false, ref transform);
            GL.UniformMatrix4(shader.uniform_viewProjectionMatrix, false, ref viewProjection);

            // enable position
            GL.EnableVertexAttribArray(shader.attribute_position);

            // bind interleaved vertex data
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
            GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf(typeof(ObjVertex)), IntPtr.Zero);

            // link vertex attributes to shader parameters 
            GL.VertexAttribPointer(shader.attribute_position, 3, VertexAttribPointerType.Float, false, 56, 5 * 4);

            // bind triangle index data and render
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, triangles.Length * 3);

            // bind quad index data and render
            if (quads.Length > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
                GL.DrawArrays(PrimitiveType.Quads, 0, quads.Length * 4);
            }

            // restore previous OpenGL state
            GL.UseProgram(0);
            GL.PopClientAttrib();
        }

        // render the mesh using the supplied shader and matrix
        public void RenderToScene(ModelShader shader, Matrix4 transform, Matrix4 view, Matrix4 projection, 
            Matrix4 lightMatrix, GameObject gameObject, DepthMap depthMap)
		{
			// on first run, prepare buffers
			Prepare();

            // enable shader
            GL.UseProgram(shader.programID);

            // safety dance
            GL.PushClientAttrib( ClientAttribMask.ClientVertexArrayBit );

            // enable texture
            GL.Uniform1(shader.uniform_textureMap, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gameObject.texture.diffuse.id);

            GL.Uniform1(shader.uniform_depthMap, 1);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, depthMap.depthMapId);

            int useNormalMap = gameObject.texture.normal == null ? 0 : 1;
            if (useNormalMap == 1)
            {
                GL.Uniform1(shader.uniform_normalMap, 2);
                GL.ActiveTexture(TextureUnit.Texture2);
                GL.BindTexture(TextureTarget.Texture2D, gameObject.texture.normal.id);
            }
            GL.Uniform1(shader.uniform_useNormalMap, useNormalMap);

            // pass transform to vertex shader
            GL.UniformMatrix4(shader.uniform_modelMatrix, false, ref transform);
            GL.UniformMatrix4(shader.uniform_viewMatrix, false, ref view);
            GL.UniformMatrix4(shader.uniform_projectionMatrix, false, ref projection);
            GL.UniformMatrix4(shader.uniform_lightSpaceMatrix, false, ref lightMatrix);

            // enable position, normal and uv attributes
            GL.EnableVertexAttribArray(shader.attribute_position);
            GL.EnableVertexAttribArray(shader.attribute_normal);
            GL.EnableVertexAttribArray(shader.attribute_uv);
            GL.EnableVertexAttribArray(shader.attribute_tangent);
            GL.EnableVertexAttribArray(shader.attribute_bitangent);

            // bind interleaved vertex data
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
            GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf(typeof(ObjVertex)), IntPtr.Zero);

            // link vertex attributes to shader parameters 
            GL.VertexAttribPointer(shader.attribute_uv, 2, VertexAttribPointerType.Float, false, 56, 0);
            GL.VertexAttribPointer(shader.attribute_normal, 3, VertexAttribPointerType.Float, true, 56, 2 * 4);
            GL.VertexAttribPointer(shader.attribute_position, 3, VertexAttribPointerType.Float, false, 56, 5 * 4);
            GL.VertexAttribPointer(shader.attribute_tangent, 3, VertexAttribPointerType.Float, true, 56, 8 * 4);
            GL.VertexAttribPointer(shader.attribute_bitangent, 3, VertexAttribPointerType.Float, true, 56, 11 * 4);

            // bind triangle index data and render
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, triangleBufferId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, triangles.Length * 3);

            // bind quad index data and render
            if (quads.Length > 0)
            {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadBufferId);
                GL.DrawArrays(PrimitiveType.Quads, 0, quads.Length * 4);
            }

            // restore previous OpenGL state
            GL.UseProgram(0);
            GL.PopClientAttrib();
        }

        // layout of a single vertex
        [StructLayout( LayoutKind.Sequential )]
		public struct ObjVertex
		{
			public Vector2 TexCoord;
			public Vector3 Normal;
			public Vector3 Vertex;
            public Vector3 Tangent;
            public Vector3 Bitangent;
		}

		// layout of a single triangle
		[StructLayout( LayoutKind.Sequential )]
		public struct ObjTriangle
		{
			public int Index0, Index1, Index2;
		}

		// layout of a single quad
		[StructLayout( LayoutKind.Sequential )]
		public struct ObjQuad
		{
			public int Index0, Index1, Index2, Index3;
		}
	}
}
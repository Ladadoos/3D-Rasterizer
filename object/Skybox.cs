using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace Template
{
    class Skybox
    {
        private int positionsVboId;
        private SkyboxShader skyboxShader = new SkyboxShader();
        private CubeTexture skyboxTexture;

        private static float[] skyboxVertices =
        {
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,

            -1.0f, -1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f, -1.0f,  1.0f,
            -1.0f, -1.0f,  1.0f,

            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f,  1.0f,
             1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f,  1.0f,
            -1.0f,  1.0f, -1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f,  1.0f,
             1.0f, -1.0f,  1.0f
        };

        public Skybox()
        {
            skyboxTexture = new CubeTexture(new string[]{ "../../assets/right.png", "../../assets/left.png", "../../assets/top.png",
                "../../assets/bottom.png", "../../assets/front.png", "../../assets/back.png" }); ;

            positionsVboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionsVboId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(skyboxVertices.Length * sizeof(float)), skyboxVertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Render(Matrix4 viewProjectionMatrix)
        {
            GL.DepthMask(false);
            skyboxShader.Bind();

            skyboxShader.LoadMatrix(skyboxShader.uniform_viewProjectionMatrix, viewProjectionMatrix);

            GL.BindBuffer(BufferTarget.ArrayBuffer, positionsVboId);
            GL.EnableVertexAttribArray(skyboxShader.attribute_position);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.BindTexture(TextureTarget.TextureCubeMap, skyboxTexture.cubeMapId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            GL.DisableVertexAttribArray(skyboxShader.attribute_position);
            GL.DepthMask(true);
            skyboxShader.Unbind();
        }
    }
}

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Template
{
    public class CubeTexture
    {
        public int cubeMapId, cubeFBOId;
        private int depthBuffer;
        public int width, height;

        public CubeTexture(string[] fileNames)
        {
            cubeMapId = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, cubeMapId);
            for(int i = 0; i < 6; i++)
            {
                Bitmap bmp = new Bitmap(fileNames[i]);
                this.width = bmp.Width;
                this.height = bmp.Height;
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
                bmp.UnlockBits(bmp_data);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        }

        public CubeTexture(int width, int height)
        {
            this.width = width;
            this.height = height;

            GL.GenTextures(1, out cubeMapId);
            GL.BindTexture(TextureTarget.TextureCubeMap, cubeMapId);
            for (int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, width, height,
                    0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);
            }
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.Ext.GenFramebuffers(1, out cubeFBOId);
            GL.Ext.GenRenderbuffers(1, out depthBuffer);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, cubeFBOId);

            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthBuffer);
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, (RenderbufferStorage)All.DepthComponent24, this.width, this.height);
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);

            GL.Ext.FramebufferTexture(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, cubeMapId, 0);
            bool untestedBoolean = RenderTarget.CheckFBOStatus();
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        public void Bind()
        {
            GL.CullFace(CullFaceMode.Front);
            GL.Viewport(0, 0, width, height);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, cubeFBOId);
        }

        public void SetRenderSide(int side)
        {
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, 
                TextureTarget.TextureCubeMapPositiveX + side, cubeMapId, 0);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public void Unbind()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            GL.CullFace(CullFaceMode.Back);
        }
    }
}

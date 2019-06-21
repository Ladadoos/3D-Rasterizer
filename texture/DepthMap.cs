using System;
using OpenTK.Graphics.OpenGL;

namespace Rasterizer
{
    public class DepthMap
    {
        public int depthMapFBO, depthMapId;
        int width, height;

        public DepthMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            GL.Ext.GenFramebuffers(1, out depthMapFBO);

            GL.GenTextures(1, out depthMapId);
            GL.BindTexture(TextureTarget.Texture2D, depthMapId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            float[] borderColor = { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, depthMapFBO);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, depthMapId, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            bool untestedBoolean = RenderTarget.CheckFBOStatus();
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        public void Bind()
        {
            GL.CullFace(CullFaceMode.Front);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, depthMapFBO);
            GL.Viewport(0, 0, width, height);
            GL.Clear(ClearBufferMask.DepthBufferBit);
        }

        public void Unbind()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            GL.CullFace(CullFaceMode.Back);
        }
    }
}

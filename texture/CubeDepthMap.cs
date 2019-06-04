using OpenTK.Graphics.OpenGL;
using System;

namespace Template
{
    public class CubeDepthMap
     {
         public int depthMapFBO, cubeDepthMapId;
         public int width, height;

         public CubeDepthMap(int width, int height)
         {
             this.width = width;
             this.height = height;

             GL.GenTextures(1, out cubeDepthMapId);
             GL.BindTexture(TextureTarget.TextureCubeMap, cubeDepthMapId);
             for (int i = 0; i < 6; i++)
             {
                 GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.DepthComponent, width, height, 
                     0, OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
             }
             GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
             GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
             GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
             GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
             GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

             GL.Ext.GenFramebuffers(1, out depthMapFBO);
             GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, depthMapFBO);
             GL.Ext.FramebufferTexture(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment, cubeDepthMapId, 0);
             GL.DrawBuffer(DrawBufferMode.None);
             GL.ReadBuffer(ReadBufferMode.None);
             bool untestedBoolean = RenderTarget.CheckFBOStatus();
             GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
         }

         public void Bind() 
         {
             GL.CullFace(CullFaceMode.Front);
             GL.Viewport(0, 0, width, height);
             GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, depthMapFBO);
         }

         public void SetRenderSide(int side)
         {           
             GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment, TextureTarget.TextureCubeMapPositiveX + side, cubeDepthMapId, 0);
             GL.Clear(ClearBufferMask.DepthBufferBit);
         }

         public void Unbind()
         {
             GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
             GL.CullFace(CullFaceMode.Back);
         }
     }

    //https://stackoverflow.com/questions/20659299/shadow-mapping-in-opengl-es-2-0-with-cubemap
    /*public class CubeDepthMap
    {
        public int depthMapFBO, cubeDepthMapId, depthTextureId;
        public int width, height;

        public CubeDepthMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            GL.Ext.GenFramebuffers(1, out depthMapFBO);

            //depth texture
            GL.GenTextures(1, out depthTextureId);
            GL.BindTexture(TextureTarget.Texture2D, depthTextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent24, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //cube texture
            GL.GenTextures(1, out cubeDepthMapId);
            GL.BindTexture(TextureTarget.TextureCubeMap, cubeDepthMapId);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            for (int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.R32f, width, height, 0, 
                    OpenTK.Graphics.OpenGL.PixelFormat.Red, PixelType.Float, IntPtr.Zero);
            }

            //framebuffer
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, depthMapFBO);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment,TextureTarget.Texture2D, depthTextureId, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            bool untestedBoolean = RenderTarget.CheckFBOStatus();
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        public void Bind()
        {
            // GL.CullFace(CullFaceMode.Front);
            GL.Viewport(0, 0, width, height);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, depthMapFBO);
            

            //GL.Clear(ClearBufferMask.DepthBufferBit);
            // GL.ClearColor(Color.Black);
        }

        public void SetRenderSide(int side)
        {
           //GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, depthMapFBO);
           GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0, TextureTarget.TextureCubeMapPositiveX + side, cubeDepthMapId, 0);
            
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }

        public void Unbind()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            //GL.CullFace(CullFaceMode.Back);
        }
    }*/
}
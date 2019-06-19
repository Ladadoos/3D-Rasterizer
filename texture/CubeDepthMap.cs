using OpenTK.Graphics.OpenGL;
using System;

namespace Rasterizer
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
}
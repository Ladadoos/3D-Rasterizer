﻿using System;
using OpenTK.Graphics.OpenGL;

// based on http://www.opentk.com/doc/graphics/frame-buffer-objects

namespace Rasterizer
{
    class RenderTarget
    {
        public int width, height;

        public int fbo;
        public int[] colorTextures;
        private int depthBuffer;
        private int samples;

        public RenderTarget(int colorTexturesCount, int width, int height, int pixelSamples = 1)
        {
            this.width = width;
            this.height = height;
            this.samples = pixelSamples;
            colorTextures = new int[colorTexturesCount];

            // create and bind fbo
            GL.Ext.GenFramebuffers(1, out fbo);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fbo);

            // create the color buffers
            DrawBuffersEnum[] buffers = new DrawBuffersEnum[colorTexturesCount];
            for (int i = 0; i < colorTexturesCount; i++)
            {
                buffers[i] = DrawBuffersEnum.ColorAttachment0 + i;
            }
            GL.DrawBuffers(colorTexturesCount, buffers);

            // create the color textures
            GL.GenTextures(colorTexturesCount, colorTextures);
            if (pixelSamples == 1)
            {
                for (int i = 0; i < colorTexturesCount; i++)
                {
                    GL.BindTexture(TextureTarget.Texture2D, colorTextures[i]);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f,
                        this.width, this.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

                    GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0 + i,
                        TextureTarget.Texture2D, colorTextures[i], 0);
                }
                GL.BindTexture(TextureTarget.Texture2D, 0);
            } else
            {
                for (int i = 0; i < colorTexturesCount; i++)
                {
                    GL.BindTexture(TextureTarget.Texture2DMultisample, colorTextures[i]);
                    GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, pixelSamples, PixelInternalFormat.Rgba32f,
                        this.width, this.height, true);

                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
                    GL.TexParameter(TextureTarget.Texture2DMultisample, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

                    GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0 + i,
                        TextureTarget.Texture2DMultisample, colorTextures[i], 0);
                }
                GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
            }

            // create depth buffer
            GL.Ext.GenRenderbuffers(1, out depthBuffer);
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthBuffer);
            if (pixelSamples == 1)
            {
                GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, (RenderbufferStorage)All.DepthComponent24, this.width, this.height);
            } else
            {
                GL.Ext.RenderbufferStorageMultisample(RenderbufferTarget.RenderbufferExt, pixelSamples, (RenderbufferStorage)All.DepthComponent24, this.width, this.height);
            }          
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer);

            // test fbo integrity
            bool untestedBoolean = CheckFBOStatus();
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to regular framebuffer
        }

        public int GetTargetTextureId(int id)
        {
            return colorTextures[id];
        }

        public void Bind()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fbo);
        }
        public void Unbind()
        {
            // return to regular framebuffer
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }
        public static bool CheckFBOStatus()
        {
            FramebufferErrorCode code = GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
            switch (code)
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    Console.WriteLine("FBO: The framebuffer is complete and valid for rendering.");
                    return true;
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    Console.WriteLine("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    Console.WriteLine("FBO: There are no attachments.");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    Console.WriteLine("FBO: Attachments are of different size. All attachments must have the same width and height.");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    Console.WriteLine("FBO: The color attachments have different format. All color attachments must have the same format.");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    Console.WriteLine("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                    break;
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    Console.WriteLine("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                    break;
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    Console.WriteLine("FBO: This particular FBO configuration is not supported by the implementation.");
                    break;
                default:
                    Console.WriteLine("FBO: Status unknown: " + code);
                    break;
            }
            return false;
        }
    }
}
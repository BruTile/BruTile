using System;
using OpenTK.Graphics.ES20;

namespace BruTile.Samples.VectorTileToBitmap
{
    internal class FrameBufferObjectHelper
    {
        public static void StopFrameBufferObject()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // disable rendering into the FBO
            GL.Enable(EnableCap.Texture2D); // enable Texture Mapping
            GL.BindTexture(TextureTarget.Texture2D, 0); // bind default texture
        }

        public static void StartFrameBufferObject(int width, int height)
        {
            uint colorTexture;
            uint depthTexture;
            uint fboHandle;

            // Create Color Tex
            GL.GenTextures(1, out colorTexture);
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.ClampToEdge);
            // GL.Ext.GenerateMipmap( GenerateMipmapTarget.Texture2D );

            // Create Depth Tex
            GL.GenTextures(1, out depthTexture);
            GL.BindTexture(TextureTarget.Texture2D, depthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat) All.DepthComponent32Oes, width, height, 0,
                PixelFormat.DepthComponent, PixelType.UnsignedByte, IntPtr.Zero);
            // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.ClampToEdge);

            // Create a FBO and attach the textures
            GL.GenFramebuffers(1, out fboHandle);
            GL.BindFramebuffer(All.Framebuffer, fboHandle);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0,
                TextureTarget.Texture2D,
                (int) colorTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment,
                TextureTarget.Texture2D,
                (int) depthTexture, 0);
        }
    }
}
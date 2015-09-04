using System;
using System.Collections.Generic;
using GeoJSON.Net.Feature;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;

namespace FrameBufferObject2DToBitmap
{
    class OpenTKRasterizer 
    {
        private static void Set2DViewport(int width, int height)
        {
            OpenTK.Graphics.ES11.GL.MatrixMode(OpenTK.Graphics.ES11.MatrixMode.Projection);
            OpenTK.Graphics.ES11.GL.LoadIdentity();

            OpenTK.Graphics.OpenGL.GL.Ortho(0, width, height, 0, 0, 1); // This has no effect: OpenTK.Graphics.ES11.GL.Ortho(0, width, height, 0, 0, 1); 
        }

        public byte[] Rasterize(int width, int height, Action drawMethod)
        {
            // There needs to be a gamewindow even though we don't write to screen. It is created but not used explicitly in our code.
            var gameWindow = new GameWindow(width, height);

            Set2DViewport(width, height);

            uint colorTexture;
            uint depthTexture;
            uint fboHandle;

            if (!GL.GetString(StringName.Extensions).Contains("GL_EXT_framebuffer_object"))
            {
                throw new NotSupportedException(
                     "GL_EXT_framebuffer_object extension is required. Please update your drivers.");
            }

            // Create Color Tex
            GL.GenTextures(1, out colorTexture);
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            // GL.Ext.GenerateMipmap( GenerateMipmapTarget.Texture2D );

            // Create Depth Tex
            GL.GenTextures(1, out depthTexture);
            GL.BindTexture(TextureTarget.Texture2D, depthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32Oes, width, height, 0, PixelFormat.DepthComponent, PixelType.UnsignedByte, IntPtr.Zero);
            // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            // GL.Ext.GenerateMipmap( GenerateMipmapTarget.Texture2D );

            // Create a FBO and attach the textures
            GL.GenFramebuffers(1, out fboHandle);
            GL.BindFramebuffer(All.Framebuffer, fboHandle);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.ColorAttachment0, TextureTarget.Texture2D, (int)colorTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferSlot.DepthAttachment, TextureTarget.Texture2D, (int)depthTexture, 0);
            
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            drawMethod();

            var byteArray = GraphicsContextToBitmapConverter.ToBitmap(width, height);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // disable rendering into the FBO
            GL.Enable(EnableCap.Texture2D); // enable Texture Mapping
            GL.BindTexture(TextureTarget.Texture2D, 0); // bind default texture

            return byteArray;
        }
    }
}

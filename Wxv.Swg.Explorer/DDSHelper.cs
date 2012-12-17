using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Drawing;
using System.Drawing.Imaging;
using AlphaSubmarines;
using System.Runtime.InteropServices;

namespace Wxv.Xna
{
    public static class DDSHelper
    {
        private static Lazy<Form> LazyForm = new Lazy<Form>(() => new Form());
        public static Form Form { get { return LazyForm.Value; } }

        private static Lazy<GraphicsDevice> LazyGraphicsDevice = new Lazy<GraphicsDevice>(() => new GraphicsDevice(
            GraphicsAdapter.DefaultAdapter,
            GraphicsProfile.HiDef,
            new PresentationParameters
            {
                IsFullScreen = false,
                DeviceWindowHandle = Form.Handle,
            }));
        public static GraphicsDevice GraphicsDevice { get { return LazyGraphicsDevice.Value; } }

        public static Texture2D LoadTexture2D(Stream stream)
        {
            Texture2D result;
            AlphaSubmarines.DDSLib.DDSFromStream(stream, GraphicsDevice, 0, false, out result);
            return result;
        }

        public static Texture2D LoadTexture2D(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                Texture2D result;
                AlphaSubmarines.DDSLib.DDSFromStream(stream, GraphicsDevice, 0, false, out result);
                return result;
            }
        }

        public static Texture2D LoadTexture2D(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                Texture2D result;
                AlphaSubmarines.DDSLib.DDSFromStream(stream, GraphicsDevice, 0, false, out result);
                return result;
            }
        }


        public static Bitmap LoadBitmap(Stream stream, System.Drawing.Color? backgroundColor = null)
        {
            Bitmap result = null;

            int width, height;
            Bitmap bitmap32;

            using (var textureStream = new MemoryStream())
            {
                using (var texture = LoadTexture2D(stream))
                {
                    width = texture.Width;
                    height = texture.Height;
                    texture.SaveAsPng(textureStream, width, height);
                }

                textureStream.Seek(0, SeekOrigin.Begin);

                bitmap32 = Bitmap.FromStream(textureStream) as Bitmap;

                if (backgroundColor.HasValue)
                {
                    using (bitmap32)
                    {
                        result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                        try
                        {
                            using (var graphics = Graphics.FromImage(result))
                            {
                                using (var solidBrush = new SolidBrush(backgroundColor.Value))
                                    graphics.FillRectangle(solidBrush, 0, 0, width, height);
                                graphics.DrawImageUnscaled(bitmap32, 0, 0);
                            }
                        }
                        catch
                        {
                            result.Dispose();
                            throw;
                        }
                    }
                }
                else
                    result = bitmap32;
            }

            /*
            if (withoutTransparency)
            {
                var bitmapData = result.LockBits(
                    new System.Drawing.Rectangle(0, 0, width, height),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);
                try
                {
                    // create byte array to copy pixel values
                    var pixelData = new byte[width * height * 4];
                    var iptr = bitmapData.Scan0;

                    // Copy data from pointer to array
                    Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);

                    // modify the data
                    byte la = 255, ha = 0;
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                        {
                            int i = ((y * width) + x) * 4;
                            byte a = pixelData[i + 3];
                            if (a < la)
                                la = a;
                            if (a > ha)
                                ha = a;

                            pixelData[i + 3] = 255;
                        }
                    System.Diagnostics.Debug.WriteLine("la:{0} , ha:{1}", la, ha);

                    // Copy data from byte array to pointer
                    Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
                }
                finally
                {
                    result.UnlockBits(bitmapData);
                }
            }
                */

            return result;
        }

        public static Bitmap LoadBitmap(byte[] data, System.Drawing.Color? backgroundColor = null)
        {
            using (var stream = new MemoryStream(data))
                return LoadBitmap(stream, backgroundColor);
        }

    }
}

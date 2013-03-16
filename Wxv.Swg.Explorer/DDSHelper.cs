using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Wxv.Swg.Explorer
{
    public static class DDSHelper
    {
        private static Lazy<Form> LazyRenderForm = new Lazy<Form>(() => new Form());
        public static Form RenderForm { get { return LazyRenderForm.Value; } }

        private static Device CreateDevice()
        {
            var presentParameters = new PresentParameters()
            {
                Windowed = true,
                SwapEffect = SwapEffect.Discard,
                BackBufferFormat = Format.Unknown,
                AutoDepthStencilFormat = DepthFormat.D16,
                EnableAutoDepthStencil = true
            };

            var deviceCaps = Manager.GetDeviceCaps(Manager.Adapters.Default.Adapter, DeviceType.Hardware).DeviceCaps;
            var createFlags = ((deviceCaps.SupportsHardwareTransformAndLight) 
                ? CreateFlags.HardwareVertexProcessing 
                : CreateFlags.SoftwareVertexProcessing);
            if (deviceCaps.SupportsPureDevice)
                createFlags |= CreateFlags.PureDevice;

            var device = new Device(0, DeviceType.Hardware, RenderForm, createFlags, presentParameters);
            device.RenderState.CullMode = Cull.None;
            device.RenderState.Lighting = false;

            device.DeviceReset += (sender, e) => 
            {
                System.Diagnostics.Debug.WriteLine("DeviceReset");
                var d = (Device)sender;
                d.RenderState.CullMode = Cull.None;
                d.RenderState.Lighting = false;
            };
            device.DeviceLost += (sender, e) => { System.Diagnostics.Debug.WriteLine("DeviceLost"); };
            device.DeviceResizing += (sender, e) => { System.Diagnostics.Debug.WriteLine("DeviceResizing"); };
            
            return device;
        }

        private static Lazy<Device> LazyDevice = new Lazy<Device>(() => CreateDevice());
        public static Device Device { get { return LazyDevice.Value; } }

        public static Bitmap LoadBitmap(Stream stream, System.Drawing.Color? backgroundColor = null)
        {
            using (var textureStream = new MemoryStream())
            using (var texture = TextureLoader.FromStream(Device, stream))
            using (var graphicStream = TextureLoader.SaveToStream(ImageFileFormat.Png, texture))
            {
                graphicStream.Seek(0, SeekOrigin.Begin);

                var bitmap = Bitmap.FromStream(graphicStream) as Bitmap;

                if (!backgroundColor.HasValue)
                    return bitmap;


                using (bitmap)
                {
                    var bitmapWithBackground = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
                    try
                    {
                        using (var graphics = Graphics.FromImage(bitmapWithBackground))
                        {
                            using (var solidBrush = new SolidBrush(backgroundColor.Value))
                                graphics.FillRectangle(solidBrush, 0, 0, bitmap.Width, bitmap.Height);
                            graphics.DrawImageUnscaled(bitmap, 0, 0);

                            return bitmapWithBackground;
                        }
                    }
                    catch
                    {
                        bitmapWithBackground.Dispose();
                        throw;
                    }
                }
            }
        }

        public static Bitmap LoadBitmap(byte[] data, System.Drawing.Color? backgroundColor = null)
        {
            using (var stream = new MemoryStream(data))
                return LoadBitmap(stream, backgroundColor);
        }

    }
}

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

//using Microsoft.Xna;
//using Microsoft.Xna.Framework;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

//using Wxv.Swg.Common;
//using Wxv.Swg.Common.Files;
//using Wxv.Swg.Common.Exporters;

namespace Wxv.Swg.Test
{
    public class Program
    {
        /*
        static void TestSkeleton()
        {
            using (var repository = Repository.Load(@"c:\swgemu"))
            {
                var skeletonFile = repository.Load<SkeletonFile>(
                    @"appearance/skeleton/all_b.skt", 
                    stream => new SkeletonFileReader().Load(stream));

                //foreach(var b in skeletonFile.Skeletons.First().Bones
                //    .Where(b => b.Parent == null || (b.Parent.Parent == null) || (b.Parent.Parent.Parent == null)))
                //{
                //    b.ToString(Console.Out); 
                //    Console.WriteLine();
                //}

                //skeletonFile.Skeletons.First().Bones.First(b => b.Name == "root").ToString(Console.Out);
                //Console.WriteLine();
                //skeletonFile.Skeletons.First().Bones.First(b => b.Name == "lThigh").ToString(Console.Out);
                //Console.WriteLine();
                //skeletonFile.Skeletons.First().Bones.First(b => b.Name == "rThigh").ToString(Console.Out);

                skeletonFile.Skeletons.First().ToString(Console.Out);

                new ColladaSkeletonExporter(repository, skeletonFile).Export(@"C:\Users\user\Desktop\all_b.dae");
            }
        }

        static void TestQuat()
        {
            var offset = new Vector3 { X = 2, Y = 2, Z = 2 };
            var preRotation = new Quaternion { X = 0, Y = 0, Z = 0, W = 1 };
            var postRotation = new Quaternion { X = -1, Y = 0, Z = 0, W = 0 };

            var offsetMatrix = Matrix.CreateTranslation(offset);
            var matrixPreRotation = Matrix.CreateFromQuaternion(preRotation);
            var matrixPostRotation = Matrix.CreateFromQuaternion(postRotation);
            var m = offsetMatrix * matrixPreRotation * matrixPostRotation;

            Console.WriteLine("aa * r :  " + Vector3.Transform(Vector3.Zero, m).ToFormatString());
            Console.WriteLine();
        }
        */

        static void TestDirect3D()
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

            using (var renderForm = new Form())
            using (var device = new Device(0, DeviceType.Hardware, renderForm, createFlags, presentParameters))
            {
                device.RenderState.CullMode = Cull.None;
                device.RenderState.Lighting = false;

                using (var stream = File.OpenRead("ui_load_tatooine.dds"))
                using (var texture = TextureLoader.FromStream(device, stream))
                {
                    Console.WriteLine("LevelOfDetail: {0}", texture.LevelOfDetail);

                    // TextureLoader.Save("ui_load_tatooine.png", ImageFileFormat.Png, texture);

                    using (var graphicStream = TextureLoader.SaveToStream(ImageFileFormat.Png, texture))
                    using (var outputStream = File.Create("ui_load_tatooine.png"))
                    {
                        graphicStream.CopyTo(outputStream);
                    }
                }

                
                //this.m_Device.DeviceReset += new EventHandler(this.OnResetDevice);
                //this.m_Device.DeviceLost += new EventHandler(this.OnDeviceLost);
                //this.m_Device.DeviceResizing += new CancelEventHandler(this.OnDeviceResizing);
                //OnResetDevice(this.m_Device, null);
            }


        }

        static void Main()
        {
            //TestQuat();
            //TestSkeleton();
            TestDirect3D();
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}

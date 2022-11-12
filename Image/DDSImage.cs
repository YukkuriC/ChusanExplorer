using Pfim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChusanExplorer{
    public class DDSImage
    {
        public string ImgPath { get; private set; }

        public DDSImage(string path)
        {
            ImgPath = path;
        }

        public bool loadFailed = false;
        public bool Valid
        {
            get => !loadFailed && File.Exists(ImgPath);
        }

        Bitmap bitmap;
        public Bitmap Image
        {
            get
            {
                if (Valid && bitmap == null)
                {
                    try
                    {
                        using (var image = Pfimage.FromFile(ImgPath))
                        {
                            PixelFormat format;
                            // Convert from Pfim's backend agnostic image format into GDI+'s image format
                            switch (image.Format)
                            {
                                case Pfim.ImageFormat.Rgba32:
                                    format = PixelFormat.Format32bppArgb;
                                    break;
                                default:
                                    // see the sample for more details
                                    throw new NotImplementedException();
                            }
                            var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                            var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                            bitmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                            bitmap = new Bitmap(bitmap);
                            GC.Collect();
                            handle.Free();
                        }
                    }
                    catch (Exception e)
                    {
                        e.ShowError($"加载dds失败: ${ImgPath}");
                        loadFailed = true;
                    }
                }
                return bitmap;
            }
        }
    }
}

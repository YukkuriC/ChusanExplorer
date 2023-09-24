using Pfim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ChusanExplorer
{
    public class ImageNode
    {
        public ImageNode prev, next;
        public Bitmap bmp;
        public int tabIndex;
        public string path;
        public bool valid = true;

        public void RemoveSelf()
        {
            prev.next = next;
            next.prev = prev;
        }
        public void InsertAfter(ImageNode head)
        {
            next = head.next;
            prev = head;
            head.next = this;
            next.prev = this;
        }
        public void LoadImage()
        {
            if (!valid) return;
            try
            {
                using (var image = Pfimage.FromFile(path))
                {
                    PixelFormat format;
                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            format = PixelFormat.Format32bppArgb;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    var bmpBackend = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                    var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                    bmp = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                    bmp = new Bitmap(bmp);
                    bmpBackend.Free();
                }
            }
            catch (Exception e)
            {
                e.ShowError($"加载dds失败: ${path}");
                valid = false;
            }
        }
    }

    public static class ImageLRU
    {
        const int CACHE_CAPACITY = 300;

        static int releaseCount;
        static Dictionary<string, ImageNode> map;
        static ImageNode link;

        public static void Init()
        {
            map = new Dictionary<string, ImageNode>();
            link = new ImageNode
            {
                valid = false
            };
            link.prev = link.next = link;
        }
        public static Bitmap LoadImage(string path)
        {
            map.TryGetValue(path, out var node);
            if (node == null) return _addImage(path);
            // update lru
            node.RemoveSelf();
            node.InsertAfter(link);
            node.tabIndex = Main.instance.CurrentTab;
            return node.bmp;
        }
        static Bitmap _addImage(string path)
        {
            ImageNode node;
            node = new ImageNode
            {
                path = path,
                tabIndex = Main.instance.CurrentTab,
            };
            node.LoadImage();
            map[path] = node;
            node.InsertAfter(link);

            releaseCount++;
            if (releaseCount >= CACHE_CAPACITY)
            {
                releaseCount = 0;
                TryRelease();
            }

            return node.bmp;
        }
        public static void TryRelease()
        {
            var currentTab = Main.instance.CurrentTab;
            while (map.Count > CACHE_CAPACITY)
            {
                var node = link.prev;
                if (node.tabIndex == currentTab) break;
                map.Remove(node.path);
                node.RemoveSelf();
            }
            GC.Collect();
        }
    }
}

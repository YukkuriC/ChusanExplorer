using System.Drawing;
using System.IO;

namespace ChusanExplorer
{
    public class DDSImage
    {
        public string ImgPath { get; private set; }

        public DDSImage(string path)
        {
            ImgPath = path;
        }

        public bool loadFailed = false;
        public bool Valid => !loadFailed && File.Exists(ImgPath);

        public Bitmap Image
        {
            get
            {
                if (loadFailed) return null;
                var img = ImageLRU.LoadImage(ImgPath);
                if (img == null) loadFailed = true;
                return img;
            }
        }
    }
}

using System.IO;
using System.Linq;

namespace ChusanExplorer
{

    public class CharaImageGroup : IDObject
    {
        public string name;
        public DDSImage[] dds;

        public bool Valid
        {
            get => dds.All(i => i.Valid);
        }

        public CharaImageGroup(string dirImage, int code)
        {
            id = code;
            dds = new DDSImage[3];
            for (var i = 0; i < 3; i++)
            {
                dds[i] = new DDSImage(Path.Combine(
                    dirImage,
                    $"ddsImage{code.PadID()}", $"CHU_UI_Character_{(code / 10).PadID(4)}_{(code % 10).PadID(2)}_{i.PadID(2)}.dds"
                ));
            }
        }
    }
}

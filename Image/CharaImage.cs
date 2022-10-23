using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChusanExplorer{

    public class CharaImageGroup : IDObject
    {
        public string name;
        public CharaImage[] dds;

        public bool Valid
        {
            get => dds.All(i => i.Valid);
        }

        public CharaImageGroup(string dirImage, int code)
        {
            id = code;
            dds = new CharaImage[3];
            for (var i = 0; i < 3; i++) dds[i] = new CharaImage(dirImage, code, i);
        }
    }

    public class CharaImage : DDSImageBase
    {
        string _path;

        public CharaImage(string dirImage, int code, int level)
        {
            _path = Path.Combine(dirImage, $"ddsImage{code.PadID()}", $"CHU_UI_Character_{(code / 10).PadID(4)}_{(code % 10).PadID(2)}_{level.PadID(2)}.dds");
        }

        public override string ImgPath => _path;
    }
}

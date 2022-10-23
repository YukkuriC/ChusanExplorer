using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChusanExplorer
{
    public static class UIEvents
    {
        public static Action<Pack> PackChoiceChanged;
        public static Action CharaListApply, MusicListApply;
        public static Action PlayerCharaProfileChanged;
        public static Action RefreshResultPage;
    }
}

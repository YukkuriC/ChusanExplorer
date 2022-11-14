using System;

namespace ChusanExplorer
{
    public static class UIEvents
    {
        public static Action<Pack> PackChoiceChanged;
        public static Action CharaListApply, MusicListApply;
        public static Action PlayerCharaProfileChanged;
        public static Action RefreshResultPage;
        public static Action RefreshPlayerItems, RefreshPlayerItemSet, RefreshPlayerItemPool;
        public static Action<BaseItem> PlayerItemSelect;
    }
}

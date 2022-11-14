using System;
using System.Data.SQLite;

namespace ChusanExplorer
{
    public partial class ItemDescriptor
    {
        public static ItemDescriptor[] choices = new ItemDescriptor[]
        {
            // 1: NamePlate
            new ItemDescriptor
            {
                id = 1,
                name = "名牌",
                getterStorage = () => Storage.NamePlate,
                getterPack = p => p.namePlates,
                getterPlayerSet = () => Selected.player.myNamePlate,
                getterPlayerChoice = () => Selected.player.itemProfile.NamePlate,
            },
            // 3: Trophy
            new ItemDescriptor
            {
                id = 3,
                name = "称号",
                getterStorage = () => Storage.Trophy,
                getterPack = p => p.trophies,
                getterPlayerSet = () => Selected.player.myTrophy,
                getterPlayerChoice = () => Selected.player.itemProfile.Trophy,
            },
            // 8: MapIcon
            new ItemDescriptor
            {
                id = 8,
                name = "图标",
                getterStorage = () => Storage.MapIcon,
                getterPack = p => p.mapIcons,
                getterPlayerSet = () => Selected.player.myMapIcon,
                getterPlayerChoice = () => Selected.player.itemProfile.MapIcon,
            },
            // 9: SystemVoice
            new ItemDescriptor
            {
                id = 9,
                name = "语音",
                getterStorage = () => Storage.SystemVoice,
                getterPack = p => p.systemVoices,
                getterPlayerSet = () => Selected.player.mySystemVoice,
                getterPlayerChoice = () => Selected.player.itemProfile.SystemVoice,
            },
        };
        public static IDStorage<ItemDescriptor> map = Helpers.IDListToDict(choices);
    }
    
    public static partial class Storage
    {
        public static IDStorage<BaseItem> NamePlate = new IDStorage<BaseItem>();
        public static IDStorage<BaseItem> Trophy = new IDStorage<BaseItem>();
        public static IDStorage<BaseItem> MapIcon = new IDStorage<BaseItem>();
        public static IDStorage<BaseItem> SystemVoice = new IDStorage<BaseItem>();
    }
    
    public partial class Pack
    {
        public void LoadItems()
        {
            loadItemInFolder(dirNamePlate, out namePlates, ref Storage.NamePlate);
            loadItemInFolder(dirTrophy, out trophies, ref Storage.Trophy);
            loadItemInFolder(dirMapIcon, out mapIcons, ref Storage.MapIcon);
            loadItemInFolder(dirSystemVoice, out systemVoices, ref Storage.SystemVoice);
        }
    }

    public partial class PlayerItemProfile
    {
        public int NamePlate;
        public int Trophy;
        public int MapIcon;
        public int SystemVoice;
        public static PlayerItemProfile FromDB(SQLiteDataReader r) => new PlayerItemProfile
        {
            NamePlate = Convert.ToInt32(r["nameplate_id"]),
            Trophy = Convert.ToInt32(r["trophy_id"]),
            MapIcon = Convert.ToInt32(r["map_icon_id"]),
            SystemVoice = Convert.ToInt32(r["voice_id"]),
        };
    }
}

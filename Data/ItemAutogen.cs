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
                nameVerbose = "名牌",
                nameField = "NamePlate",
                nameSQL = "nameplate_id",
                getterStorage = () => Storage.NamePlate,
                getterPack = p => p.namePlates,
                getterPlayerSet = () => Selected.player.myNamePlate,
            },
            // 3: Trophy
            new ItemDescriptor
            {
                id = 3,
                nameVerbose = "称号",
                nameField = "Trophy",
                nameSQL = "trophy_id",
                getterStorage = () => Storage.Trophy,
                getterPack = p => p.trophies,
                getterPlayerSet = () => Selected.player.myTrophy,
            },
            // 8: MapIcon
            new ItemDescriptor
            {
                id = 8,
                nameVerbose = "图标",
                nameField = "MapIcon",
                nameSQL = "map_icon_id",
                getterStorage = () => Storage.MapIcon,
                getterPack = p => p.mapIcons,
                getterPlayerSet = () => Selected.player.myMapIcon,
            },
            // 9: SystemVoice
            new ItemDescriptor
            {
                id = 9,
                nameVerbose = "语音",
                nameField = "SystemVoice",
                nameSQL = "voice_id",
                getterStorage = () => Storage.SystemVoice,
                getterPack = p => p.systemVoices,
                getterPlayerSet = () => Selected.player.mySystemVoice,
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
        public int NamePlate { get => fields["NamePlate"]; set => fields["NamePlate"] = value; }
        public int Trophy { get => fields["Trophy"]; set => fields["Trophy"] = value; }
        public int MapIcon { get => fields["MapIcon"]; set => fields["MapIcon"] = value; }
        public int SystemVoice { get => fields["SystemVoice"]; set => fields["SystemVoice"] = value; }
        public static PlayerItemProfile FromDB(SQLiteDataReader r) => new PlayerItemProfile
        {
            NamePlate = Convert.ToInt32(r["nameplate_id"]),
            Trophy = Convert.ToInt32(r["trophy_id"]),
            MapIcon = Convert.ToInt32(r["map_icon_id"]),
            SystemVoice = Convert.ToInt32(r["voice_id"]),
        };
    }
}

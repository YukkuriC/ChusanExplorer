using System;
using System.Linq;

namespace ChusanExplorer
{
    public static class PlayerCharaProfileLoader
    {
        public static void SetPlayerChoice(Character chara = null)
        {
            if (chara == null) chara = Selected.chara;
            var activeProfile = chara.GetProfile();
            DBLoader.Write($"update chusan_user_data set character_id={Selected.chara.id},chara_illust_id={activeProfile.illustId} where id={Selected.player.id}");
            Selected.player.chara = chara.id;
            UIEvents.PlayerCharaProfileChanged.Invoke();
        }

        public static void UpdateCharaProfile(PlayerCharaProfile newData)
        {
            var oldProfile = Selected.chara.GetProfile();
            int plrId = Selected.player.id,
                charaId = Selected.chara.id;
            if (newData == null) // 删除
                DBLoader.Write($"delete from chusan_user_character where user_id={plrId} and character_id={charaId}");
            else if (oldProfile == null) // 新增
                DBLoader.Write($"insert into chusan_user_character (character_id,user_id,is_valid,is_new_mark,param1,param2,{string.Join(",", PlayerCharaProfile.SQLFields)}) values ({charaId},{plrId},true,true,0,0,{string.Join(",", newData.SQLValues)})");
            else // 更新
                DBLoader.Write($"update chusan_user_character set {string.Join(",", PlayerCharaProfile.SQLFields.Zip(newData.SQLValues, (string k, int v) => $"{k}={v}"))} where user_id={plrId} and character_id={charaId}");

            // 更新缓存
            if (newData == null) Selected.player.charaProfiles.Remove(Selected.chara.id);
            else Selected.player.charaProfiles[Selected.chara.id] = newData;
        }
    }

    public class PlayerCharaProfile : ICloneable
    {
        public static string[] SQLFields = new string[] { "level", "ex_max_lv", "play_count", "friendship_exp", "assign_illust" };
        public int[] SQLValues
        {
            get => new int[] { level, maxLevel, playCount, exp, illustId };
        }

        public int level, maxLevel, playCount, exp, illustId;

        public int LevelExpReq
        {
            get
            {
                if (level >= maxLevel) return 0;
                if (level >= 50) return 90 + 20 * ((level - 50) / 5);
                if (level >= 20) return 20 + 10 * ((level - 20) / 5);
                if (level >= 10) return 10 + 5 * ((level - 10) / 5);
                return 2;
            }
        }

        public string Description
        {
            get
            {
                var expTxt = level >= maxLevel ? "" : $"({exp}/{LevelExpReq})";
                return $"Lv.{level}/{maxLevel}{expTxt}\n游玩次数:{playCount}";
            }
        }

        public PlayerCharaProfile Clone() => (PlayerCharaProfile)MemberwiseClone();
        object ICloneable.Clone() => Clone();

        public static PlayerCharaProfile CreateDefault(int initIllustId) => new PlayerCharaProfile
        {
            level = 1,
            maxLevel = 100,
            playCount = 0,
            exp = 0,
            illustId = initIllustId,
        };

        public static bool operator ==(PlayerCharaProfile a, PlayerCharaProfile b)
        {
            var anull = (object)a == null;
            var bnull = (object)b == null;
            if (anull != bnull) return false;
            if (anull && bnull) return true;
            var pa = a.SQLValues;
            var pb = b.SQLValues;
            for (var i = 0; i < pa.Length; i++) if (pa[i] != pb[i]) return false;
            return true;
        }
        public static bool operator !=(PlayerCharaProfile a, PlayerCharaProfile b) => !(a == b);
    }

}

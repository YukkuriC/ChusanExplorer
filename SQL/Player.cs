using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChusanExplorer
{
    public class Player
    {
        public string name;
        public int id, rating, chara, charaIllust, voice, nameplate, trophy;
        public Player(SQLiteDataReader r)
        {
            InitBase(r);
        }
        public void InitBase(SQLiteDataReader r)
        {
            name = (string)r["user_name"];
            id = Convert.ToInt32(r["id"]);
            rating = Convert.ToInt32(r["player_rating"]);
            chara = Convert.ToInt32(r["character_id"]);
            charaIllust = Convert.ToInt32(r["chara_illust_id"]);
            voice = Convert.ToInt32(r["voice_id"]);
            nameplate = Convert.ToInt32(r["nameplate_id"]);
            trophy = Convert.ToInt32(r["trophy_id"]);
        }
        public override string ToString() => $"#{id} {name}";

        bool initFlag = false;
        public void InitAdvance()
        {
            if (initFlag) return;
            loadCharaProfiles();
            loadSongResults();
            initFlag = true;
        }

        #region chara profile
        public Dictionary<int, PlayerCharaProfile> charaProfiles;
        void loadCharaProfiles()
        {
            charaProfiles = new Dictionary<int, PlayerCharaProfile>();
            using (var r = DBLoader.Read($"select * from chusan_user_character where user_id={id}"))
            {
                while (r.Read())
                {
                    var charaId = Convert.ToInt32(r["character_id"]);
                    charaProfiles[charaId] = new PlayerCharaProfile
                    {
                        level = Convert.ToInt32(r["level"]),
                        maxLevel = Convert.ToInt32(r["ex_max_lv"]),
                        illustId = Convert.ToInt32(r["assign_illust"]),
                        playCount = Convert.ToInt32(r["play_count"]),
                        exp = Convert.ToInt32(r["friendship_exp"]),
                    };
                }
            }
        }
        #endregion

        #region song results
        public Dictionary<Tuple<int, int>, PlayerLevelResult> levelProfiles;
        void loadSongResults()
        {
            levelProfiles = new Dictionary<Tuple<int, int>, PlayerLevelResult>();
            using (var r = DBLoader.Read($"select * from chusan_user_music_detail where user_id={id}"))
            {
                while (r.Read())
                {
                    int musicId = Convert.ToInt32(r["music_id"]),
                        levelId = Convert.ToInt32(r["level"]);
                    levelProfiles[Tuple.Create(musicId, levelId)] = new PlayerLevelResult
                    {
                        musicId = musicId,
                        levelId = levelId,
                        score = Convert.ToInt32(r["score_max"]),
                        playCount = Convert.ToInt32(r["play_count"]),
                        maxCombo = Convert.ToInt32(r["max_combo_count"]),
                        FC = Convert.ToBoolean(r["is_full_combo"]),
                        AJ = Convert.ToBoolean(r["is_all_justice"]),
                    };
                }
            }
        }
        #endregion
    }

    public static class PlayerCharaProfileLoader
    {
        public static void SetPlayerChoice()
        {
            var activeProfile = Selected.chara.GetProfile();
            DBLoader.Write($"update chusan_user_data set character_id={Selected.chara.id},chara_illust_id={activeProfile.illustId} where id={Selected.player.id}");
            Selected.player.chara = Selected.chara.id;
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

    public class PlayerLevelResult
    {
        public int musicId, levelId;
        public int score, playCount, maxCombo;
        public bool FC, AJ;

        public MusicLevel GetLevel()
        {
            Storage.Music.TryGetValue(musicId, out var music);
            return music?.levels[levelId];
        }

        public string Rank
        {
            get
            {
                if (score >= 1009000) return "SSS+";
                if (score >= 1007500) return "SSS";
                if (score >= 1005000) return "SS+";
                if (score >= 1000000) return "SS";
                if (score >= 990000) return "S+";
                if (score >= 975000) return "S";
                if (score >= 950000) return "AAA";
                if (score >= 925000) return "AA";
                if (score >= 900000) return "A";
                if (score >= 800000) return "BBB";
                if (score >= 700000) return "BB";
                if (score >= 600000) return "B";
                if (score >= 500000) return "C";
                return "D";
            }
        }

        static int[] rCtrlPtX = new int[] { 975000, 1000000, 1005000, 1007500, 1009000 };
        static float[] rCtrlPtY = new float[] { 0, 1, 1.5f, 2, 2.15f };
        public float CalcRatingExcess()
        {
            if (score < 975000) return (score - 975000) * 0.01f / 250;
            for (int i = 0; i < 4; i++)
            {
                if (rCtrlPtX[i + 1] < score) continue;
                int x1 = rCtrlPtX[i], x2 = rCtrlPtX[i + 1];
                float y1 = rCtrlPtY[i], y2 = rCtrlPtY[i + 1];
                return y1 + (y2 - y1) * (score - x1) / (x2 - x1);
            }
            return 2.15f;
        }
        public float Rating
        {
            get
            {
                if (levelId == 5) return 0;
                var level = GetLevel();
                if (level == null) return 0;
                var deltaRating = CalcRatingExcess();
                return level.Rating + deltaRating;
            }
        }

        public override string ToString()
        {
            var res = $"{score}({Rank})";
            if (AJ) res += "(AJ)";
            else if (FC) res += "(FC)";
            return res;
        }
    }
}

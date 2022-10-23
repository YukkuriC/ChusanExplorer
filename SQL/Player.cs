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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChusanExplorer
{
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
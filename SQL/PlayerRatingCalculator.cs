using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChusanExplorer
{
    public static class PlayerRatingCalculator
    {
        static Func<PlayerLevelResult, float> getRating = (PlayerLevelResult p) => p.Rating;

        public static List<PlayerLevelResult> b40, b30, b31_40, r10, r10Chosen;
        public static float rating, ratingTheory;
        public static float ratingB30, ratingB40, ratingB31_40, ratingR10, ratingR30, ratingR10Chosen;
        public static int r10ChosenCount;

        public static void Calc()
        {
            // lists
            b40 = (from level in MusicLevelLoader.levelAll
                   where level.index != 5
                   let p = level.GetProfile()
                   where p != null
                   select p).ToList();
            b40.Sort(Helpers.levelRatingDec);
            b40 = b40.GetRange(0, Math.Min(40, b40.Count));
            b30 = b40.GetRange(0, Math.Min(30, b40.Count));
            b31_40 = b30.Count == 30 ? b40.GetRange(30, Math.Min(10, b40.Count - 30)) : new List<PlayerLevelResult>();

            var r30 = Selected.player.r30.ToList();
            r30.Sort(Helpers.levelRatingDec);
            r10 = r30.GetRange(0, Math.Min(10, r30.Count));
            r10Chosen = r10.ToList();
            r10Chosen.AddRange(b31_40);
            r10Chosen.Sort(Helpers.levelRatingDec);
            r10Chosen = r10Chosen.GetRange(0, Math.Min(10, r10Chosen.Count));

            // ratings
            ratingB30 = ratingB40 = ratingB31_40 = ratingR10 = ratingR30 = ratingR10Chosen = 0;
            if (b40.Count > 0)
            {
                ratingB40 = b40.Average(getRating) * b40.Count / 40;
                ratingB30 = b30.Average(getRating) * b30.Count / 30;
                if (b31_40.Count > 0) ratingB31_40 = b31_40.Average(getRating) * b31_40.Count / 10;
            }
            if (Selected.player.r30.Count > 0)
            {
                ratingR30 = Selected.player.r30.Average(getRating) * Selected.player.r30.Count / 30;
                if (r10.Count > 0) ratingR10 = r10.Average(getRating) * r10.Count / 10;
            }
            if (r10Chosen.Count > 0) ratingR10Chosen = r10Chosen.Average(getRating);
            r10ChosenCount = (from r in r10Chosen where r.isRecentScore select r).Count();
            rating = (float)(Math.Floor(ratingB30 * 3000) + Math.Floor(ratingR10Chosen * 1000)) / 4000;
            ratingTheory = b40.Count > 0 ? (ratingB30 * 3 + b40[0].Rating) / 4 : rating;
        }

        public static string GetRatingSummary()
        {
            var plr = Selected.player;
            var lines = new StringBuilder();
            var line = $"Rating: {Math.Round(rating, 3)} (历史最高: {plr.highestRating * 0.01f})";
            if (Math.Floor(rating * 100) != plr.rating)
            {
                line += $"(实际: {plr.rating * 0.01f})";
            }
            lines.AppendLine(line);
            lines.AppendLine($"B30: {Math.Round(ratingB30, 3)}");
            lines.AppendLine($"B31-40: {Math.Round(ratingB31_40, 3)}");
            lines.AppendLine($"R10: {Math.Round(ratingR10, 3)} (占比: {r10ChosenCount}/{r10Chosen.Count})");
            lines.AppendLine($"保底Rating(B40): {Math.Round(ratingB40, 3)}");
            if (b40.Count > 0) lines.AppendLine($"不推分理论最高Rating: {Math.Round(ratingTheory, 3)} (单曲最高: {Math.Round(b40[0].Rating, 3)})");
            return lines.ToString();
        }
    }
}

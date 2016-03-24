using System.Linq;
using System.Text.RegularExpressions;

namespace TextCategorization
{
    /// <summary>
    /// Methods to remove HTML from strings.
    /// </summary>
    public static class HtmlRemoval
    {
        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string StripTagsRegex(string source)
        {
            Regex[] tagsRx = new Regex[16];
            tagsRx[0] = new Regex("<.*?>");
            tagsRx[1] = new Regex("@.*?\\s.*?;");
            tagsRx[2] = new Regex("{.*?}");
            tagsRx[3] = new Regex("#.*?\\s");
            tagsRx[4] = new Regex("/\\*.*?\\*/");
            tagsRx[5] = new Regex("\\..*?\\s.*?\\s");
            tagsRx[6] = new Regex("&.*?;");
            tagsRx[8] = new Regex(" \\| ");
            tagsRx[9] = new Regex("\\d+");
            tagsRx[10] = new Regex("\\S+\\/\\S+");
            tagsRx[11] = new Regex("\\W\\s");
            tagsRx[12] = new Regex("\\s\\W");
            tagsRx[13] = new Regex("™\\s");
            tagsRx[14] = new Regex("”");
            tagsRx[15] = new Regex("\\s+");

            source = tagsRx.Where(tag => tag != null).Aggregate(source, (current, tag) => tag.Replace(current, " "));

            if (Regex.IsMatch(source.Substring(0, 50), "[a-zA-Z]"))
            {
                Regex[] other = new Regex[4];
                other[0] = new Regex("[\\x00-\\x19\\x21-\\x2c\\x2e-\\x40]");
                other[1] = new Regex("\\p{IsCyrillic}"); // Cyrilic
                other[2] = new Regex("»");
                other[3] = new Regex("™");

                source = other.Where(ot => ot != null).Aggregate(source, (current, ot) => ot.Replace(current, "_"));

                Regex ap = new Regex("_+?s");
                source = ap.Replace(source, "`s");
                Regex ap1 = new Regex("_+?S");
                source = ap1.Replace(source, "`S");
                Regex ap2 = new Regex("_+?");
                source = ap2.Replace(source, " ");
                source = tagsRx[15].Replace(source, " ");
            }
            else
            {
                tagsRx[7] = new Regex("\\P{IsCyrillic}"); // Not Cyrilic
                source = tagsRx[7].Replace(source, " ");
                source = tagsRx[15].Replace(source, " ");
            }

            return source;
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            foreach (char @let in source)
            {
                switch (@let)
                {
                    case '<':
                        inside = true;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                }
                if (inside) continue;
                array[arrayIndex] = @let;
                arrayIndex++;
            }
            return new string(array, 0, arrayIndex);
        }
    }
}

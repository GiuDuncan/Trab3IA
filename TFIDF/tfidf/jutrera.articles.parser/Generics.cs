using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jutrera.articles.parser
{
    public class Generics
    {
        public static char RemoveAcute(char c)
        {
            char res = c;

            switch (c)
            {
                case 'á':
                case 'à':
                case 'ä':
                    res = 'a';
                    break;
                case 'é':
                case 'è':
                case 'ë':
                    res = 'e';
                    break;
                case 'í':
                case 'ì':
                case 'ï':
                    res = 'i';
                    break;
                case 'ó':
                case 'ò':
                case 'ö':
                    res = 'o';
                    break;
                case 'ú':
                case 'ù':
                case 'ü':
                    res = 'u';
                    break;


                case 'Á':
                case 'À':
                case 'Ä':
                    res = 'A';
                    break;
                case 'É':
                case 'È':
                case 'Ë':
                    res = 'E';
                    break;
                case 'Í':
                case 'Ì':
                case 'Ï':
                    res = 'I';
                    break;
                case 'Ó':
                case 'Ò':
                case 'Ö':
                    res = 'O';
                    break;
                case 'Ú':
                case 'Ù':
                case 'Ü':
                    res = 'U';
                    break;
            }

            return res;
        }

        public static List<string> ExtractTokens(string token)
        {
            List<string> res = new List<string>();

            var pattern = new Regex(@"(([^\W]|-|\?){1,})|([A-Z]{2})", RegexOptions.IgnorePatternWhitespace);
            token = token.Replace("&nbsp;", "") //yes, exists
                .Replace('á', 'a').Replace('à', 'a').Replace('ä', 'a')
                .Replace('é', 'e').Replace('è', 'e').Replace('ë', 'e')
                .Replace('í', 'i').Replace('ì', 'i').Replace('ï', 'i')
                .Replace('ó', 'o').Replace('ò', 'o').Replace('ö', 'o')
                .Replace('ú', 'u').Replace('ù', 'u').Replace('ü', 'u')
                .Replace('Á', 'A').Replace('À', 'A').Replace('Ä', 'A')
                .Replace('É', 'E').Replace('È', 'E').Replace('Ë', 'E')
                .Replace('Í', 'I').Replace('Ì', 'I').Replace('Ï', 'I')
                .Replace('Ó', 'O').Replace('Ò', 'O').Replace('Ö', 'O')
                .Replace('Ú', 'U').Replace('Ù', 'U').Replace('Ü', 'U');

            foreach (Match item in pattern.Matches(token))
            {
                if (!string.IsNullOrWhiteSpace(item.Groups[1].Value))
                    res.Add(item.Groups[1].Value);
            }

            return res;
        }
    }
}

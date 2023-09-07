using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramApp
{
    public static class SplitText
    {
        public static char[] SplitTextMethod(this string text)
        {
            char[] words = text.ToCharArray();
            return words;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media_Player
{
    class Utility
    {
        
        public static string TrimPath(string path)
        {
            path = path.Replace(@"%20", " ");
            int foundS1 = path.LastIndexOf("/");
            path = path.Remove(0, foundS1 + 1);

            return path;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubgraphIsomorphism.Utils
{
    class ConsoleUtils
    {

        public static string PrepareDeletionString(int length)
        {
            string del = "";
            for (int i = 0; i < length; i++)
            {
                del += "\b";
            }
            return del;
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Droid_video.Comparers
{
    class IgnoreCaseComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.ToUpper() == y.ToUpper();
        }

        public int GetHashCode(string obj)
        {
            return obj.ToUpper().GetHashCode();
        }
    }
}

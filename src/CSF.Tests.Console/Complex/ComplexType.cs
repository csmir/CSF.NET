using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Tests.Console.Complex
{
    public class ComplexType
    {
        public int X, Y, Z;

        [PrimaryConstructor]
        public ComplexType(int x, int y, int z, [Complex] ComplexType complex)
        {
            X = x; Y = y; Z = z;
        }
    }
}

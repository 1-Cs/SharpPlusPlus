using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication
{
    interface ByteReader : Reader
    {
        uint read( byte[] buffer, ulong offset, ulong count);
    }
}

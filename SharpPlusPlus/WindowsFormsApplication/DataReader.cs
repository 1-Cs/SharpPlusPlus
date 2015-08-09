using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication
{
    class DataReader : ByteReader
    {
        long position = 0;
        long markPosition = 0;
        Data data;
        public DataReader(Data date)
        {
            data = date;

        }
        public long align(long boundary)
        {
            long adjust = (boundary - (position % boundary));
            if (adjust == boundary)
                return 0;
            position += adjust;
            return adjust;
        }

        public void close()
        {
            throw new NotImplementedException();
        }

        public void mark()
        {
            markPosition = position;
        }

        public bool markSupported()
        {
            return true;
        }

        public uint read()
        {
            return data.byteAt((uint)position++);
        }

        public uint read(byte[] buffer, ulong offset, ulong count)
        {
            ulong red = count;
            if((ulong)position + count >= data.size())
            {
                if(position >= data.size())
                {
                    return 0xFFFFffff;
                }
                red = data.size() - (ulong)position;
            }
            data.copy(buffer, position,offset, red);
            position += (long)red;
            return (uint)red;
        }

        public void reset()
        {
            position = markPosition;
        }

        public void setReadLimit(long readLimit)
        {
            throw new NotImplementedException();
        }

        public long skip(long skip)
        {
            long jump = skip;
            
            if(position + skip >= data.size())
            {
                if(position > data.size())
                {
                    throw new OutOfBoundsException();
                }
                jump = data.size() - position - 1 ;
            }
            markPosition = position;
            position += jump;
            return jump;
        }
    }
}

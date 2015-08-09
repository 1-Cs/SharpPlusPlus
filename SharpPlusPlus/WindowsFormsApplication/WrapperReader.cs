using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication
{
    class ResourceException : Exception
    {

    }
    class WrapperReader : ByteReader
    {

        ByteReader theReader;
        public
                WrapperReader(ByteReader reader)
        {
            theReader = reader;

        }
        public uint read()
        {
            return ((Reader)theReader).read();
        }
        public uint read(byte[] buffer, ulong offset, ulong count)
        {
            return theReader.read(buffer, offset, count);
        }
        public void close() { }

        public bool markSupported() { return theReader.markSupported(); }

        public void setReadLimit(long readLimit) { theReader.setReadLimit(readLimit); }

        public void mark() { theReader.mark(); }

        public void reset() { theReader.reset(); }

        public long skip(long skip) { return theReader.skip(skip); }

        public long align(long boundary)
        {
            return theReader.align(boundary);
        }

        public uint readString(out String ret)
        {
            String empty = "";
            uint readCount = 0;
            char uni = (char)0;
            do
            {
                byte[] ch = new byte[2];
                uint red = read(ch, 0, 2);
                if (red == 0xFFFFffff)
                    throw new ResourceException();
                uni = (char)((ch[1] << 8) + ch[0]);
                if (uni != 0)
                    empty += uni;
                readCount += red;

            } while (uni != 0);
            ret = empty;
            return readCount;
        }
        public uint readData(out Data data,uint size)
        {
            byte [] buffer = new byte[size];
            uint red = theReader.read(buffer, 0, (ulong)size);
            data = new Data(buffer, size);
            return red;
        }
    }
}

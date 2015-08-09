using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication 
{
    class OutOfBoundsException : Exception
    {

    }
    class Data
    {
        uint theSize = 0;
        byte[] buffer = null;
        public Data(Data data)
        {
            theSize = data.theSize;
            buffer = new byte[theSize];
        }
        public Data(byte[] data, uint size )
        {
            theSize = size;
            buffer = data;            
        }

        public byte byteAt(uint position)
        {
            if (position >= theSize)
                throw new OutOfBoundsException();
            return buffer[position];
        }
        public uint copy(byte[] obuf,long position,ulong offset,ulong count)
        {
            System.Array.Copy(buffer, (int)position, obuf,(int)offset, (int)count);
            return (uint)count;
        }
        /*byte[] pointer(uint offset)
        {
            if (offset >= theSize)
                throw new OutOfBoundsException();
            if(offset == 0)
                return buffer;

            return 
        }*/
        public uint size()
        {
            return theSize;
        }
        String getHex(uint value)
        {
            String str = "";
            uint ch = ((value % 256) / 16);
            ch = ch < 10 ? ch + '0' : (ch - 10) + 'A';
            str += (char)ch;
            ch = value % 16;
            ch = ch < 10 ? ch + '0' : (ch - 10) + 'A';
            str += (char)ch;
            return str;
        }
        bool isprint(uint ch)
        {
            if (ch < 0x20 || ch > 255)
                return false;
            return true;
        }
        public ArrayList toHex()
        {
            ArrayList ar = new ArrayList();
            uint size = theSize;
            //printf(L"%p size=%d\n", p, size);
            byte[] byteX = buffer;
            //unsigned char out[20];
            String outX ="";
            String text = "";
            bool bFirst = true;
            for (int i = 0; i < size; ++i)
            {
                int j = i % 16;
                if (!bFirst && (j % 8) == 0)
                {
                    text += " ";
                }
                if (bFirst)
                    bFirst = false;
                else if (j == 0)
                {
                    text += "  ";
                    text += outX;
			        //outX.clear();
                    outX = "";
                    ar.Add(text);                //wprintf(L"%s\n", text.c_str());
                    //text.clear();
                    text = "";
                }
                text += getHex(byteX[i]) + " ";
                if (isprint(byteX[i]))
                {
        			outX += (char)byteX[i];
                }
                else
            		outX += ".";
            }
            uint jX = size % 16;
            if (jX != 0)
            {
                for (; jX < 16; ++jX)
                {
                    if (jX != 0 && ((jX % 8) != 0))
                    {
                        text += " ";
                    }
                    text += "   ";
                }
            }
            text += "   ";
            text += outX;
            ar.Add(text); //            wprintf(L"%s\n", text.c_str());
            return ar;
        }
    }
}

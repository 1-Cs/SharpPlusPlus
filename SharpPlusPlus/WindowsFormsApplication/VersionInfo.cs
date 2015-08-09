using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public struct Header
    {
        public ushort wLength;
        public ushort wValueLength;
        public ushort wType;
        public Header(byte[]buffer)
        {
            wLength = (ushort)((buffer[1] << 8) + buffer[0]);
            wValueLength = (ushort)((buffer[3] << 8) + buffer[2]);
            wType = (ushort)((buffer[5] << 8) + buffer[4]);
        }
    };
    class VersionInfo
    {
        Header header;
        String key;
        Data rest = null;
        Data allData = null;
        Data values = null;
        String value = "";
        System.Collections.ArrayList children = new System.Collections.ArrayList();
        public bool initFromData(ByteReader versionReader)
        {
            WrapperReader reader = new WrapperReader(versionReader);
            
            uint red = 0;
            reader.align(4);
            reader.mark();
            byte[] buffer = new byte[6];
            uint ret = reader.read(buffer,0,6);
            if (ret == 0xFFFFffff || ret < 6)
                return false;
            header = new Header(buffer);
            red += ret;
            red += reader.readString(out key);
            red += (uint)reader.align(4);
            if (header.wValueLength != 0)
            {
                if (header.wType == 0)
                {
                    values = new Data(null, (uint)header.wValueLength);
                    red += reader.readData(out values, (uint)header.wValueLength);                
                }
                else if (header.wType == 1)
                {
                    values = new Data(null, (uint)header.wValueLength << 1);
                    //buffer = new byte[header.wValueLength << 1];
                    //red += reader.read(buffer, 0, (ulong)header.wValueLength << 1);
                    //values = new Data(buffer, (uint)header.wValueLength << 1);
                    red += reader.readData(out values, (uint)header.wValueLength << 1);

                }
                else
                    throw new ResourceException();

            }
            if (header.wLength > red)
            {
                buffer = new byte[header.wLength - red];
                
                reader.read(buffer, 0, header.wLength - red);
                rest = new Data(buffer, header.wLength - red);
            }
            reader.reset();
            buffer = new byte[header.wLength];

            reader.read(buffer, 0, header.wLength);
            allData = new Data(buffer, header.wLength);
            //dump();
            if (rest != null && rest.size() != 0)
            {
                bool bContinue = false;
                DataReader childReader = new DataReader(rest);
                do
                {
                    VersionInfo pNew = new VersionInfo();
                    bContinue = pNew.initFromData(childReader);
                    //pNew->dump();
                    if(bContinue)
                        children.Add(pNew);
                } while (bContinue);

            }
            return true;
        }
        public TreeNode makeNodes()
        {
            TreeNode[] childArray = new TreeNode[children.Count] ;
            int i = 0;
            foreach (VersionInfo info in children)
            {
                TreeNode retX = info.makeNodes();
                if(retX != null)
                    childArray[i++] = retX;
            }
            TreeNode ret = null;
            if (children.Count > 0)
            {
                ret = new TreeNode(key, childArray);
            }
            else if (key != null && key.Length > 0)
            {
                ret = new TreeNode(key);
            }
            if(ret!= null)
                ret.Tag = this;
            return ret;
        }
        public String[] text()
        {
            String tell = "" ;
            if (header.wType == 1 && values != null)
            {
                DataReader reader = new DataReader(values);
                WrapperReader helper = new WrapperReader(reader);
                helper.readString(out tell);
            }
            else if(header.wType == 0)
            {
                ArrayList p = values.toHex();
                String[] ar = new String[p.Count];
                int i = 0;
                foreach(String x in p)
                {
                    ar[i++] = x;
                }
                return ar;
            }
            return new String[] { key + " : " + tell, "empty" };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace WindowsFormsApplication
{
    public class FileVersion
    {
        [DllImport("version.dll")]
        private static extern bool GetFileVersionInfo(string sFileName,
                int handle, int size, byte[] infoBuffer);
        [DllImport("version.dll")]
        private static extern int GetFileVersionInfoSize(string sFileName,
                out int handle);
        int handle = 0;
        public byte[] getFileVersion(String path)
        {
            int size = GetFileVersionInfoSize(path, out handle);
            if (size != 0)
            {
                byte[] buffer = new byte[size];

                if (GetFileVersionInfo(path, handle, size, buffer))
                {
                    return buffer;
                }
            }
            return null;
        }
        class Type
        {

            public String name;
            public IntPtr type;
            public Type(IntPtr type, String name)
            {
                this.type = type;
                this.name = name;
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ResEntry
        {
            public UInt64 type;
            public UInt64 name;
            public UInt16 lang;
            //string strType;
            //string strName;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BufOut
        {
            public UInt32 size;
            public byte[] bytes;
        };

        class Container
        {
            TreeNode root;
            public Container(TreeNode node)
            {
                root = node;

            }
            TreeNode addType(ResEntry entry,TreeNode node)
            {
                IntPtr i = (IntPtr)entry.type;
                String type = ResourceTypes.getResTypeName(i);
                TreeNode[] treeList = node.Nodes.Find(type, false);

                if (treeList == null || treeList.Length == 0)
                {
                    TreeNode added = node.Nodes.Add(type);
                    added.Name = type;
                    treeList = new TreeNode[] { added };
                }
                return treeList[0];

            }
            TreeNode addName(ResEntry entry,TreeNode node)
            {
                IntPtr i = (IntPtr)entry.name;
                String name = i.ToString();
                TreeNode[] treeList = node.Nodes.Find(name, false);
                if (treeList == null || treeList.Length == 0)
                {
                    TreeNode added = node.Nodes.Add(name);
                    added.Name = name;
                    treeList = new TreeNode[] { added };
                }
                return treeList[0];
            }
            TreeNode addLang(ResEntry entry,TreeNode node)
            {
                String name = entry.lang.ToString();
                TreeNode[] treeList = node.Nodes.Find(name, false);
                if (treeList == null || treeList.Length == 0)
                {
                    TreeNode added = node.Nodes.Add(name);
                    added.Name = name;
                    treeList = new TreeNode[] { added };
                }
                return treeList[0];

            }
            ArrayList list = new ArrayList();
            public void add(ResEntry entry)
            {

                list.Add(entry);

                TreeNode node = addType(entry,root);
                TreeNode nameNode = addName(entry, node);
                TreeNode lang = addLang(entry, nameNode);
                lang.Tag = entry;
            }

        }
        private delegate bool EnumAllProc(ResEntry entry,ref Container container);
        private delegate long EnumResTypeProc(IntPtr hModule, IntPtr lpszType, IntPtr lParam);
        private delegate bool EnumResNameProc(IntPtr hModule, IntPtr lpszType, IntPtr lpszName,IntPtr lParam);

        bool MyEnumAllProc(ResEntry entry,ref Container container)
        {
            container.add(entry);
            return true;
        }

        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32.dll")]
        private extern static void FreeLibrary(IntPtr handle);
        static ArrayList types = new ArrayList();
        static ArrayList typeNames = new ArrayList();
        static ArrayList names = new ArrayList();
        String path = null;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool UpdateResource(IntPtr hUpdate, string lpType, string lpName, ushort wLanguage, IntPtr lpData, uint cbData);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern long EnumResourceTypes(IntPtr hModule, EnumResTypeProc lpEnumFunc, IntPtr lParam);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern long EnumResourceNames(IntPtr hModule, IntPtr lpszType,EnumResNameProc lpEnumFunc, IntPtr lParam);
        [DllImport("kernel32.dll")]
        static extern UInt32 GetLastError();

        static bool MyEnumResNameProc(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam)
        {
            if((int)lpszName > 0x00010000)
            {
                
            }
            names.Add(lpszName.ToString());
            return true;
        }
        static long MyEnumResTypeProc(IntPtr hModule, IntPtr lpszType, IntPtr lParam)
        {
            String res = lpszType.ToString();
            String name = ResourceTypes.getResTypeName(lpszType);
            types.Add(new Type(lpszType,name));
            return 0xFFFFffff;
        }

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void testSet(ref ResEntry entry);

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern unsafe ResEntry * testNew();

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool enumAll(UInt64 handle, EnumAllProc function, ref Container container);

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern UInt64 initSeasons();

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void freeSeasons(UInt64 hSeason);

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CharSet=CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern int openResModule(UInt64 hSeason, String path);

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void closeResModule(UInt64 hSeason);

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void getResourceSize(UInt64 hSeason,ref ResEntry entry,ref UInt32 size);

        [DllImport("d:\\Source\\1-cpp\\Seasons\\Debug\\Seasoning.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void getResource(UInt64 hSeason, ref ResEntry entry, UInt32 size ,byte[] buffer);

        Container container = null;
        
        public bool isResource(object obj)
        {
            if (obj is ResEntry)
                return true;
            return false;
        }
        public String[] loadResource(object obj,ref Bitmap bitmap)
        {
            if (obj is ResEntry)
            {
                ResEntry entry = (ResEntry)obj;
                UInt64 handle = initSeasons();
                UInt32 size = 0;
                int bRet = openResModule(handle, path);
                getResourceSize(handle, ref entry, ref size);
                byte[] buffer = new byte[size];
                for(int i = 0; i < size; ++i)
                {
                    buffer[i] = (byte)(i % 256);
                }
                getResource(handle, ref entry, size ,buffer);
                closeResModule(handle);
                freeSeasons(handle);
                Data data = new Data(buffer, size);
                ArrayList list = data.toHex();
                String[] ar = new String[ list.Count ];
                int j = 0;
                foreach(String s in list)
                {
                    ar[j++] = s;
                }

                byte[] obuf = new byte[buffer.Length + 6 + 16];
                int k = 0;
                /*
                typedef struct {
         DWORD DataSize;
        DWORD HeaderSize;
        DWORD TYPE; Unicode If the first WORD in the member is equal to the value 0xffff,it is ordinal otherwise unicode
       DWORD NAME; Unicode
        Padding;
        DWORD DataVersion;
        WORD MemoryFlags;

                        MOVEABLE(0x0010)
                        FIXED(~MOVEABLE)
                        PURE(0x0020)
                        IMPURE(~PURE)
                        PRELOAD(0x0040)
                        LOADONCALL(~PRELOAD)
                        DISCARDABLE(0x1000)

        WORD LanguageId;
        DWORD Version;
        DWORD Characteristics;
    }
    https://msdn.microsoft.com/en-us/library/windows/desktop/ms648027(v=vs.85).aspx
    RESOURCEHEADER;*/
                // directory
                obuf[k++] = 0;
                obuf[k++] = 0;
                obuf[k++] = 1; // Icon = 1 ; Cursor = 2
                obuf[k++] = 0; 
                obuf[k++] = 1; // 1 icon in file
                obuf[k++] = 0;
                // end directory


                Copy(buffer, 0, obuf, 6, buffer.Length );
                File.WriteAllBytes("my.ico", obuf);
                return ar;
                
            }
            return new String[0];
        }
        public void Copy(byte[]buffer,int offset,byte[]outBuffer,int outOffset,int count)
        {
            for(int i = 0;i < count;++i)
            {
                outBuffer[outOffset + i] = buffer[offset + i];
            }
        }
        public TreeNode init(String path)
        {
            ResEntry entry = new ResEntry();
            unsafe
            {
                ResEntry* r;
                r = testNew();
                testSet(ref *r);
                testSet(ref entry);
            }
            TreeNode root = new TreeNode("Current Resources");
            container = new Container(root);
            UInt64 hSeason = initSeasons();
            char[] str = new char[path.Length];
            str = path.ToCharArray();
            int bRet = openResModule(hSeason,path);
            if(bRet == 0)
            {
                MessageBox.Show("Cannot open <" + str + "> for reading");
            }
            enumAll(hSeason, MyEnumAllProc,ref container);
            closeResModule(hSeason);
            freeSeasons(hSeason);
            //byte[] p = (byte[])testNew();

            this.path = path;
            types.Clear();
            IntPtr hModule = LoadLibrary(path);
            if(hModule == null)
            {
                DialogResult box = MessageBox.Show("Module " + path + " cannot be loaded","FileVersion");
                return null;

            }
            UInt32 err = 0;
            long ret = EnumResourceTypes(hModule, new EnumResTypeProc(MyEnumResTypeProc),(IntPtr)0);
            err = GetLastError();
             
            FreeLibrary(hModule);
            if(ret == 0)
            {
                String x ;
                x = String.Format("{0:x8}", err);
                DialogResult box = MessageBox.Show("Module <" + path + "> resources cannot be enumerated " + x, "FileVersion");
            }
            return root;
        }
        public String[] text()
        {
            if(types != null && types.Count > 0)
            {
                String[] ar = new String[types.Count];
                int i = 0;
                foreach(Type s in types)
                {
                    ar[i++] = s.name;
                }
                return ar;
            }
            return null;
        }
        public TreeNode makeNodes()
        {
            int i = 0;
            if(types.Count == 0)
            {
                return null;
            }
            TreeNode[] ar = new TreeNode[types.Count];
            foreach(Type type in types)
            {
                IntPtr hModule = LoadLibrary(path);
                names.Clear();
                EnumResourceNames(hModule, type.type, MyEnumResNameProc, (IntPtr)0);
                
                FreeLibrary(hModule);
                TreeNode[] array = new TreeNode[names.Count];
                int j = 0;
                foreach(String name in names)
                {
                    array[j++] = new TreeNode(name);
                }
                ar[i++] = new TreeNode(type.name, array);
            }
            return new TreeNode("Resources", ar);
        }
    }
}

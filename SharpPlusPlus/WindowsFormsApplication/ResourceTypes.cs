using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication
{
    class ResourceTypes
    {
        const int RT_ACCELERATOR = 9; //Accelerator table.
        const int RT_ANICURSOR = 21; //Animated cursor.
        const int RT_ANIICON = 22; //Animated icon.
        const int RT_BITMAP = 2; //Bitmap resource.
        const int RT_CURSOR = 1; //Hardware-dependent cursor resource.
        const int RT_DIALOG = 5; //Dialog box.
        const int RT_DLGINCLUDE = 17; //Allows
        const int RT_FONT = 8; //Font resource.
        const int RT_FONTDIR = 7; //Font directory resource.
        const int RT_GROUP_CURSOR = ((RT_CURSOR) + 11); //Hardware-independent cursor resource.
        const int RT_GROUP_ICON = ((RT_ICON) + 11); //Hardware-independent icon resource.
        const int RT_HTML = 23; //HTML resource.
        const int RT_ICON = 3; //Hardware-dependent icon resource.
        const int RT_MANIFEST = 24; //Side-by-Side Assembly Manifest.
        const int RT_MENU = 4; //Menu resource.
        const int RT_MESSAGETABLE = 11; //Message-table entry.
        const int RT_PLUGPLAY = 19; //Plug and Play resource.
        const int RT_RCDATA = 10; //Application-defined resource (raw data).
        const int RT_STRING = 6; //String-table entry.
        const int RT_VERSION = 16; //Version resource.
        const int RT_VXD = 20; //
        const int RT_DLGINIT = 240;
        const int RT_TOOLBAR = 241;
        public static string getResTypeName(IntPtr Type)
        {
            string ret;
            switch ((int)Type)
            {
                case RT_ACCELERATOR:
                    ret = "RT_ACCELERATOR";
                    break;
                case RT_ANICURSOR:
                    ret = "RT_ANICURSOR";
                    break;
                case RT_ANIICON:
                    ret = "RT_ANIICON";
                    break;
                case RT_BITMAP:
                    ret = "RT_BITMAP";
                    break;
                case RT_CURSOR:
                    ret = "RT_CURSOR";
                    break;
                case RT_DIALOG:
                    ret = "RT_DIALOG";
                    break;
                case RT_DLGINCLUDE:
                    ret = "RT_DLGINCLUDE";
                    break;
                case RT_FONT:
                    ret = "RT_FONT";
                    break;
                case RT_FONTDIR:
                    ret = "RT_FONTDIR";
                    break;
                case RT_GROUP_CURSOR:
                    ret = "RT_GROUP_CURSOR";
                    break;
                case RT_GROUP_ICON:
                    ret = "RT_GROUP_ICON";
                    break;
                case RT_HTML:
                    ret = "RT_HTML";
                    break;
                case RT_ICON:
                    ret = "RT_ICON";
                    break;
                case RT_MANIFEST:
                    ret = "RT_MANIFEST";
                    break;
                case RT_MENU:
                    ret = "RT_MENU";
                    break;
                case RT_MESSAGETABLE:
                    ret = "RT_MESSAGETABLE";
                    break;
                case RT_PLUGPLAY:
                    ret = "RT_PLUGPLAY";
                    break;
                case RT_RCDATA:
                    ret = "RT_RCDATA";
                    break;
                case RT_STRING:
                    ret = "RT_STRING";
                    break;
                case RT_VERSION:
                    ret = "RT_VERSION";
                    break;
                case RT_VXD:
                    ret = "RT_VXD";
                    break;
                case RT_DLGINIT:
                    ret = "RT_DLGINIT";
                    break;
                case RT_TOOLBAR:
                    ret = "RT_TOOLBAR";
                    break;
                default:
                    ret = Type.ToString();
                    break;
            }
            return ret;
        }
    }
}

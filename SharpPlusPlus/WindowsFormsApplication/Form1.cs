using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using System.Collections;
using System.Runtime.InteropServices;
using Test;

namespace WindowsFormsApplication
{
    enum Type
    {
        None = 0,
        Operator = 1,
        Begin = 2,
        End = 3,
        Range = 4,
        Exclusion = 5,
        EscapeSeq = 6,
        Production = 7,
        Assignment = 8,
        EOL = 9,
        WhiteSpace = 10,
        Identifier = 11,
        SyntaxError = 12,
        String = 13,
        Line = 14,
        EOS = 19,
    };
    public class Helper
    {
        public Assignment assignment;
        public String text;
    };
    public partial class BaseForm : Form
    {

        Lexer lex = new Lexer();
        TestClass tester = new TestClass();
        String style = "<style>\nbold { font-family: \"Courier\"; sans-serif; font-size: 12pt; margin-bottom: 0.1cm ; margin-top: 0cm } \n ";
        String styleEnd =    "\n</style>";
        String para = 
                "p { font-family: \"Arial\", sans-serif; font-size: 12pt; margin-bottom: 0.1cm ; margin-top: 0cm ; line-height: 80% ;} \n ";
        String green = "bold.green { color: green; } \n ";
        String blue = "bold.blue { color: blue; } \n ";
        String red = "bold.red { color: red; } \n ";
        String cyan = "bold.cyan { color: cyan; } \n ";
        String magenda = "bold.magenda { color: magenda; } \n ";
        String yellow = "bold.yellow { color: yellow; } \n ";
        String black = "bold.black { color: black; } \n ";

        FileVersion version = new FileVersion();
        TreeNode added = null;
        TreeNode vers = null;
        String path = "D:\\Source\\cs\\WindowsFormsApplication\\WindowsFormsApplication\\LexFile.xml";  
            //"d:\\Program Files (x86)\\SlySoft\\AnyDVD\\AnyDVD.exe";
        public BaseForm()
        {
            InitializeComponent();
            this.openFileDialog1.FileName = path;
            filename.Text = path;
            style += green + blue + red + magenda + yellow + cyan + black + para + styleEnd;
        }
        String getType(int type)
        {
            switch(type)
            {
                case 4:
                    return "Range";
                case 5:
                    return "Exclusion";
                case 6:
                    return "EscapeSequence";
                case 8:
                    return "Assignment";
                default:
                    return "No Assignment";
            }
        }
        String getColor(int type)
        {
            switch(type)
            {
            case 11:
                return "blue";
            case 12:
                return "red";
            case 13:
                return "magenda";
            case 17:
            case 18:
                return "cyan";
            default:
                if (type >= 1 && type <= 8)
                    return "green";
                else
                    return "black";
            }
        }
        String work(String text,ref ArrayList list,ref ArrayList listAssign)
        {
            tester.foo();
            String ret = "";
            int pos = 0;
            int oldpos = 0;
            ArrayList array = new ArrayList();
            lex.read(text);
            lex.fill(array);
            lex.getProductions(list);
            lex.getAssignments(listAssign);
            Token old = new Token(0,0,0,0,0);
            foreach(Token t in array)
            {
                if (t.type != (int)Type.Line)
                {
                    
                    
                    pos = t.pos;
                    if (oldpos > t.pos)
                    {
                        ret += "<h3>MIST</h3>";
                        ret += "<br>" + getColor(old.type) + "(" + old.type.ToString() + ":" + 
                                old.pos.ToString() + ":" + old.count.ToString() + ")";
                        ret += "<br>" + getColor(t.type) + "(" + t.type.ToString() + ":" + t.pos.ToString() + ":" + t.count.ToString() + ")";
                        ret += "<br>";
                        oldpos = t.pos + t.count;
                        continue;
                    }
                    String strText = text.Substring(oldpos, t.pos - oldpos);
                    ret += strText;
                    if (t.type == (int)Type.EOL)
                    {
                        ret += "<br>";
                    }
                    else if(t.type == (int)Type.EOS)
                    {
                        ret += "<br><BIG>ENDE</BIG>";
                    }
                    else
                    {
                        //ret += "<br>" + getColor(t.type) + "(" + t.type.ToString() + ":" + t.pos.ToString() + ":" + t.count.ToString() + ")";
                        ret += "<bold class=\"" + getColor(t.type) + "\">" + text.Substring(t.pos, t.count) + "</bold>";
                    }
                    oldpos = t.pos + t.count;
                    old = t;
                }
                else
                    ret += "<br>";
            }

            /*while(oldpos < text.Length && (pos = text.IndexOf('\n',oldpos)) != -1)
            {
                ret += text.Substring(oldpos, pos - oldpos);
                ret += "<p class=\"xx\">";
                pos++;
                oldpos = pos;
            }
            if(oldpos < text.Length)
                ret += text.Substring(oldpos);*/
            return ret;
        }
        String escape(String text)
        {
            String ret = "";
            
            for(int i = 0; i < text.Length; ++i)
            {
                switch(text[ i ])
                {
                    case '<':
                        ret += "&lt;";
                        break;
                    case '>':
                        ret += "&gt;";
                        break;
                    case '\n':
                        ret += "<br>";
                        break;
                    default:
                        ret += text[i];
                        break;

                }
            }
            return ret;
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            object tag = e.Node.Tag;
            if (tag is Dictionary<String, String>)
            {
                keyValue.Visible = true;
                picture.Visible = false;
                keyValue.Items.Clear();
                String[] s = new String[2];
                Dictionary<String, String> map = (Dictionary < String, String> )tag;
                foreach(String key in map.Keys)
                {
                    s[0] = key; // + "=" + map[key];
                    s[1] = map[key];

                    ListViewItem item = new ListViewItem(s[0]);
                    item.SubItems.Add(s[1]);
                    keyValue.Items.Add(item);
                }
            }
            else
            {
                keyValue.Visible = false;
            }
            if(tag is Helper)
            {
                webBrowser.Visible = true;
                Assignment ass = ((Helper)tag).assignment;
                String text = ((Helper)tag).text;
                Token token = new Token();
                ass.getType(token);
                String html = "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />" +
                    style + "</head><body>";
                html += "<h1>" + getType(token.type) + "</h1>";
                ass.getString(token);

                html += "<bold class=\"blue\">\"" + text.Substring(token.pos,token.count) + "\"</bold><br>";
                html += "</body></html>";
                webBrowser.DocumentText = html;
            }
            else if(tag is String)
            {
                webBrowser.Visible = true;
                String text = (String)tag;
                ArrayList listProd = new ArrayList();
                ArrayList listAssign = new ArrayList();
                String textOut = work(text,ref listProd,ref listAssign);
                TreeNode nodeProd = e.Node.Nodes.Add("Productions");
                TreeNode nodeAssign = e.Node.Nodes.Add("Assign");
                foreach (Object obj in listProd)
                {
                    if(obj is Token)
                    {
                        Token t = (Token)obj;
                        nodeProd.Nodes.Add(text.Substring(t.pos, t.count) );
                    }
                }
                foreach (Object obj in listAssign)
                {
                    if (obj is Test.Assignment)
                    {
                        Test.Assignment t = (Test.Assignment)obj;
                        Token token = new Token();
                        t.getIdentifier(token);
                        Helper helper = new Helper();
                        helper.assignment = t;
                        helper.text = text;
                        nodeAssign.Nodes.Add("<" + text.Substring(token.pos, token.count) + ">").Tag = helper;
                      
                    }
                }
                String html = "<html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />" + 
                    style + "</head><body><h1>Value</h1>" + textOut;
                
                
                webBrowser.DocumentText = html + "</body></html>";
            }
            else
            {
                webBrowser.Visible = false;
            }
            return;
            if (e.Node.Tag is VersionInfo)
            {
                VersionInfo p = (VersionInfo)e.Node.Tag;
                if (p != null)
                    hexWindow.Lines = p.text();
            }
            else if (version.isResource(e.Node.Tag))
            {
                Bitmap bitmap = new Bitmap(300,400);
                hexWindow.Lines = version.loadResource(e.Node.Tag,ref bitmap);

                picture.Image = bitmap; ;
                picture.Refresh();
                
            }

            if (e.Node.Text == "ResourceTypes")
            {
                if(vers != null)
                    baseTree.Nodes.Remove(vers);
                vers = version.init(path);
                if (vers != null)
                    baseTree.Nodes.Add(vers);
                String[] text = version.text();
                keyValue.Items.Clear();
                if(text != null)
                foreach (String s in text)
                {
                    keyValue.Items.Add(s);
                }
                
                TreeNode node = version.makeNodes();
                
                if(added != null)
                    baseTree.Nodes.Remove(added);
                if (node != null)
                {
                    added = node;

                    baseTree.Nodes.Add(node);
                }
            }
            else
            {
                object obj = e.Node.Tag;

            }

        }

        private void filename_TextChanged(object sender, EventArgs e)
        {
            path = filename.Text;
            if(!File.Exists(path))
            {
                return;
            }
            baseTree.Nodes.Clear();
            
            XmlReader xml = XmlReader.Create(path);
            Stack<String> backStack = new Stack<String>();
            Stack<TreeNode> nodeStack = new Stack<TreeNode>();
            TreeNode node = new TreeNode("ROOT");
            TreeNode oldnode = null;
            baseTree.Nodes.Add(node);

            while (xml.Read())
            {
                switch (xml.NodeType)
                {
                    case XmlNodeType.Element:
                        oldnode = node;
                        if (oldnode != null)
                            nodeStack.Push(oldnode);
                        node = new TreeNode(xml.Name);
                       
                        node.Tag = new Dictionary<String, String>();
                        oldnode.Nodes.Add(node);                        
                        backStack.Push(xml.Name);
                        if (xml.HasAttributes)
                        {
                            xml.MoveToFirstAttribute();
                            do
                            {
                                ((Dictionary<String, String>)node.Tag).Add(xml.Name, xml.Value);
                            }
                            while (xml.MoveToNextAttribute());
                        }
                        break;
                    case XmlNodeType.Text:
                        node.Nodes.Add("Text").Tag = xml.Value;
                        break;
                    case XmlNodeType.Attribute:
                        ((Dictionary<String, String>)node.Tag).Add(xml.Name, xml.Value);
                        break;
                    case XmlNodeType.CDATA:
                        node.Nodes.Add("CDATA").Tag = xml.Value;
                        break;
                    case XmlNodeType.Comment:
                        node.Nodes.Add("Comment").Tag = xml.Value;
                        break;
                    case XmlNodeType.EndElement:
                        String nodeX = backStack.Pop();
                        if (nodeX != xml.Name)
                            throw new SyntaxErrorException();
                        node = nodeStack.Pop();
                        break;
                    case XmlNodeType.ProcessingInstruction:
                        break;  
                    default:
                        break;

                }
            }
            xml.Close();
            baseTree.ExpandAll();
            return;
            byte[] buffer = version.getFileVersion(path);

            Data data = new Data(buffer, (uint)buffer.Length);
            DataReader reader = new DataReader(data);

            VersionInfo info = new VersionInfo();
            info.initFromData(reader);
            TreeNode vTree = info.makeNodes();
            if (vTree != null)
                baseTree.Nodes.Add(vTree);
            baseTree.Nodes.Add("ResourceTypes");
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            DialogResult res = this.openFileDialog1.ShowDialog();
            if(res == DialogResult.OK)
            {
                filename.Text = openFileDialog1.FileName;
            }
        }
    }

}


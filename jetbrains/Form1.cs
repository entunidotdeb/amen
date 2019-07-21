using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace jetbrains
{
    public partial class Form1 : Form
    {
        private HttpListener listener = new HttpListener();
        private Rectangle rect = new Rectangle();
        private RectangleF rectf = new RectangleF();
        private PageSettings de = new PageSettings();
        private JObject json = new JObject();
        private HttpListenerContext context;
        private HttpListenerRequest request;
        private HttpListenerResponse response;
        private StreamReader reader;
        private Stream body;
        private string s;
        private int X;
        private int Y;
        private int W;
        private int H;
        public Form1()
        {
            listener.Prefixes.Add("http:localhost:8080/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            listener.Start();
            while(true)
            {
                Console.WriteLine("Listening...");
                context = listener.GetContext();
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

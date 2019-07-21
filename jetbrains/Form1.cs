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
            listener.Prefixes.Add("http://localhost:8080/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            listener.Start();
            while(true)
            {
                Console.WriteLine("Listening...");
                context = listener.GetContext();
                HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)context.User.Identity;
                if (identity.Name == "User1" && identity.Password == "pwd")
                {
                    Console.WriteLine("Auth Reached");
                    request = context.Request;
                    response = context.Response;
                    if (!request.HasEntityBody)
                    {
                        Console.WriteLine("Nothing to Print");
                    }
                    else
                    {
                        Console.WriteLine("Http Method Used: " + request.HttpMethod);
                        body = request.InputStream;
                        Encoding contentEncoding = request.ContentEncoding;
                        reader = new StreamReader(body, contentEncoding);
                        s = reader.ReadToEnd();
                        PrintDocument printDocument = new PrintDocument();
                        printDocument.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                        printDocument.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                        de = printDocument.PrinterSettings.DefaultPageSettings;
                        rect = de.Bounds;
                        rect.X = 0;
                        rect.Y = 0;
                        rect.Width = 284;
                        rect.Height = 1169;
                        X = rect.X;
                        Y = rect.Y;
                        W = rect.Width;
                        H = rect.Height;
                        printDocument.Print();
                        body.Close();
                        reader.Close();
                        byte[] bytes = Encoding.UTF8.GetBytes("<HTML> <BODY>RECEIPT PRINTED </BODY></HTML>");
                        response.ContentLength64 = (long)bytes.Length;
                        Stream outputStream = response.OutputStream;
                        outputStream.Write(bytes, 0, bytes.Length);
                        Console.WriteLine("message sent...");
                        outputStream.Close();
                    }

                }
                else
                {
                    context.Response.StatusCode = 401;
                }
                if (context.Response.StatusCode == 401)
                {
                        context.Response.AddHeader("WWW-Authenticate", "Basic Realm=\"My WebDAV Server\"");
                        byte[] bytes = new UTF8Encoding().GetBytes("Access denied");
                        context.Response.ContentLength64 = (long)bytes.Length;
                        context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }

            }

        }
        private RectangleF Crect(int x, int y, int width, int height)
        {
            return new RectangleF(x, y, width,height);
        }
        public void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            Dictionary<string, List<string>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(s);
            string str1 = dictionary["POS"][0];
            string str2 = dictionary["TableNo"][0];
            string str3 = dictionary["OrderNo"][0];
            string str4 = dictionary["Date"][0];
            string str5 = dictionary["DDate"][0];
            string str6 = dictionary["Time"][0];
            Console.WriteLine(dictionary["itemnames"][0]);
            Font font1 = new Font("Arial", 8f, FontStyle.Bold, GraphicsUnit.Point);
            Font font2 = new Font("Arial", 6f, FontStyle.Bold, GraphicsUnit.Point);
            Font font3 = new Font("Arial", 10f, FontStyle.Bold, GraphicsUnit.Point);
            Font font4 = new Font("Arial", 8f, GraphicsUnit.Point);
            Font font5 = new Font("Arial", 6f, GraphicsUnit.Point);
            SolidBrush solidBrush = new SolidBrush(Color.Black);
            Pen pen = new Pen(Color.Black);
            ev.Graphics.DrawRectangle(pen, X, Y, 228, 1169);
            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;
            StringFormat format2 = new StringFormat();
            format2.Alignment = StringAlignment.Near;
            StringFormat format3 = new StringFormat();
            format3.Alignment = StringAlignment.Far;
            rectf = Crect(X, Y, W, 20);
            ev.Graphics.DrawString("TOURIST VILLAGE SHIVPURI:", font1, Brushes.Blue, rectf, format1);
            rectf = Crect(X, Y += 15, W, 20);
            ev.Graphics.DrawString("Near Bhadalaya Kund, Chatri Road,", font2, Brushes.Black, rectf, format1);
            rectf = Crect(X, Y += 10, W, 20);
            ev.Graphics.DrawString("Shivpuri, India, 473551", font2, Brushes.Black, rectf, format1);
            rectf = Crect(X, Y += 10, W, 20);
            ev.Graphics.DrawString("+919999999999", font2, Brushes.Black, rectf, format1);
            int x1 = X;
            Y += 5;
            for (; x1 < W; x1 += 5)
                ev.Graphics.DrawString("-", font5, Brushes.Black, x1, Y);
            int width = W - 20;
            rectf = Crect(X, Y += 10, width, 20);
            ev.Graphics.DrawString("Place of supply: " + str1.ToString(), font2, Brushes.Black, rectf, format2);
            ev.Graphics.DrawString("Table No: " + str2.ToString(), font2, Brushes.Black, rectf, format3);
            rectf = Crect(X, Y += 10, width, 20);
            ev.Graphics.DrawString("Order No: " + str3.ToString(), font2, Brushes.Black, rectf, format2);
            ev.Graphics.DrawString("Date:  " + str4.ToString(), font2, Brushes.Black, rectf, format3);
            rectf = Crect(X, Y += 10, width, 20);
            ev.Graphics.DrawString("Delivery Date: " + str5.ToString(), font2, Brushes.Black, rectf, format2);
            ev.Graphics.DrawString("Time: " + str6.ToString(), font2, Brushes.Black, rectf, format3);
            int x2 = X;
            Y += 10;
            for (; x2 < W; x2 += 5)
                ev.Graphics.DrawString("-", font5, Brushes.Black, x2, Y);
            rectf = Crect(X, Y += 10, width / 2 + 30, 20);
            ev.Graphics.DrawString("Item Name", font2, Brushes.Black, rectf, format1);
            ev.Graphics.DrawString("QTY", font2, Brushes.Black, rectf, format3);
            rectf = Crect(X + width / 2 + 45, Y, width / 2 - 45, 20);
            ev.Graphics.DrawString("Price", font2, Brushes.Black, rectf, format2);
            ev.Graphics.DrawString("Amt", font2, Brushes.Black, rectf, format3);
            int x3 = X;
            Y += 10;
            for (; x3 < W; x3 += 5)
                ev.Graphics.DrawString("-", font5, Brushes.Black, (float)x3, (float)Y);
            List<string> stringList = new List<string>();
            for (int index = 0; index < dictionary["itemnames"].Count - 1; index += 4)
                stringList.Add(dictionary["itemnames"][index]);
            Y += 10;
            for (int index = 0; index < dictionary["itemnames"].Count; index += 4)
            {
                rectf = Crect(X, Y, width / 2 + 30, 20);
                ev.Graphics.DrawString(dictionary["itemnames"][index], font2, Brushes.Black, rectf, format1);
                ev.Graphics.DrawString(dictionary["itemnames"][index + 1], font2, Brushes.Black, rectf, format3);
                rectf = Crect(X + width / 2 + 45, Y, width / 2 - 45, 20);
                ev.Graphics.DrawString(dictionary["itemnames"][index + 2], font2, Brushes.Black, rectf, format2);
                ev.Graphics.DrawString(dictionary["itemnames"][index + 3], font2, Brushes.Black, rectf, format3);
                Y += 20;
            }
            int x4 = X;
            Y -= 10;
            for (; x4 < W; x4 += 5)
                ev.Graphics.DrawString("-", font5, Brushes.Black, (float)x4, (float)Y);
            Y += 10;
            rectf = Crect(X, Y, width / 2, 20);
            ev.Graphics.DrawString("Total: ", font2, Brushes.Black, rectf, format1);
            rectf = Crect(X + width / 2 + 45, Y, width / 2 - 45, 20);
            ev.Graphics.DrawString(dictionary["Total"][0], font2, Brushes.Black, rectf, format3);
            rectf = Crect(X, Y += 20, width / 2, 20);
            ev.Graphics.DrawString("CGST @2.50% ", font2, Brushes.Black, rectf, format1);
            rectf = Crect(X + width / 2 + 45, Y, width / 2 - 45, 20);
            ev.Graphics.DrawString(dictionary["CGST @2.50%"][0], font2, Brushes.Black, rectf, format3);
            rectf = Crect(X, Y += 20, width / 2, 20);
            ev.Graphics.DrawString("SGST @2.50%", font2, Brushes.Black, rectf, format1);
            rectf = Crect(X + width / 2 + 45, Y, width / 2 - 45, 20);
            ev.Graphics.DrawString(dictionary["SGST @2.50%"][0], font2, Brushes.Black, rectf, format3);
            rectf = Crect(X, Y += 20, width / 2, 20);
            ev.Graphics.DrawString("Total Tax", font2, Brushes.Black, rectf, format1);
            rectf = Crect(X + width / 2 + 45, Y, width / 2 - 45, 20);
            ev.Graphics.DrawString(dictionary["Total Tax"][0], font2, Brushes.Black, rectf, format3);
            int x5 = X;
            Y += 10;
            for (; x5 < W; x5 += 5)
                ev.Graphics.DrawString("-", font5, Brushes.Black, (float)x5, (float)Y);
            rectf = Crect(X, Y += 10, width / 2 + 25, 20);
            ev.Graphics.DrawString("NET PAYABLE(INR)", font1, Brushes.Black, rectf, format1);
            rectf = Crect(X + width / 2 + 45, Y, width / 2 - 45, 20);
            ev.Graphics.DrawString(dictionary["Net Payable(INR)"][0], font1, Brushes.Black, rectf, format1);
            int x6 = X;
            Y += 10;
            for (; x6 < W; x6 += 5)
                ev.Graphics.DrawString("-", font5, Brushes.Black, (float)x6, (float)Y);
            Y += 10;
            rectf = Crect(X, Y, W, 20);
            ev.Graphics.DrawString("THANK YOU!", font1, Brushes.Black, rectf, format1);
            rectf = Crect(X, Y += 20, W, 20);
            string str7 = DateTime.Now.ToString("F");
            Console.WriteLine(str7);
            ev.Graphics.DrawString("Printed on: " + str7, font2, Brushes.Black, rectf, format1);
            Console.WriteLine("success");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

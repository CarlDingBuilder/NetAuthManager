using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Helper;

public class ValidateCode
{
    private int length = 4;

    private string code = "";

    private int fontSize = 40;

    private int padding = 4;

    private bool chaos = true;

    private Color chaosColor = Color.LightGray;

    private Color backgroundColor = Color.White;

    private Color[] colors = new Color[8]
    {
        Color.Black,
        Color.Red,
        Color.DarkBlue,
        Color.Green,
        Color.Orange,
        Color.Brown,
        Color.DarkCyan,
        Color.Purple
    };

    private string[] fonts = new string[2] { "Arial", "Georgia" };

    private string codeSerial = "2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";

    private const double PI = Math.PI;

    private const double PI2 = Math.PI * 2.0;

    public int Length
    {
        get
        {
            return length;
        }
        set
        {
            length = value;
        }
    }

    public string Code
    {
        get
        {
            return code;
        }
        set
        {
            code = value;
        }
    }

    public int FontSize
    {
        get
        {
            return fontSize;
        }
        set
        {
            fontSize = value;
        }
    }

    public int Padding
    {
        get
        {
            return padding;
        }
        set
        {
            padding = value;
        }
    }

    public bool Chaos
    {
        get
        {
            return chaos;
        }
        set
        {
            chaos = value;
        }
    }

    public Color ChaosColor
    {
        get
        {
            return chaosColor;
        }
        set
        {
            chaosColor = value;
        }
    }

    public Color BackgroundColor
    {
        get
        {
            return backgroundColor;
        }
        set
        {
            backgroundColor = value;
        }
    }

    public Color[] Colors
    {
        get
        {
            return colors;
        }
        set
        {
            colors = value;
        }
    }

    public string[] Fonts
    {
        get
        {
            return fonts;
        }
        set
        {
            fonts = value;
        }
    }

    public string CodeSerial
    {
        get
        {
            return codeSerial;
        }
        set
        {
            codeSerial = value;
        }
    }

    public MemoryStream GetGifImage()
    {
        AnimatedGifEncoder encoder = new AnimatedGifEncoder();
        MemoryStream stream = new MemoryStream();
        MemoryStream outstream = new MemoryStream();
        string path = Path.GetTempPath();
        encoder.Start();
        encoder.SetDelay(300);
        encoder.SetRepeat(0);
        ValidateCode v = new ValidateCode();
        v.Length = length;
        v.FontSize = fontSize;
        v.Chaos = chaos;
        v.BackgroundColor = backgroundColor;
        v.ChaosColor = chaosColor;
        v.CodeSerial = codeSerial;
        v.Colors = colors;
        v.Fonts = fonts;
        v.Padding = padding;
        Code = v.CreateVerifyCode();
        Bitmap bgbitmap = CreateImageCode(code);
        for (int i = 0; i < 6; i++)
        {
            Bitmap bitmap = DrawCode(DeepCopyBitmap(bgbitmap), code);
            bitmap = DrawCurveLine(bitmap);
            bitmap = TwistImage(bitmap, bXDir: true, 0.0, 0.0);
            bitmap.Save(stream, ImageFormat.Png);
            encoder.AddFrame(Image.FromStream(stream));
            stream = new MemoryStream();
        }
        encoder.OutPut(ref stream);
        return stream;
    }

    public Bitmap DeepCopyBitmap(Bitmap bitmap)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                return new Bitmap(ms);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
    {
        Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
        Graphics graph = Graphics.FromImage(destBmp);
        graph.FillRectangle(new SolidBrush(Color.Green), 0, 0, destBmp.Width, destBmp.Height);
        graph.Dispose();
        double dBaseAxisLen = (bXDir ? ((double)destBmp.Height) : ((double)destBmp.Width));
        for (int i = 0; i < destBmp.Width; i++)
        {
            for (int j = 0; j < destBmp.Height; j++)
            {
                double dx = 0.0;
                dx = (bXDir ? (Math.PI * 2.0 * (double)j / dBaseAxisLen) : (Math.PI * 2.0 * (double)i / dBaseAxisLen));
                dx += dPhase;
                double dy = Math.Sin(dx);
                int nOldX = 0;
                int nOldY = 0;
                nOldX = (bXDir ? (i + (int)(dy * dMultValue)) : i);
                nOldY = (bXDir ? j : (j + (int)(dy * dMultValue)));
                Color color = srcBmp.GetPixel(i, j);
                if (nOldX >= 0 && nOldX < destBmp.Width && nOldY >= 0 && nOldY < destBmp.Height)
                {
                    destBmp.SetPixel(nOldX, nOldY, color);
                }
            }
        }
        return destBmp;
    }

    public Bitmap CreateImageCode(string code)
    {
        int fSize = FontSize;
        int fWidth = fSize + Padding;
        int imageWidth = code.Length * fWidth + 4 + Padding * 2;
        int imageHeight = fSize * 2 + Padding;
        Bitmap image = new Bitmap(imageWidth, imageHeight);
        Graphics g = Graphics.FromImage(image);
        g.Clear(BackgroundColor);
        Random rand = new Random();
        Pen pen = new Pen(Color.Green, 4f);
        if (Chaos)
        {
            pen = new Pen(ChaosColor, 30f);
            int c = Length * 10;
            for (int i = 0; i < c; i++)
            {
                int x = rand.Next(image.Width);
                int y = rand.Next(image.Height);
                g.DrawRectangle(pen, x, y, 1, 1);
            }
        }
        g.Dispose();
        return image;
    }

    public Bitmap DrawCode(Bitmap bit, string code)
    {
        int fSize = FontSize;
        int fWidth = fSize + Padding;
        int imageWidth = bit.Width;
        int imageHeight = bit.Height;
        Random rand = new Random();
        Pen pen = new Pen(Color.Green, 4f);
        Graphics g = Graphics.FromImage(bit);
        int left = 0;
        int top = 0;
        int top2 = 1;
        int top3 = 1;
        int n1 = imageHeight - FontSize - Padding * 2;
        int n2 = n1 / 4;
        top2 = n2;
        top3 = n2 * 2;
        int emptyindex = rand.Next(5);
        for (int i = 0; i < code.Length; i++)
        {
            int cindex = rand.Next(Colors.Length - 1);
            int findex = rand.Next(Fonts.Length - 1);
            Font f = new Font(Fonts[findex], fSize, FontStyle.Bold);
            Brush b = new SolidBrush(Colors[cindex]);
            top = ((i % 2 != 1) ? top2 : top3);
            top = rand.Next(top * 2);
            left = i * fWidth;
            float py = rand.Next(-45, 45);
            string scode = code.Substring(i, 1);
            if (emptyindex == i)
            {
                scode = "";
            }
            Image img = BuildBitmap(scode, f, b, py);
            Point point = new Point(left, top);
            g.DrawImage(img, point);
        }
        g.Dispose();
        return bit;
    }

    public Image BuildBitmap(string s, Font font, Brush b, float py)
    {
        Bitmap bitmap = new Bitmap(80, 80);
        Graphics g = Graphics.FromImage(bitmap);
        g.TranslateTransform(bitmap.Width / 2, bitmap.Height / 2);
        g.RotateTransform(py);
        SizeF size = g.MeasureString(s, font);
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.DrawString(s, font, b, ((float)bitmap.Width - size.Width) / 2f - (float)(bitmap.Width / 2), ((float)bitmap.Height - size.Height) / 2f - (float)(bitmap.Height / 2));
        return bitmap;
    }

    public Bitmap DrawCurveLine(Bitmap bit)
    {
        int imageWidth = bit.Width;
        int imageHeight = bit.Height;
        Random rand = new Random();
        Pen pen = new Pen(Color.Green, 4f);
        Graphics g = Graphics.FromImage(bit);
        pen = new Pen(Color.Green, 7f);
        int colorIndex = rand.Next(Colors.Length - 1);
        Pen CurvePen = new Pen(Colors[colorIndex], 8f);
        PointF[] CurvePointF = new PointF[4];
        switch (rand.Next(3))
        {
            case 0:
                g.DrawCurve(CurvePen, new PointF[4]
                {
                new PointF(0f, 0f),
                new PointF(imageWidth / 3, imageHeight),
                new PointF(2 * imageWidth / 3, 0f),
                new PointF(imageWidth, imageHeight)
                }, 0.5f);
                g.DrawCurve(CurvePen, new PointF[3]
                {
                new PointF(0f, 0f),
                new PointF(imageWidth / 2, imageHeight),
                new PointF(imageWidth, 0f)
                }, 0.5f);
                break;
            case 1:
                g.DrawCurve(CurvePen, new PointF[3]
                {
                new PointF(0f, 0f),
                new PointF(imageWidth / 2, imageHeight),
                new PointF(imageWidth, 0f)
                }, 0.5f);
                break;
            default:
                g.DrawCurve(CurvePen, new PointF[4]
                {
                new PointF(0f, 0f),
                new PointF(imageWidth / 3, imageHeight),
                new PointF(2 * imageWidth / 3, 0f),
                new PointF(imageWidth, imageHeight)
                }, 0.5f);
                break;
        }
        g.DrawRectangle(new Pen(Color.Gainsboro, 0f), 0, 0, bit.Width - 1, bit.Height - 1);
        g.Dispose();
        return bit;
    }

    //public void CreateImageOnPage(string code, HttpContext context)
    //{
    //    context.Response.Buffer = true;
    //    context.Response.ExpiresAbsolute = DateTime.Now - new TimeSpan(1, 0, 0);
    //    context.Response.Expires = 0;
    //    context.Response.CacheControl = "no-cache";
    //    MemoryStream ms = new MemoryStream();
    //    Bitmap image = CreateImageCode(code);
    //    image.Save(ms, ImageFormat.Jpeg);
    //    context.Response.ClearContent();
    //    context.Response.ContentType = "image/Jpeg";
    //    context.Response.BinaryWrite(ms.GetBuffer());
    //    ms.Close();
    //    ms = null;
    //    image.Dispose();
    //    image = null;
    //}

    public string CreateVerifyCode(int codeLen)
    {
        if (codeLen == 0)
        {
            codeLen = Length;
        }
        string[] arr = CodeSerial.Split(',');
        string code = "";
        int randValue = -1;
        Random rand = new Random((int)DateTime.Now.Ticks);
        for (int i = 0; i < codeLen; i++)
        {
            randValue = rand.Next(0, arr.Length - 1);
            code += arr[randValue];
        }
        return code;
    }

    public string CreateVerifyCode()
    {
        return CreateVerifyCode(0);
    }
}

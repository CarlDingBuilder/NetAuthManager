using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetAuthManager.Application.Base.Results;

namespace NetAuthManager.Application;

public class ValidateCodeService: IValidateCodeService, ITransient
{
    #region 注入与构造

    private readonly ICacheService _cacheService;
    public ValidateCodeService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    #endregion 注入与构造

    /// <summary>
    /// 获取验证码
    /// </summary>
    /// <returns></returns>
    public GetValidateCodeResult GetValidateCode()
    {
        Tuple<string, string> mcode = GetCode();
        string base64String = "data:image/gif;base64," + mcode.Item2;
        var keystore = Guid.NewGuid().ToString();
        _cacheService.Add(keystore, mcode.Item1);
        return new GetValidateCodeResult
        {
            Base64String = base64String,
            Keystore = keystore
        };
    }

    private Tuple<string, string> GetCode()
    {
        int codeW = 80;
        int codeH = 22;
        int fontSize = 16;
        string chkCode = string.Empty;
        Color[] color = new Color[8]
        {
            Color.Black,
            Color.Red,
            Color.Blue,
            Color.Green,
            Color.Orange,
            Color.Brown,
            Color.Brown,
            Color.DarkBlue
        };
        string[] font = new string[5] { "Times New Roman", "Verdana", "Arial", "Gungsuh", "Impact" };
        char[] character = new char[39]
        {
            '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'd',
            'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A',
            'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L',
            'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y'
        };
        Random rnd = new Random();
        for (int l = 0; l < 4; l++)
        {
            chkCode += character[rnd.Next(character.Length)];
        }
        Bitmap bmp = new Bitmap(codeW, codeH);
        Graphics g = Graphics.FromImage(bmp);
        g.Clear(Color.White);
        for (int k = 0; k < 1; k++)
        {
            int x2 = rnd.Next(codeW);
            int y2 = rnd.Next(codeH);
            int x3 = rnd.Next(codeW);
            int y3 = rnd.Next(codeH);
            Color clr = color[rnd.Next(color.Length)];
            g.DrawLine(new Pen(clr), x2, y2, x3, y3);
        }
        for (int j = 0; j < chkCode.Length; j++)
        {
            string fnt = font[rnd.Next(font.Length)];
            Font ft = new Font(fnt, fontSize);
            Color clr2 = color[rnd.Next(color.Length)];
            g.DrawString(chkCode[j].ToString(), ft, new SolidBrush(clr2), (float)j * 18f + 2f, 0f);
        }
        for (int i = 0; i < 100; i++)
        {
            int x = rnd.Next(bmp.Width);
            int y = rnd.Next(bmp.Height);
            Color clr3 = color[rnd.Next(color.Length)];
            bmp.SetPixel(x, y, clr3);
        }
        MemoryStream ms = new MemoryStream();
        try
        {
            bmp.Save(ms, ImageFormat.Png);
            return new Tuple<string, string>(chkCode, Convert.ToBase64String(ms.ToArray()));
        }
        finally
        {
            bmp.Dispose();
            g.Dispose();
        }
    }
}

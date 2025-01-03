using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Helper;

public class AnimatedGifEncoder
{
    protected int width;

    protected int height;

    protected Color transparent = Color.Empty;

    protected int transIndex;

    protected int repeat = -1;

    protected int delay = 0;

    protected bool started = false;

    protected Image image;

    protected byte[] pixels;

    protected byte[] indexedPixels;

    protected int colorDepth;

    protected byte[] colorTab;

    protected MemoryStream Memory;

    protected bool[] usedEntry = new bool[256];

    protected int palSize = 7;

    protected int dispose = -1;

    protected bool closeStream = false;

    protected bool firstFrame = true;

    protected bool sizeSet = false;

    protected int sample = 10;

    public void SetDelay(int ms)
    {
        delay = (int)Math.Round((float)ms / 10f);
    }

    public void SetDispose(int code)
    {
        if (code >= 0)
        {
            dispose = code;
        }
    }

    public void SetRepeat(int iter)
    {
        if (iter >= 0)
        {
            repeat = iter;
        }
    }

    public void SetTransparent(Color c)
    {
        transparent = c;
    }

    public bool AddFrame(Image im)
    {
        if (im == null || !started)
        {
            return false;
        }
        bool ok = true;
        try
        {
            if (!sizeSet)
            {
                SetSize(im.Width, im.Height);
            }
            image = im;
            GetImagePixels();
            AnalyzePixels();
            if (firstFrame)
            {
                WriteLSD();
                WritePalette();
                if (repeat >= 0)
                {
                    WriteNetscapeExt();
                }
            }
            WriteGraphicCtrlExt();
            WriteImageDesc();
            if (!firstFrame)
            {
                WritePalette();
            }
            WritePixels();
            firstFrame = false;
        }
        catch (IOException)
        {
            ok = false;
        }
        return ok;
    }

    public bool Finish()
    {
        if (!started)
        {
            return false;
        }
        bool ok = true;
        started = false;
        try
        {
            Memory.WriteByte(59);
            Memory.Flush();
            if (closeStream)
            {
                Memory.Close();
            }
        }
        catch (IOException)
        {
            ok = false;
        }
        transIndex = 0;
        Memory = null;
        image = null;
        pixels = null;
        indexedPixels = null;
        colorTab = null;
        closeStream = false;
        firstFrame = true;
        return ok;
    }

    public void OutPut(ref MemoryStream MemoryResult)
    {
        started = false;
        Memory.WriteByte(59);
        Memory.Flush();
        MemoryResult = Memory;
        Memory.Close();
        Memory.Dispose();
        transIndex = 0;
        Memory = null;
        image = null;
        pixels = null;
        indexedPixels = null;
        colorTab = null;
        firstFrame = true;
    }

    public void SetFrameRate(float fps)
    {
        if (fps != 0f)
        {
            delay = (int)Math.Round(100f / fps);
        }
    }

    public void SetQuality(int quality)
    {
        if (quality < 1)
        {
            quality = 1;
        }
        sample = quality;
    }

    public void SetSize(int w, int h)
    {
        if (!started || firstFrame)
        {
            width = w;
            height = h;
            if (width < 1)
            {
                width = 320;
            }
            if (height < 1)
            {
                height = 240;
            }
            sizeSet = true;
        }
    }

    public void Start()
    {
        Start(new MemoryStream());
    }

    public bool Start(MemoryStream memory)
    {
        if (memory == null)
        {
            return false;
        }
        bool ok = true;
        closeStream = false;
        Memory = memory;
        try
        {
            WriteString("GIF89a");
        }
        catch (IOException)
        {
            ok = false;
        }
        return started = ok;
    }

    protected void AnalyzePixels()
    {
        int len = pixels.Length;
        int nPix = len / 3;
        indexedPixels = new byte[nPix];
        NeuQuant nq = new NeuQuant(pixels, len, sample);
        colorTab = nq.Process();
        int j = 0;
        for (int i = 0; i < nPix; i++)
        {
            int index = nq.Map(pixels[j++] & 0xFF, pixels[j++] & 0xFF, pixels[j++] & 0xFF);
            usedEntry[index] = true;
            indexedPixels[i] = (byte)index;
        }
        pixels = null;
        colorDepth = 8;
        palSize = 7;
        if (transparent != Color.Empty)
        {
            transIndex = FindClosest(transparent);
        }
    }

    protected int FindClosest(Color c)
    {
        if (colorTab == null)
        {
            return -1;
        }
        int r = c.R;
        int g = c.G;
        int b = c.B;
        int minpos = 0;
        int dmin = 16777216;
        int len = colorTab.Length;
        for (int i = 0; i < len; i++)
        {
            int dr = r - (colorTab[i++] & 0xFF);
            int dg = g - (colorTab[i++] & 0xFF);
            int db = b - (colorTab[i] & 0xFF);
            int d = dr * dr + dg * dg + db * db;
            int index = i / 3;
            if (usedEntry[index] && d < dmin)
            {
                dmin = d;
                minpos = index;
            }
        }
        return minpos;
    }

    protected void GetImagePixels()
    {
        int w = image.Width;
        int h = image.Height;
        if (w != width || h != height)
        {
            Image temp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(temp);
            g.DrawImage(image, 0, 0);
            image = temp;
            g.Dispose();
        }
        pixels = new byte[3 * image.Width * image.Height];
        int count = 0;
        Bitmap tempBitmap = new Bitmap(image);
        for (int th = 0; th < image.Height; th++)
        {
            for (int tw = 0; tw < image.Width; tw++)
            {
                Color color = tempBitmap.GetPixel(tw, th);
                pixels[count] = color.R;
                count++;
                pixels[count] = color.G;
                count++;
                pixels[count] = color.B;
                count++;
            }
        }
    }

    protected void WriteGraphicCtrlExt()
    {
        Memory.WriteByte(33);
        Memory.WriteByte(249);
        Memory.WriteByte(4);
        int transp;
        int disp;
        if (transparent == Color.Empty)
        {
            transp = 0;
            disp = 0;
        }
        else
        {
            transp = 1;
            disp = 2;
        }
        if (dispose >= 0)
        {
            disp = dispose & 7;
        }
        disp <<= 2;
        Memory.WriteByte(Convert.ToByte(0 | disp | 0 | transp));
        WriteShort(delay);
        Memory.WriteByte(Convert.ToByte(transIndex));
        Memory.WriteByte(0);
    }

    protected void WriteImageDesc()
    {
        Memory.WriteByte(44);
        WriteShort(0);
        WriteShort(0);
        WriteShort(width);
        WriteShort(height);
        if (firstFrame)
        {
            Memory.WriteByte(0);
        }
        else
        {
            Memory.WriteByte(Convert.ToByte(0x80 | palSize));
        }
    }

    protected void WriteLSD()
    {
        WriteShort(width);
        WriteShort(height);
        Memory.WriteByte(Convert.ToByte(0xF0 | palSize));
        Memory.WriteByte(0);
        Memory.WriteByte(0);
    }

    protected void WriteNetscapeExt()
    {
        Memory.WriteByte(33);
        Memory.WriteByte(byte.MaxValue);
        Memory.WriteByte(11);
        WriteString("NETSCAPE2.0");
        Memory.WriteByte(3);
        Memory.WriteByte(1);
        WriteShort(repeat);
        Memory.WriteByte(0);
    }

    protected void WritePalette()
    {
        Memory.Write(colorTab, 0, colorTab.Length);
        int j = 768 - colorTab.Length;
        for (int i = 0; i < j; i++)
        {
            Memory.WriteByte(0);
        }
    }

    protected void WritePixels()
    {
        LZWEncoder encoder = new LZWEncoder(width, height, indexedPixels, colorDepth);
        encoder.Encode(Memory);
    }

    protected void WriteShort(int value)
    {
        Memory.WriteByte(Convert.ToByte(value & 0xFF));
        Memory.WriteByte(Convert.ToByte((value >> 8) & 0xFF));
    }

    protected void WriteString(string s)
    {
        char[] chars = s.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            Memory.WriteByte((byte)chars[i]);
        }
    }
}

public class NeuQuant
{
protected static readonly int netsize = 256;

protected static readonly int prime1 = 499;

protected static readonly int prime2 = 491;

protected static readonly int prime3 = 487;

protected static readonly int prime4 = 503;

protected static readonly int minpicturebytes = 3 * prime4;

protected static readonly int maxnetpos = netsize - 1;

protected static readonly int netbiasshift = 4;

protected static readonly int ncycles = 100;

protected static readonly int intbiasshift = 16;

protected static readonly int intbias = 1 << intbiasshift;

protected static readonly int gammashift = 10;

protected static readonly int gamma = 1 << gammashift;

protected static readonly int betashift = 10;

protected static readonly int beta = intbias >> betashift;

protected static readonly int betagamma = intbias << gammashift - betashift;

protected static readonly int initrad = netsize >> 3;

protected static readonly int radiusbiasshift = 6;

protected static readonly int radiusbias = 1 << radiusbiasshift;

protected static readonly int initradius = initrad * radiusbias;

protected static readonly int radiusdec = 30;

protected static readonly int alphabiasshift = 10;

protected static readonly int initalpha = 1 << alphabiasshift;

protected int alphadec;

protected static readonly int radbiasshift = 8;

protected static readonly int radbias = 1 << radbiasshift;

protected static readonly int alpharadbshift = alphabiasshift + radbiasshift;

protected static readonly int alpharadbias = 1 << alpharadbshift;

protected byte[] thepicture;

protected int lengthcount;

protected int samplefac;

protected int[][] network;

protected int[] netindex = new int[256];

protected int[] bias = new int[netsize];

protected int[] freq = new int[netsize];

protected int[] radpower = new int[initrad];

public NeuQuant(byte[] thepic, int len, int sample)
{
    thepicture = thepic;
    lengthcount = len;
    samplefac = sample;
    network = new int[netsize][];
    for (int i = 0; i < netsize; i++)
    {
        network[i] = new int[4];
        int[] p = network[i];
        p[0] = (p[1] = (p[2] = (i << netbiasshift + 8) / netsize));
        freq[i] = intbias / netsize;
        bias[i] = 0;
    }
}

public byte[] ColorMap()
{
    byte[] map = new byte[3 * netsize];
    int[] index = new int[netsize];
    for (int j = 0; j < netsize; j++)
    {
        index[network[j][3]] = j;
    }
    int l = 0;
    for (int i = 0; i < netsize; i++)
    {
        int k = index[i];
        map[l++] = (byte)network[k][0];
        map[l++] = (byte)network[k][1];
        map[l++] = (byte)network[k][2];
    }
    return map;
}

public void Inxbuild()
{
    int previouscol = 0;
    int startpos = 0;
    for (int i = 0; i < netsize; i++)
    {
        int[] p = network[i];
        int smallpos = i;
        int smallval = p[1];
        int[] q;
        for (int j = i + 1; j < netsize; j++)
        {
            q = network[j];
            if (q[1] < smallval)
            {
                smallpos = j;
                smallval = q[1];
            }
        }
        q = network[smallpos];
        if (i != smallpos)
        {
            int j = q[0];
            q[0] = p[0];
            p[0] = j;
            j = q[1];
            q[1] = p[1];
            p[1] = j;
            j = q[2];
            q[2] = p[2];
            p[2] = j;
            j = q[3];
            q[3] = p[3];
            p[3] = j;
        }
        if (smallval != previouscol)
        {
            netindex[previouscol] = startpos + i >> 1;
            for (int j = previouscol + 1; j < smallval; j++)
            {
                netindex[j] = i;
            }
            previouscol = smallval;
            startpos = i;
        }
    }
    netindex[previouscol] = startpos + maxnetpos >> 1;
    for (int j = previouscol + 1; j < 256; j++)
    {
        netindex[j] = maxnetpos;
    }
}

public void Learn()
{
    if (lengthcount < minpicturebytes)
    {
        samplefac = 1;
    }
    alphadec = 30 + (samplefac - 1) / 3;
    byte[] p = thepicture;
    int pix = 0;
    int lim = lengthcount;
    int samplepixels = lengthcount / (3 * samplefac);
    int delta = samplepixels / ncycles;
    int alpha = initalpha;
    int radius = initradius;
    int rad = radius >> radiusbiasshift;
    if (rad <= 1)
    {
        rad = 0;
    }
    int i;
    for (i = 0; i < rad; i++)
    {
        radpower[i] = alpha * ((rad * rad - i * i) * radbias / (rad * rad));
    }
    int step = ((lengthcount < minpicturebytes) ? 3 : ((lengthcount % prime1 != 0) ? (3 * prime1) : ((lengthcount % prime2 != 0) ? (3 * prime2) : ((lengthcount % prime3 == 0) ? (3 * prime4) : (3 * prime3)))));
    i = 0;
    while (i < samplepixels)
    {
        int b = (p[pix] & 0xFF) << netbiasshift;
        int g = (p[pix + 1] & 0xFF) << netbiasshift;
        int r = (p[pix + 2] & 0xFF) << netbiasshift;
        int j = Contest(b, g, r);
        Altersingle(alpha, j, b, g, r);
        if (rad != 0)
        {
            Alterneigh(rad, j, b, g, r);
        }
        pix += step;
        if (pix >= lim)
        {
            pix -= lengthcount;
        }
        i++;
        if (delta == 0)
        {
            delta = 1;
        }
        if (i % delta == 0)
        {
            alpha -= alpha / alphadec;
            radius -= radius / radiusdec;
            rad = radius >> radiusbiasshift;
            if (rad <= 1)
            {
                rad = 0;
            }
            for (j = 0; j < rad; j++)
            {
                radpower[j] = alpha * ((rad * rad - j * j) * radbias / (rad * rad));
            }
        }
    }
}

public int Map(int b, int g, int r)
{
    int bestd = 1000;
    int best = -1;
    int i = netindex[g];
    int j = i - 1;
    while (i < netsize || j >= 0)
    {
        int[] p;
        int a;
        int dist;
        if (i < netsize)
        {
            p = network[i];
            dist = p[1] - g;
            if (dist >= bestd)
            {
                i = netsize;
            }
            else
            {
                i++;
                if (dist < 0)
                {
                    dist = -dist;
                }
                a = p[0] - b;
                if (a < 0)
                {
                    a = -a;
                }
                dist += a;
                if (dist < bestd)
                {
                    a = p[2] - r;
                    if (a < 0)
                    {
                        a = -a;
                    }
                    dist += a;
                    if (dist < bestd)
                    {
                        bestd = dist;
                        best = p[3];
                    }
                }
            }
        }
        if (j < 0)
        {
            continue;
        }
        p = network[j];
        dist = g - p[1];
        if (dist >= bestd)
        {
            j = -1;
            continue;
        }
        j--;
        if (dist < 0)
        {
            dist = -dist;
        }
        a = p[0] - b;
        if (a < 0)
        {
            a = -a;
        }
        dist += a;
        if (dist < bestd)
        {
            a = p[2] - r;
            if (a < 0)
            {
                a = -a;
            }
            dist += a;
            if (dist < bestd)
            {
                bestd = dist;
                best = p[3];
            }
        }
    }
    return best;
}

public byte[] Process()
{
    Learn();
    Unbiasnet();
    Inxbuild();
    return ColorMap();
}

public void Unbiasnet()
{
    for (int i = 0; i < netsize; i++)
    {
        network[i][0] >>= netbiasshift;
        network[i][1] >>= netbiasshift;
        network[i][2] >>= netbiasshift;
        network[i][3] = i;
    }
}

protected void Alterneigh(int rad, int i, int b, int g, int r)
{
    int lo = i - rad;
    if (lo < -1)
    {
        lo = -1;
    }
    int hi = i + rad;
    if (hi > netsize)
    {
        hi = netsize;
    }
    int j = i + 1;
    int k = i - 1;
    int l = 1;
    while (j < hi || k > lo)
    {
        int a = radpower[l++];
        if (j < hi)
        {
            int[] p = network[j++];
            try
            {
                p[0] -= a * (p[0] - b) / alpharadbias;
                p[1] -= a * (p[1] - g) / alpharadbias;
                p[2] -= a * (p[2] - r) / alpharadbias;
            }
            catch (Exception)
            {
            }
        }
        if (k > lo)
        {
            int[] p = network[k--];
            try
            {
                p[0] -= a * (p[0] - b) / alpharadbias;
                p[1] -= a * (p[1] - g) / alpharadbias;
                p[2] -= a * (p[2] - r) / alpharadbias;
            }
            catch (Exception)
            {
            }
        }
    }
}

protected void Altersingle(int alpha, int i, int b, int g, int r)
{
    int[] j = network[i];
    j[0] -= alpha * (j[0] - b) / initalpha;
    j[1] -= alpha * (j[1] - g) / initalpha;
    j[2] -= alpha * (j[2] - r) / initalpha;
}

protected int Contest(int b, int g, int r)
{
    int bestd = int.MaxValue;
    int bestbiasd = bestd;
    int bestpos = -1;
    int bestbiaspos = bestpos;
    for (int i = 0; i < netsize; i++)
    {
        int[] j = network[i];
        int dist = j[0] - b;
        if (dist < 0)
        {
            dist = -dist;
        }
        int a = j[1] - g;
        if (a < 0)
        {
            a = -a;
        }
        dist += a;
        a = j[2] - r;
        if (a < 0)
        {
            a = -a;
        }
        dist += a;
        if (dist < bestd)
        {
            bestd = dist;
            bestpos = i;
        }
        int biasdist = dist - (bias[i] >> intbiasshift - netbiasshift);
        if (biasdist < bestbiasd)
        {
            bestbiasd = biasdist;
            bestbiaspos = i;
        }
        int betafreq = freq[i] >> betashift;
        freq[i] -= betafreq;
        bias[i] += betafreq << gammashift;
    }
    freq[bestpos] += beta;
    bias[bestpos] -= betagamma;
    return bestbiaspos;
}
}

public class LZWEncoder
{
private static readonly int EOF = -1;

private int imgW;

private int imgH;

private byte[] pixAry;

private int initCodeSize;

private int remaining;

private int curPixel;

private static readonly int BITS = 12;

private static readonly int HSIZE = 5003;

private int n_bits;

private int maxbits = BITS;

private int maxcode;

private int maxmaxcode = 1 << BITS;

private int[] htab = new int[HSIZE];

private int[] codetab = new int[HSIZE];

private int hsize = HSIZE;

private int free_ent = 0;

private bool clear_flg = false;

private int g_init_bits;

private int ClearCode;

private int EOFCode;

private int cur_accum = 0;

private int cur_bits = 0;

private int[] masks = new int[17]
{
    0, 1, 3, 7, 15, 31, 63, 127, 255, 511,
    1023, 2047, 4095, 8191, 16383, 32767, 65535
};

private int a_count;

private byte[] accum = new byte[256];

public LZWEncoder(int width, int height, byte[] pixels, int color_depth)
{
    imgW = width;
    imgH = height;
    pixAry = pixels;
    initCodeSize = Math.Max(2, color_depth);
}

private void Add(byte c, Stream outs)
{
    accum[a_count++] = c;
    if (a_count >= 254)
    {
        Flush(outs);
    }
}

private void ClearTable(Stream outs)
{
    ResetCodeTable(hsize);
    free_ent = ClearCode + 2;
    clear_flg = true;
    Output(ClearCode, outs);
}

private void ResetCodeTable(int hsize)
{
    for (int i = 0; i < hsize; i++)
    {
        htab[i] = -1;
    }
}

private void Compress(int init_bits, Stream outs)
{
    g_init_bits = init_bits;
    clear_flg = false;
    n_bits = g_init_bits;
    maxcode = MaxCode(n_bits);
    ClearCode = 1 << init_bits - 1;
    EOFCode = ClearCode + 1;
    free_ent = ClearCode + 2;
    a_count = 0;
    int ent = NextPixel();
    int hshift = 0;
    for (int fcode = hsize; fcode < 65536; fcode *= 2)
    {
        hshift++;
    }
    hshift = 8 - hshift;
    int hsize_reg = hsize;
    ResetCodeTable(hsize_reg);
    Output(ClearCode, outs);
    int c;
    while ((c = NextPixel()) != EOF)
    {
        int fcode = (c << maxbits) + ent;
        int i = (c << hshift) ^ ent;
        if (htab[i] == fcode)
        {
            ent = codetab[i];
            continue;
        }
        if (htab[i] >= 0)
        {
            int disp = hsize_reg - i;
            if (i == 0)
            {
                disp = 1;
            }
            while (true)
            {
                if ((i -= disp) < 0)
                {
                    i += hsize_reg;
                }
                if (htab[i] == fcode)
                {
                    break;
                }
                if (htab[i] >= 0)
                {
                    continue;
                }
                goto IL_0160;
            }
            ent = codetab[i];
            continue;
        }
        goto IL_0160;
    IL_0160:
        Output(ent, outs);
        ent = c;
        if (free_ent < maxmaxcode)
        {
            codetab[i] = free_ent++;
            htab[i] = fcode;
        }
        else
        {
            ClearTable(outs);
        }
    }
    Output(ent, outs);
    Output(EOFCode, outs);
}

public void Encode(Stream os)
{
    os.WriteByte(Convert.ToByte(initCodeSize));
    remaining = imgW * imgH;
    curPixel = 0;
    Compress(initCodeSize + 1, os);
    os.WriteByte(0);
}

private void Flush(Stream outs)
{
    if (a_count > 0)
    {
        outs.WriteByte(Convert.ToByte(a_count));
        outs.Write(accum, 0, a_count);
        a_count = 0;
    }
}

private int MaxCode(int n_bits)
{
    return (1 << n_bits) - 1;
}

private int NextPixel()
{
    if (remaining == 0)
    {
        return EOF;
    }
    remaining--;
    int temp = curPixel + 1;
    if (temp < pixAry.GetUpperBound(0))
    {
        byte pix = pixAry[curPixel++];
        return pix & 0xFF;
    }
    return 255;
}

private void Output(int code, Stream outs)
{
    cur_accum &= masks[cur_bits];
    if (cur_bits > 0)
    {
        cur_accum |= code << cur_bits;
    }
    else
    {
        cur_accum = code;
    }
    cur_bits += n_bits;
    while (cur_bits >= 8)
    {
        Add((byte)((uint)cur_accum & 0xFFu), outs);
        cur_accum >>= 8;
        cur_bits -= 8;
    }
    if (free_ent > maxcode || clear_flg)
    {
        if (clear_flg)
        {
            maxcode = MaxCode(n_bits = g_init_bits);
            clear_flg = false;
        }
        else
        {
            n_bits++;
            if (n_bits == maxbits)
            {
                maxcode = maxmaxcode;
            }
            else
            {
                maxcode = MaxCode(n_bits);
            }
        }
    }
    if (code == EOFCode)
    {
        while (cur_bits > 0)
        {
            Add((byte)((uint)cur_accum & 0xFFu), outs);
            cur_accum >>= 8;
            cur_bits -= 8;
        }
        Flush(outs);
    }
}
}
public class GifDecoder
{
    public class GifFrame
    {
        public Image image;

        public int delay;

        public GifFrame(Image im, int del)
        {
            image = im;
            delay = del;
        }
    }

    public static readonly int STATUS_OK = 0;

    public static readonly int STATUS_FORMAT_ERROR = 1;

    public static readonly int STATUS_OPEN_ERROR = 2;

    protected Stream inStream;

    protected int status;

    protected int width;

    protected int height;

    protected bool gctFlag;

    protected int gctSize;

    protected int loopCount = 1;

    protected int[] gct;

    protected int[] lct;

    protected int[] act;

    protected int bgIndex;

    protected int bgColor;

    protected int lastBgColor;

    protected int pixelAspect;

    protected bool lctFlag;

    protected bool interlace;

    protected int lctSize;

    protected int ix;

    protected int iy;

    protected int iw;

    protected int ih;

    protected Rectangle lastRect;

    protected Image image;

    protected Bitmap bitmap;

    protected Image lastImage;

    protected byte[] block = new byte[256];

    protected int blockSize = 0;

    protected int dispose = 0;

    protected int lastDispose = 0;

    protected bool transparency = false;

    protected int delay = 0;

    protected int transIndex;

    protected static readonly int MaxStackSize = 4096;

    protected short[] prefix;

    protected byte[] suffix;

    protected byte[] pixelStack;

    protected byte[] pixels;

    protected ArrayList frames;

    protected int frameCount;

    public int GetDelay(int n)
    {
        delay = -1;
        if (n >= 0 && n < frameCount)
        {
            delay = ((GifFrame)frames[n]).delay;
        }
        return delay;
    }

    public int GetFrameCount()
    {
        return frameCount;
    }

    public Image GetImage()
    {
        return GetFrame(0);
    }

    public int GetLoopCount()
    {
        return loopCount;
    }

    private int[] GetPixels(Bitmap bitmap)
    {
        int[] pixels = new int[3 * image.Width * image.Height];
        int count = 0;
        for (int th = 0; th < image.Height; th++)
        {
            for (int tw = 0; tw < image.Width; tw++)
            {
                Color color = bitmap.GetPixel(tw, th);
                pixels[count] = color.R;
                count++;
                pixels[count] = color.G;
                count++;
                pixels[count] = color.B;
                count++;
            }
        }
        return pixels;
    }

    private void SetPixels(int[] pixels)
    {
        int count = 0;
        for (int th = 0; th < image.Height; th++)
        {
            for (int tw = 0; tw < image.Width; tw++)
            {
                Color color = Color.FromArgb(pixels[count++]);
                bitmap.SetPixel(tw, th, color);
            }
        }
    }

    protected void SetPixels()
    {
        int[] dest = GetPixels(bitmap);
        if (lastDispose > 0)
        {
            if (lastDispose == 3)
            {
                int k = frameCount - 2;
                if (k > 0)
                {
                    lastImage = GetFrame(k - 1);
                }
                else
                {
                    lastImage = null;
                }
            }
            if (lastImage != null)
            {
                int[] prev = GetPixels(new Bitmap(lastImage));
                Array.Copy(prev, 0, dest, 0, width * height);
                if (lastDispose == 2)
                {
                    Graphics g = Graphics.FromImage(image);
                    Color c2 = Color.Empty;
                    c2 = ((!transparency) ? Color.FromArgb(lastBgColor) : Color.FromArgb(0, 0, 0, 0));
                    Brush brush = new SolidBrush(c2);
                    g.FillRectangle(brush, lastRect);
                    brush.Dispose();
                    g.Dispose();
                }
            }
        }
        int pass = 1;
        int inc = 8;
        int iline = 0;
        for (int i = 0; i < ih; i++)
        {
            int line = i;
            if (interlace)
            {
                if (iline >= ih)
                {
                    pass++;
                    switch (pass)
                    {
                        case 2:
                            iline = 4;
                            break;
                        case 3:
                            iline = 2;
                            inc = 4;
                            break;
                        case 4:
                            iline = 1;
                            inc = 2;
                            break;
                    }
                }
                line = iline;
                iline += inc;
            }
            line += iy;
            if (line >= height)
            {
                continue;
            }
            int j = line * width;
            int dx = j + ix;
            int dlim = dx + iw;
            if (j + width < dlim)
            {
                dlim = j + width;
            }
            int sx = i * iw;
            for (; dx < dlim; dx++)
            {
                int index = pixels[sx++] & 0xFF;
                int c = act[index];
                if (c != 0)
                {
                    dest[dx] = c;
                }
            }
        }
        SetPixels(dest);
    }

    public Image GetFrame(int n)
    {
        Image im = null;
        if (n >= 0 && n < frameCount)
        {
            im = ((GifFrame)frames[n]).image;
        }
        return im;
    }

    public Size GetFrameSize()
    {
        return new Size(width, height);
    }

    public int Read(Stream inStream)
    {
        Init();
        if (inStream != null)
        {
            this.inStream = inStream;
            ReadHeader();
            if (!Error())
            {
                ReadContents();
                if (frameCount < 0)
                {
                    status = STATUS_FORMAT_ERROR;
                }
            }
            inStream.Close();
        }
        else
        {
            status = STATUS_OPEN_ERROR;
        }
        return status;
    }

    public int Read(string name)
    {
        status = STATUS_OK;
        try
        {
            name = name.Trim().ToLower();
            status = Read(new FileInfo(name).OpenRead());
        }
        catch (IOException)
        {
            status = STATUS_OPEN_ERROR;
        }
        return status;
    }

    protected void DecodeImageData()
    {
        int NullCode = -1;
        int npix = iw * ih;
        if (pixels == null || pixels.Length < npix)
        {
            pixels = new byte[npix];
        }
        if (prefix == null)
        {
            prefix = new short[MaxStackSize];
        }
        if (suffix == null)
        {
            suffix = new byte[MaxStackSize];
        }
        if (pixelStack == null)
        {
            pixelStack = new byte[MaxStackSize + 1];
        }
        int data_size = Read();
        int clear = 1 << data_size;
        int end_of_information = clear + 1;
        int available = clear + 2;
        int old_code = NullCode;
        int code_size = data_size + 1;
        int code_mask = (1 << code_size) - 1;
        for (int code = 0; code < clear; code++)
        {
            prefix[code] = 0;
            suffix[code] = (byte)code;
        }
        int bits;
        int count;
        int first;
        int top;
        int pi;
        int bi;
        int datum = (bits = (count = (first = (top = (pi = (bi = 0))))));
        int i = 0;
        while (i < npix)
        {
            if (top == 0)
            {
                if (bits < code_size)
                {
                    if (count == 0)
                    {
                        count = ReadBlock();
                        if (count <= 0)
                        {
                            break;
                        }
                        bi = 0;
                    }
                    datum += (block[bi] & 0xFF) << bits;
                    bits += 8;
                    bi++;
                    count--;
                    continue;
                }
                int code = datum & code_mask;
                datum >>= code_size;
                bits -= code_size;
                if (code > available || code == end_of_information)
                {
                    break;
                }
                if (code == clear)
                {
                    code_size = data_size + 1;
                    code_mask = (1 << code_size) - 1;
                    available = clear + 2;
                    old_code = NullCode;
                    continue;
                }
                if (old_code == NullCode)
                {
                    pixelStack[top++] = suffix[code];
                    old_code = code;
                    first = code;
                    continue;
                }
                int in_code = code;
                if (code == available)
                {
                    pixelStack[top++] = (byte)first;
                    code = old_code;
                }
                while (code > clear)
                {
                    pixelStack[top++] = suffix[code];
                    code = prefix[code];
                }
                first = suffix[code] & 0xFF;
                if (available >= MaxStackSize)
                {
                    break;
                }
                pixelStack[top++] = (byte)first;
                prefix[available] = (short)old_code;
                suffix[available] = (byte)first;
                available++;
                if ((available & code_mask) == 0 && available < MaxStackSize)
                {
                    code_size++;
                    code_mask += available;
                }
                old_code = in_code;
            }
            top--;
            pixels[pi++] = pixelStack[top];
            i++;
        }
        for (i = pi; i < npix; i++)
        {
            pixels[i] = 0;
        }
    }

    protected bool Error()
    {
        return status != STATUS_OK;
    }

    protected void Init()
    {
        status = STATUS_OK;
        frameCount = 0;
        frames = new ArrayList();
        gct = null;
        lct = null;
    }

    protected int Read()
    {
        int curByte = 0;
        try
        {
            curByte = inStream.ReadByte();
        }
        catch (IOException)
        {
            status = STATUS_FORMAT_ERROR;
        }
        return curByte;
    }

    protected int ReadBlock()
    {
        blockSize = Read();
        int i = 0;
        if (blockSize > 0)
        {
            try
            {
                int count = 0;
                for (; i < blockSize; i += count)
                {
                    count = inStream.Read(block, i, blockSize - i);
                    if (count == -1)
                    {
                        break;
                    }
                }
            }
            catch (IOException)
            {
            }
            if (i < blockSize)
            {
                status = STATUS_FORMAT_ERROR;
            }
        }
        return i;
    }

    protected int[] ReadColorTable(int ncolors)
    {
        int nbytes = 3 * ncolors;
        int[] tab = null;
        byte[] c = new byte[nbytes];
        int k = 0;
        try
        {
            k = inStream.Read(c, 0, c.Length);
        }
        catch (IOException)
        {
        }
        if (k < nbytes)
        {
            status = STATUS_FORMAT_ERROR;
        }
        else
        {
            tab = new int[256];
            int i = 0;
            int j = 0;
            while (i < ncolors)
            {
                int r = c[j++] & 0xFF;
                int g = c[j++] & 0xFF;
                int b = c[j++] & 0xFF;
                tab[i++] = (int)(0xFF000000u | (r << 16) | (g << 8) | b);
            }
        }
        return tab;
    }

    protected void ReadContents()
    {
        bool done = false;
        while (!done && !Error())
        {
            switch (Read())
            {
                case 44:
                    ReadImage();
                    break;
                case 33:
                    switch (Read())
                    {
                        case 249:
                            ReadGraphicControlExt();
                            break;
                        case 255:
                            {
                                ReadBlock();
                                string app = "";
                                for (int i = 0; i < 11; i++)
                                {
                                    string text = app;
                                    char c = (char)block[i];
                                    app = text + c;
                                }
                                if (app.Equals("NETSCAPE2.0"))
                                {
                                    ReadNetscapeExt();
                                }
                                else
                                {
                                    Skip();
                                }
                                break;
                            }
                        default:
                            Skip();
                            break;
                    }
                    break;
                case 59:
                    done = true;
                    break;
                default:
                    status = STATUS_FORMAT_ERROR;
                    break;
                case 0:
                    break;
            }
        }
    }

    protected void ReadGraphicControlExt()
    {
        Read();
        int packed = Read();
        dispose = (packed & 0x1C) >> 2;
        if (dispose == 0)
        {
            dispose = 1;
        }
        transparency = (packed & 1) != 0;
        delay = ReadShort() * 10;
        transIndex = Read();
        Read();
    }

    protected void ReadHeader()
    {
        string id = "";
        for (int i = 0; i < 6; i++)
        {
            id += (char)Read();
        }
        if (!id.StartsWith("GIF"))
        {
            status = STATUS_FORMAT_ERROR;
            return;
        }
        ReadLSD();
        if (gctFlag && !Error())
        {
            gct = ReadColorTable(gctSize);
            bgColor = gct[bgIndex];
        }
    }

    protected void ReadImage()
    {
        ix = ReadShort();
        iy = ReadShort();
        iw = ReadShort();
        ih = ReadShort();
        int packed = Read();
        lctFlag = (packed & 0x80) != 0;
        interlace = (packed & 0x40) != 0;
        lctSize = 2 << (packed & 7);
        if (lctFlag)
        {
            lct = ReadColorTable(lctSize);
            act = lct;
        }
        else
        {
            act = gct;
            if (bgIndex == transIndex)
            {
                bgColor = 0;
            }
        }
        int save = 0;
        if (transparency)
        {
            save = act[transIndex];
            act[transIndex] = 0;
        }
        if (act == null)
        {
            status = STATUS_FORMAT_ERROR;
        }
        if (Error())
        {
            return;
        }
        DecodeImageData();
        Skip();
        if (!Error())
        {
            frameCount++;
            bitmap = new Bitmap(width, height);
            image = bitmap;
            SetPixels();
            frames.Add(new GifFrame(bitmap, delay));
            if (transparency)
            {
                act[transIndex] = save;
            }
            ResetFrame();
        }
    }

    protected void ReadLSD()
    {
        width = ReadShort();
        height = ReadShort();
        int packed = Read();
        gctFlag = (packed & 0x80) != 0;
        gctSize = 2 << (packed & 7);
        bgIndex = Read();
        pixelAspect = Read();
    }

    protected void ReadNetscapeExt()
    {
        do
        {
            ReadBlock();
            if (block[0] == 1)
            {
                int b1 = block[1] & 0xFF;
                int b2 = block[2] & 0xFF;
                loopCount = (b2 << 8) | b1;
            }
        }
        while (blockSize > 0 && !Error());
    }

    protected int ReadShort()
    {
        return Read() | (Read() << 8);
    }

    protected void ResetFrame()
    {
        lastDispose = dispose;
        lastRect = new Rectangle(ix, iy, iw, ih);
        lastImage = image;
        lastBgColor = bgColor;
        bool transparency = false;
        int delay = 0;
        lct = null;
    }

    protected void Skip()
    {
        do
        {
            ReadBlock();
        }
        while (blockSize > 0 && !Error());
    }
}
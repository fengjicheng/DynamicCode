using Gif.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace ValidyCode.Common
{
    public class ValidateCode
    {
        public MemoryStream GetGifImage()
        {
            AnimatedGifEncoder encoder = new AnimatedGifEncoder();
            MemoryStream stream = new MemoryStream();
            MemoryStream outstream = new MemoryStream();
            string path = System.IO.Path.GetTempPath();
            encoder.Start();
            ///间隔
            encoder.SetDelay(500);
            ///循环多少次后停止
            encoder.SetRepeat(0);

            ValidateCode v = new ValidateCode();

            v.Length = this.length;
            v.FontSize = this.fontSize;
            v.Chaos = this.chaos;
            v.BackgroundColor = this.backgroundColor;
            v.ChaosColor = this.chaosColor;
            v.CodeSerial = this.codeSerial;
            v.Colors = this.colors;
            v.Fonts = this.fonts;
            v.Padding = this.padding;
            string code = v.CreateVerifyCode();
            //画布
            Bitmap bgbitmap = this.CreateImageCode(code);
            ///创建10张循环
            for (int i = 0; i < 5; i++)
            {
                //image = TwistImage(image, true, 0, 0);
                // Bitmap bitmap = TwistImage(DeepCopyBitmap(bgbitmap), true, 0, 0);
                Bitmap bitmap = DrawCode(DeepCopyBitmap(bgbitmap),code);
                bitmap = DrawCurveLine(bitmap);
                bitmap = TwistImage(bitmap, true, 0, 0);
                bitmap.Save(stream, ImageFormat.Png);
                encoder.AddFrame(Image.FromStream(stream));
                stream = new MemoryStream();
            }
            encoder.OutPut(ref stream);
           // encoder.Finish();
            // System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //Bitmap image = DeepCopyBitmap(bgbitmap);
            //image = DrawCurveLine(bgbitmap);
            //image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return stream;
        }
        #region bitmap拷贝
        public  Bitmap DeepCopyBitmap(Bitmap bitmap)

        {
            try
            {

                Bitmap dstBitmap = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, bitmap);
                    ms.Seek(0, SeekOrigin.Begin);
                    dstBitmap = (Bitmap)bf.Deserialize(ms);
                    ms.Close();
                }
                return dstBitmap;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
#endregion
        #region 验证码长度(默认6个验证码的长度)
        int length = 5;
        /// <summary>
        ///  验证码长度(默认6个验证码的长度)
        /// </summary>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        #endregion

        #region 验证码字体大小(为了显示扭曲效果，默认40像素，可以自行修改)
        int fontSize = 40;
        /// <summary>
        /// 验证码字体大小(为了显示扭曲效果，默认40像素，可以自行修改)
        /// </summary>
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }
        #endregion

        #region 边框补(默认1像素)
        int padding = 4;
        /// <summary>
        /// 边框补(默认1像素)
        /// </summary>
        public int Padding
        {
            get { return padding; }
            set { padding = value; }
        }
        #endregion

        #region 是否输出燥点(默认不输出)
        bool chaos = true;
        /// <summary>
        /// 是否输出燥点(默认不输出)
        /// </summary>
        public bool Chaos
        {
            get { return chaos; }
            set { chaos = value; }
        }
        #endregion

        #region 输出燥点的颜色(默认灰色)
        Color chaosColor = Color.LightGray;
        /// <summary>
        /// 输出燥点的颜色(默认灰色)
        /// </summary>
        public Color ChaosColor
        {
            get { return chaosColor; }
            set { chaosColor = value; }
        }
        #endregion

        #region 自定义背景色(默认白色)
        Color backgroundColor = Color.White;
        /// <summary>
        /// 自定义背景色(默认白色)
        /// </summary>
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        #endregion

        #region 自定义随机颜色数组
        Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
        /// <summary>
        /// 自定义随机颜色数组
        /// </summary>
        public Color[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }
        #endregion

        #region 自定义字体数组
        string[] fonts = { "Arial", "Georgia" };
        /// <summary>
        /// 自定义字体数组
        /// </summary>
        public string[] Fonts
        {
            get { return fonts; }
            set { fonts = value; }
        }
        #endregion

        #region 自定义随机码字符串序列(使用逗号分隔)
        string codeSerial = "2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,j,k,m,n,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
        /// <summary>
        /// 自定义随机码字符串序列(使用逗号分隔)
        /// </summary>
        public string CodeSerial
        {
            get { return codeSerial; }
            set { codeSerial = value; }
        }
        #endregion

        #region 产生波形滤镜效果

        private const double PI = 3.1415926535897932384626433832795;
        private const double PI2 = 6.283185307179586476925286766559;

        /// <summary>
        /// 正弦曲线Wave扭曲图片（Edit By 51aspx.com）
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        /// <returns></returns>
        public System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.Green), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    System.Drawing.Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }

            return destBmp;
        }



        #endregion

        #region 生成校验码图片
        public Bitmap CreateImageCode(string code)
        {
            int fSize = FontSize;
            int fWidth = fSize + Padding;

            int imageWidth = (int)(code.Length * fWidth) + 4 + Padding * 2;
            int imageHeight = fSize * 2 + Padding;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageWidth, imageHeight);

            Graphics g = Graphics.FromImage(image);

            g.Clear(BackgroundColor);

            Random rand = new Random();
            Pen pen = new Pen(System.Drawing.Color.Green, 4);


            //给背景添加随机生成的燥点
            if (this.Chaos)
            {

                pen = new Pen(ChaosColor, 30);
                int c = Length * 10;

                for (int i = 0; i < c; i++)
                {
                    int x = rand.Next(image.Width);
                    int y = rand.Next(image.Height);

                    g.DrawRectangle(pen, x, y, 1, 1);
                }
            }
   

           
            //画一个边框 边框颜色为Color.Gainsboro
           // g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, image.Width - 1, image.Height - 1);
            g.Dispose();

            //产生波形（Add By 51aspx.com）
            //image = TwistImage(image, true, 0, 0);

            return image;
        }
        #endregion
        public Bitmap DrawCode(Bitmap bit,string code)
        {
            int fSize = FontSize;
            int fWidth = fSize + Padding;
            int imageWidth = bit.Width;
            int imageHeight = bit.Height;
            Random rand = new Random();
            Pen pen = new Pen(System.Drawing.Color.Green, 4);
            Graphics g = Graphics.FromImage(bit);
            int left = 0, top = 0, top1 = 1, top2 = 1;
            int n1 = (imageHeight - FontSize - Padding * 2);
            int n2 = n1 / 4;
            top1 = n2;
            top2 = n2 * 2;

            Font f;
            Brush b;

            int cindex, findex;

            //随机字体和颜色的验证码字符
            for (int i = 0; i < code.Length; i++)
            {
                cindex = rand.Next(Colors.Length - 1);
                findex = rand.Next(Fonts.Length - 1);

                f = new System.Drawing.Font(Fonts[findex], fSize, System.Drawing.FontStyle.Bold);
                b = new System.Drawing.SolidBrush(Colors[cindex]);

                if (i % 2 == 1)
                {
                    top = top2;
                }
                else
                {
                    top = top1;
                }
                top = rand.Next(top*2);
                left = i * fWidth;
                //设置旋转远点
                //g.TranslateTransform(left+ f.Size/2, top+f.GetHeight()/2);
                float py = rand.Next(-45, 45);//长生随机的偏移量
                //获取旋转后字母
                var img = BuildBitmap(code.Substring(i, 1),f,b,py);
                var point = new Point(left, top);//小图画到大图上的位置
                g.DrawImage(img, point);//把小图画到大图上

                // g.DrawString(code.Substring(i, 1), f, b, left, top);
            }
            g.Dispose();
            return bit;
        }
        /// 生成单个小图
        /// </summary>
        /// <param name="s">数字</param>
        /// <param name="c">数字颜色</param>
        /// <param name="py">旋转偏移量</param>
        /// <returns></returns>
        public System.Drawing.Image BuildBitmap(string s, Font font, Brush b, float py)
        {
            System.Drawing.Bitmap bitmap = new Bitmap(80, 80);
            var g = System.Drawing.Graphics.FromImage(bitmap);
           // g.Clear();
            //设置画板的坐标原点为中点
            g.TranslateTransform(bitmap.Width / 2, bitmap.Height / 2);
            //以指定角度对画板进行旋转
            g.RotateTransform(py);
            var size = g.MeasureString(s, font);
            //让文字变得平滑
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            //把数字画到画板的中点位置
            g.DrawString(s, font,b, (bitmap.Width - size.Width) / 2 - bitmap.Width / 2, (bitmap.Height - size.Height) / 2 - bitmap.Height / 2);
            return bitmap;
        }

        /// <summary>
        /// 画干扰线
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public Bitmap DrawCurveLine(Bitmap bit)
        {
            int imageWidth = bit.Width;
            int imageHeight = bit.Height;
            Random rand = new Random();
            Pen pen = new Pen(System.Drawing.Color.Green, 4);
            Graphics g = Graphics.FromImage(bit);
            //给背景添加随即曲线
            pen = new Pen(System.Drawing.Color.Green, 7);
            //g.DrawLine(pen, new Point(0, imageHeight / 2), new Point(imageWidth, imageHeight / 2));//直线
            // g.DrawEllipse(pen, new Rectangle(new Point(0, imageHeight/ 4),new Size(imageWidth,imageHeight/2)));//椭圆 
            //g.DrawArc(pen, new Rectangle(new Point(0, imageHeight / 3), new Size(imageWidth, imageHeight / 3)), 180, 180);//椭圆弧线
            int colorIndex = rand.Next(Colors.Length - 1);
            Pen CurvePen = new Pen(Colors[colorIndex], 8);
            PointF[] CurvePointF = new PointF[4];

            int randM = rand.Next(3);
            if (randM == 0)
            {
                CurvePointF = new PointF[4];
                CurvePointF[0] = new PointF(0, 0);
                CurvePointF[1] = new PointF(imageWidth / 3, imageHeight);
                CurvePointF[2] = new PointF(2 * imageWidth / 3, 0);
                CurvePointF[3] = new PointF(imageWidth, imageHeight);
                g.DrawCurve(CurvePen, CurvePointF, 0.5f);
                CurvePointF = new PointF[3];
                CurvePointF[0] = new PointF(0, 0);
                CurvePointF[1] = new PointF(imageWidth / 2, imageHeight);
                CurvePointF[2] = new PointF(imageWidth, 0);
                g.DrawCurve(CurvePen, CurvePointF, 0.5f);
            }
            else if (randM == 1)
            {
                CurvePointF = new PointF[3];
                CurvePointF[0] = new PointF(0, 0);
                CurvePointF[1] = new PointF(imageWidth / 2, imageHeight);
                CurvePointF[2] = new PointF(imageWidth, 0);
                g.DrawCurve(CurvePen, CurvePointF, 0.5f);
            }
            else
            {
                CurvePointF = new PointF[4];
                CurvePointF[0] = new PointF(0, 0);
                CurvePointF[1] = new PointF(imageWidth / 3, imageHeight);
                CurvePointF[2] = new PointF(2 * imageWidth / 3, 0);
                CurvePointF[3] = new PointF(imageWidth, imageHeight);
                g.DrawCurve(CurvePen, CurvePointF, 0.5f);
            }
            //画一个边框 边框颜色为Color.Gainsboro
            g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, bit.Width - 1, bit.Height - 1);
            g.Dispose();
            return bit;
        }
        #region 将创建好的图片输出到页面
        public void CreateImageOnPage(string code, HttpContext context)
        {
            ///禁用缓存
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = DateTime.Now - new TimeSpan(1, 0, 0);
            context.Response.Expires = 0;
            context.Response.CacheControl = "no-cache";

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Bitmap image = this.CreateImageCode(code);

            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            context.Response.ClearContent();
            context.Response.ContentType = "image/Jpeg";
            context.Response.BinaryWrite(ms.GetBuffer());

            ms.Close();
            ms = null;
            image.Dispose();
            image = null;
        }
        #endregion

        #region 生成随机字符码
        public string CreateVerifyCode(int codeLen)
        {
            if (codeLen == 0)
            {
                codeLen = Length;
            }

            string[] arr = CodeSerial.Split(',');

            string code = "";

            int randValue = -1;

            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));

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
        #endregion
    }
}
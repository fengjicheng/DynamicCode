using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ValidyCode.Common;

namespace ValidyCode
{
    public partial class Demo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidateCode validateCode = new ValidateCode();
            MemoryStream memoryStream = validateCode.GetGifImage();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ContentType = "image/Gif";
            HttpContext.Current.Response.BinaryWrite(memoryStream.ToArray());
            memoryStream.Close();
            memoryStream.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OKHOSTING.ORM.UI.Web.Forms.Private.UserControls
{
	/// <summary>
	/// Summary description for ForeignKeyPickerAutoComplete
	/// </summary>
	public class ForeignKeyPickerAutoComplete : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			context.Response.Write("Hello World");
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}
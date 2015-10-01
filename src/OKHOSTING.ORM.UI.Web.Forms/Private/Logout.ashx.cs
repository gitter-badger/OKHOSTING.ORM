using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OKHOSTING.ORM.UI.Web.Forms.Private
{
	/// <summary>
	/// Summary description for Logout
	/// </summary>
	public class Logout : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			System.Web.Security.FormsAuthentication.SignOut();
			context.Response.Redirect("/login.aspx");
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
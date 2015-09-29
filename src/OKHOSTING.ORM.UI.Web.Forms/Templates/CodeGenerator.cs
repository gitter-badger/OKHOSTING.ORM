using System.Collections.Generic;
using System.IO;

namespace OKHOSTING.Sql.ORM.UI.Web.Forms.Templates
{
	public static class CodeGenerator
	{
		public static void Generate(IEnumerable<DataType> dtypes)
		{
			var session = new Dictionary<string, object>();
			string outputDirectory = Path.Combine(OKHOSTING.Core.DefaultPaths.Base, "Private");

			foreach (DataType dtype in dtypes)
			{
				session["dtype"] = dtype;
				string directoryPath = Path.Combine(outputDirectory, dtype.InnerType.Name);

				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
                }
				
				//Delete
				var deleteAspx = new Delete.aspx();
				deleteAspx.Session = session;
				deleteAspx.Initialize();
                File.WriteAllText(Path.Combine(directoryPath, "Delete.aspx"), deleteAspx.TransformText());

				var deleteAspxCs = new Delete.aspx_cs();
				deleteAspxCs.Session = session;
				deleteAspxCs.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "Delete.aspx.cs"), deleteAspxCs.TransformText());

				var deleteAspxDesignerCs = new Delete.aspx_designer_cs();
				deleteAspxDesignerCs.Session = session;
				deleteAspxDesignerCs.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "Delete.aspx.designer.cs"), deleteAspxDesignerCs.TransformText());

				//Detail
				var detailAspx = new Detail.aspx();
				detailAspx.Session = session;
				detailAspx.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "Detail.aspx"), detailAspx.TransformText());

				var detailAspxCs = new Detail.aspx_cs();
				detailAspxCs.Session = session;
				detailAspxCs.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "Detail.aspx.cs"), detailAspxCs.TransformText());

				var detailAspxDesignerCs = new Detail.aspx_designer_cs();
				detailAspxDesignerCs.Session = session;
				detailAspxDesignerCs.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "Detail.aspx.designer.cs"), detailAspxDesignerCs.TransformText());

				//Edit
				var editAspx = new Edit.aspx();
				editAspx.Session = session;
				editAspx.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "Edit.aspx"), editAspx.TransformText());

				var editAspxCs = new Edit.aspx_cs();
				editAspxCs.Session = session;
				editAspxCs.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "Edit.aspx.cs"), editAspxCs.TransformText());

				var editAspxDesignerCs = new Edit.aspx_designer_cs();
				editAspxDesignerCs.Session = session;
				editAspxDesignerCs.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "Edit.aspx.designer.cs"), editAspxDesignerCs.TransformText());

				//List
				var listAspx = new List.aspx();
				listAspx.Session = session;
				listAspx.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "List.aspx"), listAspx.TransformText());

				var listAspxCs = new List.aspx_cs();
				listAspxCs.Session = session;
				listAspxCs.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "List.aspx.cs"), listAspxCs.TransformText());

				var listAspxDesignerCs = new List.aspx_designer_cs();
				listAspxDesignerCs.Session = session;
				listAspxDesignerCs.Initialize();
				File.WriteAllText(Path.Combine(directoryPath, "List.aspx.designer.cs"), listAspxDesignerCs.TransformText());
			}
		}
    }
}
using OKHOSTING.UI;
using OKHOSTING.UI.Controls;
using OKHOSTING.UI.Controls.Layouts;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using OKHOSTING.ORM.Operations;

namespace OKHOSTING.ORM.UI
{
	/// <summary>
	/// Creates a grid that shows a collection of persistent objects
	/// </summary>
	public class ObjectGrid
	{
		/// <summary>
		/// Select operation that will be used to populate this grid
		/// </summary>
		public Select DataSource { get; set; }

		/// <summary>
		/// Number of elements to show per page
		/// </summary>
		public int PageSize { get; set; } = 20;

		/// <summary>
		/// Zero based index of the currently displayed page
		/// </summary>
		public int CurrentPage { get; set; }

		/// <summary>
		/// A protected list of columns that should be displayed, that is calculated from the DataSource.Members list
		/// </summary>
		protected IEnumerable<MemberInfo> Columns { get; set; }

		/// <summary>
		/// Constructs the grid and populates it with data
		/// </summary>
		public IGrid CreateGrid()
		{
			var db = DataBase.CreateDataBase();
			int count = (int) db.Count(DataSource);
			IGrid grid = Platform.Current.Create<IGrid>();
			Columns = DataSource.DataType.GetMembers(DataSource.Members.Select(sm => sm.DataMember));

			grid.ColumnCount = DataSource.Members.Count; //one column per member
			grid.RowCount = count > PageSize? count + 1 : PageSize + 1; //all records + header

			//create header row
			int column = 0;

			foreach (MemberInfo member in Columns)
			{
				ILabelButton header = Platform.Current.Create<ILabelButton>();
				header.Text = Translator.Translate(member);
				header.Click += Header_Click;

				grid.SetContent(0, column, header);
				column++;
			}

			//create data rows
			int row = 1;
			foreach (object instance in DataSource)
			{
				column = 0;

				foreach (MemberInfo member in Members)
				{
					ILabel content = Platform.Current.Create<ILabel>();
					content.Text = Data.Validation.MemberExpression.GetValue(member, instance).ToString();

					grid.SetContent(row, column, content);
				}

				row++;
			}

			db.Dispose();

			return grid;
		}

		private void Header_Click(object sender, EventArgs e)
		{
			MemberInfo member = Columns.Where(mi => ((ILabelButton) sender).Text == Translator.Translate(mi)).Single();
			DataMember dmember = null;

			//is this an atomic value?
			if (DataSource.Members.Where(dm => dm.DataMember.Member.FinalMemberInfo == member).Count() == 1)
			{
				dmember = DataSource.DataType[member.Name];
			}
			//this is a foreign key
			else if (DataSource.DataType.IsForeignKey(member))
			{
			}

			//DataSource.OrderBy.Add(new OrderBy())
		}
	}
}
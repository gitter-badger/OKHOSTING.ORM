using OKHOSTING.ORM.Operations;
using OKHOSTING.ORM.UI.Forms;
using OKHOSTING.UI;
using OKHOSTING.UI.Controls;
using OKHOSTING.UI.Controls.Layouts;
using System;

namespace OKHOSTING.ORM.UI
{
	public class SelectController : Controller
	{
		/// <summary>
		/// <summary>
		/// The type of object that will be inserted
		/// </summary>
		public readonly Select DataSource;

		/// <summary>
		/// The actual form that we will use to populate the object from user input
		/// </summary>
		protected ObjectGrid Grid;

		public SelectController(Select dataSource)
		{
			if (dataSource == null)
			{
				throw new ArgumentNullException(nameof(dataSource));
			}

			DataSource = dataSource;
		}

		public override void Start()
		{
			base.Start();
			Refresh();
		}

		public override void Refresh()
		{
			base.Refresh();

			//rebuild data grid
			
			Grid = new ObjectGrid();
			Grid.DataSource = DataSource;
			Grid.DataBind();

			//add New button
			IButton newRecord = Platform.Current.Create<IButton>();
			newRecord.Text = Resources.Strings.OKHOSTING_ORM_UI_SelectController_New;
			newRecord.Click += NewRecord_Click;

			//create our own grid
			IGrid container = Platform.Current.Create<IGrid>();
			container.ColumnCount = 1;
			container.RowCount = 2;

			container.SetContent(0, 0, Grid.Content);
			container.SetContent(1, 0, newRecord);

			Platform.Current.Page.Title = Resources.Strings.OKHOSTING_ORM_UI_SelectController_List + ' ' + Translator.Translate(DataSource.DataType.InnerType);
			Platform.Current.Page.Content = container;
		}

		private void NewRecord_Click(object sender, EventArgs e)
		{
			new InsertController(DataSource.DataType).Start();
		}
	}
}
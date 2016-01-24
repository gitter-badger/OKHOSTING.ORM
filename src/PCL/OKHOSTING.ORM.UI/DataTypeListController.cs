using OKHOSTING.ORM.Operations;
using OKHOSTING.UI;
using OKHOSTING.UI.Controls;
using OKHOSTING.UI.Controls.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OKHOSTING.ORM.UI
{
	/// <summary>
	/// Shows a list with all available datatypes to the user, and links them to a SelectController
	/// </summary>
	public class DataTypeListController: Controller
	{
		public override void Start()
		{
			base.Start();

			IStack list = Platform.Current.Create<IStack>();

			foreach (DataType dtype in DataType.AllDataTypes)
			{
				ILabelButton link = Platform.Current.Create<ILabelButton>();
				link.Text = Translator.Translate(dtype.InnerType);
				link.Click += Link_Click;

				list.Children.Add(link);
			}

			Platform.Current.Page.Title = Resources.Strings.OKHOSTING_ORM_UI_DataTypeListController_Title;
			Platform.Current.Page.Content = list;
		}

		private void Link_Click(object sender, EventArgs e)
		{
			ILabelButton clicked = (ILabelButton) sender;
			DataType selected = DataType.AllDataTypes.Where(dt => Translator.Translate(dt.InnerType) == clicked.Text).Single();

			Select select = new Select();
			select.DataType = selected;
			select.Limit = new SelectLimit(0, 20);

			new SelectController(select).Start();
		}
	}
}
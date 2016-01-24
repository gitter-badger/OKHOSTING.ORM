using OKHOSTING.ORM.UI.Forms;
using OKHOSTING.UI;
using OKHOSTING.UI.Controls;
using OKHOSTING.UI.Controls.Layouts;
using System;

namespace OKHOSTING.ORM.UI
{
	public class InsertController: Controller
	{
		/// <summary>
		/// The type of object that will be inserted
		/// </summary>
		public readonly DataType DataType;

		/// <summary>
		/// The object that will be populated with data and then inserted
		/// </summary>
		protected readonly object Instance;

		/// <summary>
		/// The actual form that we will use to populate the object from user input
		/// </summary>
		protected readonly ObjectForm Form;

		public InsertController(DataType dataType)
		{
			if (dataType == null)
			{
				throw new ArgumentNullException(nameof(dataType));
			}

			DataType = dataType;
			Instance = Activator.CreateInstance(DataType.InnerType);
			Form = new ObjectForm();
		}

		public override void Start()
		{
			base.Start();

			//add fields and populate values
			//Form.AddFieldsFrom(Instance);

			DataType dtype = Instance.GetType();

			//create fields
			foreach (System.Reflection.MemberInfo member in dtype.AllMemberInfos)
			{
				OKHOSTING.UI.Controls.Forms.FormField field = Form.AddField(member);
				field.Value = Data.Validation.MemberExpression.GetValue(member, Instance);
			}

			//actually create the form
			Form.DataBind();

			//add Save button
			IButton save = Platform.Current.Create<IButton>();
			save.Text = Resources.Strings.OKHOSTING_ORM_UI_InsertController_Save;
			save.Click += Save_Click;

			//add Cancel button
			IButton cancel = Platform.Current.Create<IButton>();
			cancel.Text = Resources.Strings.OKHOSTING_ORM_UI_InsertController_Cancel;
			cancel.Click += Cancel_Click;

			//create our own grid
			IGrid grid = Platform.Current.Create<IGrid>();
			grid.ColumnCount = 2;
			grid.RowCount = 2;

			grid.SetContent(0, 0, Form.Content);
			grid.SetColumnSpan(2, Form.Content);

			grid.SetContent(1, 0, save);
			grid.SetContent(1, 1, cancel);

			Platform.Current.Page.Title = Resources.Strings.OKHOSTING_ORM_UI_InsertController_New + ' ' + Translator.Translate(DataType.InnerType);
			Platform.Current.Page.Content = grid;
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			Finish();
		}

		private void Save_Click(object sender, EventArgs e)
		{
			Form.CopyValuesTo(Instance);

			using (var db = DataBase.CreateDataBase())
			{
				db.Insert(Instance);
			}

			Finish();
		}
	}
}
using OKHOSTING.ORM.UI.Forms;
using OKHOSTING.UI;
using OKHOSTING.UI.Controls;
using OKHOSTING.UI.Controls.Layouts;
using System;

namespace OKHOSTING.ORM.UI
{
	public class DeleteController : Controller
	{
		/// <summary>
		/// The object that will be populated with data and then inserted
		/// </summary>
		public readonly object Instance;

		/// <summary>
		/// The type of object that will be inserted
		/// </summary>
		public readonly DataType DataType;

		protected ICheckBox ChkConfirm { get; set; }
		protected ILabel LblConfirm { get; set; }

		public DeleteController(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			Instance = instance;
			DataType = instance.GetType();
		}

		public override void Start()
		{
			base.Start();

			//add label with instance name
			ILabel lblInstanceName = Platform.Current.Create<ILabel>();
			lblInstanceName.Text = Instance.ToString();

			//add confirmation checkbox
			LblConfirm = Platform.Current.Create<ILabel>();
			LblConfirm.Text = Resources.Strings.OKHOSTING_ORM_UI_DeleteController_Confirm;

			ChkConfirm = Platform.Current.Create<ICheckBox>();

			//add delete button
			IButton delete = Platform.Current.Create<IButton>();
			delete.Text = Resources.Strings.OKHOSTING_ORM_UI_UpdateController_Save;
			delete.Click += Delete_Click; ;

			//add Cancel button
			IButton cancel = Platform.Current.Create<IButton>();
			cancel.Text = Resources.Strings.OKHOSTING_ORM_UI_UpdateController_Cancel;
			cancel.Click += Cancel_Click;

			//create our own grid
			IGrid grid = Platform.Current.Create<IGrid>();
			grid.ColumnCount = 2;
			grid.RowCount = 3;

			grid.SetContent(0, 0, lblInstanceName);
			grid.SetColumnSpan(2, lblInstanceName);

			grid.SetContent(1, 0, LblConfirm);
			grid.SetContent(1, 1, ChkConfirm);

			grid.SetContent(2, 0, delete);
			grid.SetContent(2, 1, cancel);

			Platform.Current.Page.Title = Resources.Strings.OKHOSTING_ORM_UI_DeleteController_Delete + ' ' + Translator.Translate(DataType.InnerType);
			Platform.Current.Page.Content = grid;
		}

		private void Delete_Click(object sender, EventArgs e)
		{
			if (ChkConfirm.Value)
			{
				using (var db = DataBase.CreateDataBase())
				{
					db.Delete(Instance);
				}

				Finish();
			}
			else
			{
				LblConfirm.FontColor = new Color(255, 255, 0, 0);
			}
		}

		private void Cancel_Click(object sender, EventArgs e)
		{
			Finish();
		}
	}
}
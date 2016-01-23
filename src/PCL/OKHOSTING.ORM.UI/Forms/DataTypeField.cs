using System;
using System.Linq;
using OKHOSTING.UI.Controls.Forms;

namespace OKHOSTING.ORM.UI.Forms
{
	/// <summary>
	/// Field for selecting a DataType
	/// </summary>
	public class DataTypeField : ListPickerField
	{
		public override Type ValueType
		{
			get
			{
				return typeof(DataType);
			}
		}

		public override object Value
		{
			get
			{
				if (ValueControl.Value == OKHOSTING.UI.Resources.Strings.OKHOSTING_UI_Controls_Forms_EmptyValue)
				{
					return null;
				}
				else
				{
					return DataType.GetMap(Type.GetType(ValueControl.Value));
				}
			}
			set
			{
				if (value == null && !Required)
				{
					ValueControl.Value = OKHOSTING.UI.Resources.Strings.OKHOSTING_UI_Controls_Forms_EmptyValue;
				}
				else
				{
					ValueControl.Value = ((DataType) value).InnerType.FullName;
				}
			}
		}

		/// <summary>
		/// Creates the controls for displaying the field
		/// </summary>
		protected override void CreateValueControl()
		{
			//create listpicker and add empty value if not required
			base.CreateValueControl();

			//Create an item for each loaded DataType
			foreach (DataType dtype in DataType.AllDataTypes.OrderBy(dt => dt.FullName))
			{
				ValueControl.Items.Add(dtype.InnerType.FullName);
			}
		}
	}
}
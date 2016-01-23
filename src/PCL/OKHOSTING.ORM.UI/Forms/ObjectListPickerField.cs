using OKHOSTING.UI.Controls.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OKHOSTING.ORM.UI.Forms
{
	/// <summary>
	/// A DropDownList for selecting DataObjects
	/// </summary>
	public class ObjectListPickerField : ListPickerField
	{
		/// <summary>
		/// The type of object that will be selected on this field
		/// </summary>
		public readonly DataType DataType;

		/// <summary>
		/// Primary keys will be stored here in the same order as the Items, so you can get
		/// the selected object by using the ISelectPicker.SelectedIndex property and not rely on parsing a ISelectPicker.Value as a string
		/// </summary>
		protected readonly List<object> PrimaryKeys = new List<object>();

		/// <summary>
		/// Cache of value, so we dont read it from database more than necessary
		/// </summary>
		protected object ValueCache;

		public ObjectListPickerField(DataType dtype)
		{
			if (dtype == null)
			{
				throw new ArgumentNullException(nameof(dtype));
			}

			DataType = dtype;
		}

		public override object Value
		{
			get
			{
				if (ValueCache == null)
				{
					using (DataBase db = DataBase.CreateDataBase())
					{
						if (DataType.PrimaryKey.Count() == 1)
						{
							ValueCache = db.SelectById(DataType, (IComparable)PrimaryKeys[ValueControl.SelectedIndex]);
						}
						else
						{
							ValueCache = db.SelectById(DataType, (IComparable[])PrimaryKeys[ValueControl.SelectedIndex]);
						}
					}
				}

				return ValueCache;
			}
			set
			{
				ValueCache = value; //cache the object itself
				ValueControl.Value = value.ToString();
			}
		}

		public override Type ValueType
		{
			get
			{
				return DataType.InnerType;
			}
		}

		/// <summary>
		/// Creates the controls for displaying the field
		/// </summary>
		protected override void CreateValueControl()
		{
			base.CreateValueControl();
			ValueControl.ValueChanged += ValueControl_ValueChanged;
			PrimaryKeys.Clear();

			using (DataBase db = DataBase.CreateDataBase())
			{
				Operations.Select select = new Operations.Select();
				select.DataType = DataType;
				var primaryKey = DataType.PrimaryKey.ToArray();

				foreach (object instance in db.Select(select))
				{
					//add item as a string
					ValueControl.Items.Add(instance.ToString());

					//single column primary key? add as a single object
					if (primaryKey.Length == 1)
					{
						PrimaryKeys.Add(primaryKey.Single().Member.GetValue(instance));
					}
					//multiple column primary key? add as an array of objects
					else
					{
						//add primary key
						IComparable[] pkValues = new IComparable[primaryKey.Length];

						for (int i = 0; i < primaryKey.Length; i++)
						{
							pkValues[i] = (IComparable) primaryKey[i].Member.GetValue(instance);
						}

						PrimaryKeys.Add(pkValues);
					}
				}
			}
		}

		private void ValueControl_ValueChanged(object sender, string e)
		{
			ValueCache = null; //empty cache so we re-read the value from DB
		}
	}
}
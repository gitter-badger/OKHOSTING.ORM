using static OKHOSTING.Core.TypeExtensions;
using OKHOSTING.ORM.Filters;
using OKHOSTING.UI.Controls.Forms;
using System;
using System.Collections.Generic;

namespace OKHOSTING.ORM.UI
{
	/// <summary>
	/// A form that enables the user to create advanced filters when searching for DataObjects
	/// </summary>
	public class FilterDataForm : Form
	{
		/// <summary>
		/// Copies all values entered by the user to a DavaValueInstance collection
		/// </summary>
		/// <param name="members">Collection where values will be copied to</param>
		public IEnumerable<Filter> GetFiltersFrom(IEnumerable<DataMember> members)
		{
			//validate arguments
			if (members == null) throw new ArgumentNullException("members");

			//filters
			Filter filter = null;

			//create filters
			foreach (DataMember dvalue in members)
			{
				#region range filter

				//if it's a datetime or a numeric field, create range filter
				if (dvalue.ValueType.Equals(typeof(DateTime)) || dvalue.ValueType.IsNumeric())
				{
					//realted fields
					FormField fieldMin = null, fieldMax = null;

					//search corresponding field for this DataValueInstance
					foreach (FormField f in Fields)
					{
						if (f.Id == dvalue.Name + "_0") fieldMin = f;
						if (f.Id == dvalue.Name + "_1") fieldMax = f;
					}

					//if no controls where found, continue
					if (fieldMin == null && fieldMax == null) continue;

					//if no value was set, continue to the next field
					if (NullValues.IsNull(fieldMin.Value) && NullValues.IsNull(fieldMax.Value)) continue;

					//if both values are set, create range filter
					if (!NullValues.IsNull(fieldMin.Value) && !NullValues.IsNull(fieldMax.Value))
					{
						filter = new RangeFilter(dvalue, (IComparable)fieldMin.Value, (IComparable)fieldMax.Value);
					}

					//only min value is defined
					else if (!NullValues.IsNull(fieldMin.Value))
					{
						filter = new ValueCompareFilter(dvalue, (IComparable)fieldMin.Value, CompareOperator.GreaterThanEqual);
					}

					//only max value is defined
					else if (!NullValues.IsNull(fieldMax.Value))
					{
						filter = new ValueCompareFilter(dvalue, (IComparable)fieldMax.Value, CompareOperator.LessThanEqual);
					}
				}

				#endregion

				#region single value filter

				//create single value filter
				else
				{
					//realted fields
					FormField field = null;

					//search corresponding field for this DataValueInstance
					foreach (FormField f in Fields)
					{
						if (f.Id == dvalue.Name) field = f;
					}

					//if field not found, continue
					if (field == null) continue;

					//if no value was set, continue to the next field
					if (NullValues.IsNull(field.Value)) continue;

					//if its a string, make a LIKE filter
					else if (field.ValueType.Equals(typeof(string)))
					{
						filter = new LikeFilter(dvalue, "%" + field.Value + "%");
					}

					//if it's a dataobject, create a foreign key filter
					else if (DataType.IsDataObject(field.ValueType))
					{
						filter = new ForeignKeyFilter(dvalue, (DataObject)field.Value);
					}

					//otherwise create a compare filter
					else
					{
						filter = new ValueCompareFilter(dvalue, (IComparable)field.Value, CompareOperator.Equal);
					}
				}

				#endregion

				//if filter is not null, add to filter collection and continue
				if (filter != null) filters.Add(filter);
				filter = null;
			}

			//return generated filters
			return filters;
		}

		/// <summary>
		/// Creates a field for a DataMember
		/// </summary>
		public void AddFieldsFrom(DataMember member)
		{
			//if there's no values defined, exit
			if (member == null) throw new ArgumentNullException(nameof(member));

			//field
			FormField fieldMin, fieldMax;

			//DateTime and numeric, create range fields
			if (member.Member.ReturnType.Equals(typeof(DateTime)) || member.Member.ReturnType.IsNumeric())
			{
				//create fields
				fieldMin = ObjectForm.CreateFieldFrom(member.Member.ReturnType);
				fieldMax = ObjectForm.CreateFieldFrom(member.Member.ReturnType);

				//set id
				fieldMin.Name += "_0";
				fieldMax.Name += "_1";

				//labels
				fieldMin.CaptionControl.Text += OKHOSTING.UI.Translator.Translate(this.GetType(), this.GetType().GetFriendlyFullName() + "_Min");
				fieldMax.CaptionControl.Text += OKHOSTING.UI.Translator.Translate(this.GetType(), "OKHOSTING.ORM.UI.FilterDataForm.Max"] + ")";

				//set container
				fieldMin.Container = this;
				fieldMax.Container = this;

				//not required
				fieldMin.Required = false;
				fieldMax.Required = false;

				//set to false always
				fieldMin.TableWide = false;
				fieldMax.TableWide = false;

				//order
				if (member.Column.IsPrimaryKey)
				{
					fieldMin.SortOrder = fieldMax.SortOrder = 0;
				}
				else
				{
					fieldMin.SortOrder = fieldMax.SortOrder = 1;
				}

				//add
				Fields.Add(fieldMin);
				Fields.Add(fieldMax);
			}
			//single value fields
			else
			{
				//create field
				fieldMin = ObjectForm.CreateFieldFrom(member.Member.ReturnType);

				//enable all fields
				fieldMin.Enabled = true;
				
				//set container
				fieldMin.Container = this;

				//not required
				fieldMin.Required = false;

				//set to false always
				fieldMin.TableWide = false;

				//order
				if (member.IsPrimaryKey) fieldMin.SortOrder = 0;
				else fieldMin.SortOrder = 1;

				//add
				Fields.Add(fieldMin);
			}
		}
		
		/// <summary>
		/// Returns the fields that corresponds to this DataMember
		/// </summary>
		public List<FormField> GetFieldsFor(DataMember dvalue)
		{
			//found fields
			FormField fieldMin = null, fieldMax = null;

			//if there's no values defined, exit
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			if (dvalue.ValueType.Equals(typeof(DateTime)) || dvalue.ValueType.IsNumeric())
			{
				//search corresponding field for this DataMember
				foreach (FormField f in Fields)
				{
					if (f.Id == dvalue.Name + "_0") fieldMin = f;
					if (f.Id == dvalue.Name + "_1") fieldMax = f;
				}
				
				if (fieldMin != null && fieldMax != null) return new List<FormField>() { fieldMin, fieldMax };
			}
			else
			{
				//search corresponding field for this DataMember
				foreach (FormField f in Fields)
				{
					if (f.Id == dvalue.Name) return new List<FormField>() { f };
				}
			}

			//nothing was found
			return null;
		}
	}
}

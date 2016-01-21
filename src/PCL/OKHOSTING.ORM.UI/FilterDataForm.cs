using OKHOSTING.Data;
using OKHOSTING.Data.Validation;
using OKHOSTING.ORM.Filters;
using OKHOSTING.UI.Controls.Forms;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using static OKHOSTING.Core.TypeExtensions;

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
		public Filter GetFilter(MemberInfo member)
		{
			//validate arguments
			if (member == null) throw new ArgumentNullException(nameof(member));

			//filters
			Filter filter = null;

			DataType dtype = member.DeclaringType;
			Type returnType = MemberExpression.GetReturnType(member);

			//is this a single value member? (not a foreign key?)
			if (dtype.IsMapped(member.Name))
			{
				DataMember dmember = dtype[member.Name];

				#region Range filter

				//if it's a datetime or a numeric field, create range filter
				if (returnType.Equals(typeof(DateTime)) || returnType.IsNumeric())
				{
					//realted fields
					FormField fieldMin = null, fieldMax = null;

					//search corresponding field for this DataValueInstance
					foreach (FormField f in Fields)
					{
						if (f.Name == member.Name + "_0") fieldMin = f;
						if (f.Name == member.Name + "_1") fieldMax = f;
					}

					//if no controls where found, return no filter
					if (fieldMin == null && fieldMax == null)
					{
						return null;
					}

					//if no value was set, return no filter
					if (!RequiredValidator.HasValue(fieldMin.Value) && !RequiredValidator.HasValue(fieldMax.Value))
					{
						return null;
					}

					//if both values are set, create range filter
					if (RequiredValidator.HasValue(fieldMin.Value) && RequiredValidator.HasValue(fieldMax.Value))
					{
						filter = new RangeFilter(dmember, (IComparable) fieldMin.Value, (IComparable) fieldMax.Value);
					}

					//only min value is defined
					else if (RequiredValidator.HasValue(fieldMin.Value))
					{
						filter = new ValueCompareFilter(dmember, (IComparable) fieldMin.Value, CompareOperator.GreaterThanEqual);
					}

					//only max value is defined
					else if (RequiredValidator.HasValue(fieldMax.Value))
					{
						filter = new ValueCompareFilter(dmember, (IComparable) fieldMax.Value, CompareOperator.LessThanEqual);
					}
				}

				#endregion

				#region Single value filter

				//create single value filter
				else
				{
					//realted fields
					FormField field = Fields.Where(f => f.Name == member.Name).SingleOrDefault();

					//if field not found, return no filter
					if (field == null)
					{
						return null;
					}

					//if no value was set, return no filter
					if (!RequiredValidator.HasValue(field.Value))
					{
						return null;
					}

					//if its a string, make a LIKE filter
					else if (field.ValueType.Equals(typeof(string)))
					{
						filter = new LikeFilter(dmember, "%" + field.Value + "%");
					}

					//otherwise create a compare filter
					else
					{
						filter = new ValueCompareFilter(dmember, (IComparable) field.Value, CompareOperator.Equal);
					}
				}

				#endregion
			}
			//this is a foreign key
			else if (DataType.IsMapped(returnType) && dtype.IsForeignKey(member))
			{
				//realted fields
				FormField field = Fields.Where(f => f.Name == member.Name).SingleOrDefault();
				
				//if field not found, return no filter
				if (field == null)
				{
					return null;
				}

				//if no value was set, return no filter
				if (!RequiredValidator.HasValue(field.Value))
				{
					return null;
				}

				filter = new ForeignKeyFilter(dtype, member, field.Value);
			}

			return filter;
		}

		/// <summary>
		/// Creates a field for a DataMember
		/// </summary>
		public void AddFields(MemberInfo member)
		{
			//if there's no values defined, exit
			if (member == null) throw new ArgumentNullException(nameof(member));

			//field
			FormField fieldMin, fieldMax;
			Type returnType = MemberExpression.GetReturnType(member);

			//DateTime and numeric, create range fields
			if (returnType.Equals(typeof(DateTime)) || returnType.IsNumeric())
			{
				//create fields
				fieldMin = ObjectForm.CreateField(returnType);
				fieldMax = ObjectForm.CreateField(returnType);

				//set id
				fieldMin.Name += "_0";
				fieldMax.Name += "_1";

				//labels
				fieldMin.CaptionControl.Text += Resources.Strings.OKHOSTING_ORM_UI_FilterDataForm_Min;
				fieldMax.CaptionControl.Text += Resources.Strings.OKHOSTING_ORM_UI_FilterDataForm_Max;

				//set container
				fieldMin.Container = this;
				fieldMax.Container = this;

				//not required
				fieldMin.Required = false;
				fieldMax.Required = false;

				//set to false always
				fieldMin.TableWide = false;
				fieldMax.TableWide = false;

				//add
				Fields.Add(fieldMin);
				Fields.Add(fieldMax);
			}
			//single value fields
			else
			{
				//create field
				fieldMin = ObjectForm.CreateField(returnType);

				//set container
				fieldMin.Container = this;

				//not required
				fieldMin.Required = false;

				//set to false always
				fieldMin.TableWide = false;

				//add
				Fields.Add(fieldMin);
			}
		}
		
		/// <summary>
		/// Returns the fields that corresponds to this DataMember
		/// </summary>
		public List<FormField> GetFields(MemberInfo member)
		{
			//found fields
			FormField fieldMin = null, fieldMax = null;
			Type returnType = MemberExpression.GetReturnType(member);

			//if there's no values defined, exit
			if (member == null) throw new ArgumentNullException("dvalue");

			if (returnType.Equals(typeof(DateTime)) || returnType.IsNumeric())
			{
				//search corresponding field for this DataMember
				foreach (FormField f in Fields)
				{
					if (f.Name == member.Name + "_0") fieldMin = f;
					if (f.Name == member.Name + "_1") fieldMax = f;
				}
				
				if (fieldMin != null && fieldMax != null) return new List<FormField>() { fieldMin, fieldMax };
			}
			else
			{
				//search corresponding field for this DataMember
				foreach (FormField f in Fields)
				{
					if (f.Name == member.Name) return new List<FormField>() { f };
				}
			}

			//nothing was found
			return null;
		}
	}
}

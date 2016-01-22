using OKHOSTING.Data.Validation;
using OKHOSTING.UI;
using OKHOSTING.UI.Controls.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OKHOSTING.ORM.UI
{
	/// <summary>
	/// A dataForm that creates fields automatically for a DataMember conllection
	/// </summary>
	public class ObjectForm: Form
	{
		/// <summary>
		/// Creates a field for a DataMember
		/// </summary>
		public FormField AddField(MemberInfo member)
		{
			//if there's no values defined, exit
			if (member == null) throw new ArgumentNullException(nameof(member));

			//field
			FormField field;
			Type returnType = MemberExpression.GetReturnType(member);

			//String
			if (returnType.Equals(typeof(string)))
			{
				field = new StringField();

				//set max lenght, if defined
				int maxLenght = (int) StringLengthValidator.GetMaxLength(member);

				if (maxLenght == 0)
				{
					field.TableWide = true;
				}
				else
				{
					((StringField) field).MaxLenght = maxLenght;
				}

				//set regular expression validation, if defined
				var regex = member.CustomAttributes.Where(att => att.AttributeType.Equals(typeof(RegexValidator))).SingleOrDefault();

				if (regex != null)
				{
					((StringField) field).RegularExpression = (string) regex.ConstructorArguments[0].Value;
				}
			}

			//DataType
			else if (returnType.Equals(typeof(DataType)))
			{
				field = new DataTypeField();
			}

			//otherwise delegate to the static method to create the field from the return type
			else
			{
				field = CreateField(returnType);
			}

			field.Name = member.Name;
			field.Container = this;
			field.Required = RequiredValidator.IsRequired(member);
			field.CaptionControl.Text = Translator.Translate(member);
			//if (member.Column.IsPrimaryKey) field.SortOrder = 0;

			//add to fields collection
			Fields.Add(field);

			return field;
		}

		/// <summary>
		/// Returns the field that corresponds to this DataMember
		/// </summary>
		public FormField GetField(MemberInfo member)
		{
			//if there's no values defined, exit
			if (member == null) throw new ArgumentNullException(nameof(member));

			//search corresponding field for this DataMember
			return Fields.Where(f => f.Name == member.Name).SingleOrDefault();
		}

		/// <summary>
		/// Copies all values entered by the user to a DavaValueInstance collection
		/// </summary>
		/// <param name="dvalues">Collection where values will be copied to</param>
		public void CopyValuesTo(object instance)
		{
			//validate arguments
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			DataType dtype = instance.GetType();

			//create fields
			foreach (MemberInfo member in dtype.AllMemberInfos)
			{
				//search corresponding field for this DataValueInstance
				FormField field = GetField(member);

				if (field != null)
				{
					//assing value
					MemberExpression.SetValue(member, instance, field.Value);
				}
			}
		}

		/// <summary>
		/// Adds fields for every Member that is mapped on a persistent object
		/// </summary>
		/// <param name="instance">Object which values will be copied to the form</param>
		public IEnumerable<FormField> AddFields(object instance)
		{
			//validate arguments
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			DataType dtype = instance.GetType();

			//create fields
			foreach (MemberInfo member in dtype.AllMemberInfos)
			{
				FormField field = AddField(member);
				field.Value = MemberExpression.GetValue(member, instance);

				yield return field;
			}
		}

		#region Static methods

		/// <summary>
		/// Creates a field that will contain a value of a specific type
		/// </summary>
		public static FormField CreateField(Type type)
		{
			//validate arguments
			if (type == null) throw new ArgumentNullException("type");

			//field
			FormField field;

			//The type is a persistent type
			if (DataType.IsMapped(type))
			{
				field = new ObjectListPickerField(DataType.GetMap(type));

				//if it's a short list, create a DropDown
				/*if (DataBase.Current.Count(type) <= 100)
				{
					field = new DataObjectListPickerField();
				}
				//otherwise create an autocomplete
				else
				{
					field = new DataObjectAutoCompleteField();
				}*/
			}

			//DataType
			else if (type.Equals(typeof(DataType)))
			{
				field = new DataTypeField();
			}

			//just create a simple field for common values
			else
			{
				field = FormField.CreateFieldFrom(type);
			}

			//return
			return field;
		}

		#endregion
	}
}

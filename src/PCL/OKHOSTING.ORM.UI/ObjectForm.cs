using OKHOSTING.Data.Validation;
using OKHOSTING.UI.Controls.Forms;
using System;
using System.Linq;

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
		public void AddFieldFrom(DataMember member)
		{
			//if there's no values defined, exit
			if (member == null) throw new ArgumentNullException(nameof(member));

			//field
			FormField field;

			//String
			if (member.Member.ReturnType.Equals(typeof(string)))
			{
				field = new StringField();

				//set max lenght, if defined
				int maxLenght = (int)StringLengthValidator.GetMaxLength(member.Member.FinalMemberInfo);

				if (maxLenght == 0)
				{
					field.TableWide = true;
				}
				else
				{
					((StringField)field).MaxLenght = maxLenght;
				}

				//set regular expression validation, if defined
				var regex = member.Member.FinalMemberInfo.CustomAttributes.Where(att => att.AttributeType.Equals(typeof(RegexValidator))).SingleOrDefault();

				if (regex != null)
				{
					((StringField)field).RegularExpression = (string)regex.ConstructorArguments[0].Value;
				}
			}

			//DataType
			else if (member.Member.ReturnType.Equals(typeof(DataType)))
			{
				field = new DataTypeField();
			}

			//otherwise delegate to the static method to create the field from the return type
			else
			{
				field = CreateFieldFrom(member.Member.ReturnType);
			}

			field.Name = member.Member.Expression;
			field.Container = this;
			field.Required = RequiredValidator.IsRequired(member.Member.FinalMemberInfo);
			field.CaptionControl.Text = new System.Resources.ResourceManager(member.DataType.InnerType).GetString(member.DataType.FullName.Replace('.', '_') + '_' + member.Member.Expression.Replace('.', '_'));
			if (member.Column.IsPrimaryKey) field.SortOrder = 0;


			//add to fields collection
			Fields.Add(field);
		}

		/// <summary>
		/// Returns the field that corresponds to this DataMember
		/// </summary>
		public FormField GetFieldFor(DataMember member)
		{
			//if there's no values defined, exit
			if (member == null) throw new ArgumentNullException(nameof(member));

			//search corresponding field for this DataMember
			return Fields.Where(f => f.Name == member.Member.Expression).SingleOrDefault();
		}

		#region Static methods

		/// <summary>
		/// Creates a field that will contain a value of a specific type
		/// </summary>
		public static FormField CreateFieldFrom(Type type)
		{
			//validate arguments
			if (type == null) throw new ArgumentNullException("type");

			//field
			FormField field;

			//The type is a persistent type
			if (DataType.IsMapped(type))
			{
				field = new DataObjectListPickerField(DataType.GetMap(type));

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

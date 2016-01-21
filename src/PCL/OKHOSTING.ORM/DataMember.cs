using OKHOSTING.Data.Validation;
using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OKHOSTING.ORM
{
	public class DataMember
	{
		public DataMember()
		{
		}

		public DataMember(DataType type, string member): this(type, member, null)
		{
		}

		public DataMember(DataType type, string member, Column column)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (string.IsNullOrWhiteSpace(member))
			{
				throw new ArgumentNullException("member");
			}

			DataType = type;
			Member = new MemberExpression(type.InnerType, member);

			if (column == null)
			{
				CreateColumn();
			}
			else
			{
				if (column.Table != type.Table)
				{
					throw new ArgumentOutOfRangeException("column", column, "This column does not belong the the Table that TypeMap is mapped to");
				}

				Column = column;
			}
		}

		/// <summary>
		/// The type map that contains this object
		/// </summary>
		public DataType DataType { get; set; }
		
		/// <summary>
		/// The database column where this field will be stored
		/// </summary>
		public Column Column { get; set; }

		/// <summary>
		/// String representing the member (property or field) that is being mapped
		/// </summary>
		public MemberExpression Member { get; set; }

		/// <summary>
		/// Conversions to apply when writing yo or reading from the database
		/// </summary>
		public Conversions.ConverterBase Converter { get; set; }

		/// <summary>
		/// Returns the validators that this DataMember has defined.
		/// </summary>
		/// <remarks>
		/// This is just a filtered list of validators from DataType.Validators where the validators are MemberValidator and it's member equalis this.Member
		/// </remarks>
		public IEnumerable<ValidatorBase> Validators
		{
			get
			{
				return DataType.Validators.Where(v => v is MemberValidator && ((MemberValidator) v).Member == this.Member);
			}
		}

		public object GetValueForColumn(object obj)
		{
			object value = Member.GetValue(obj);

			if (Converter != null)
			{
				value = Converter.MemberToColumn(value);
			}

			return value;
		}

		public void SetValueFromColumn(object obj, object value)
		{
			if (Converter != null)
			{
				value = Converter.ColumnToMember(value);
			}

			Member.SetValue(obj, value);
		}

		public void CreateColumn()
		{
			var finalMember = Member.FinalMemberInfo;

			Column = new Column()
			{
				Table = DataType.Table,
				Name = Member.Expression.Replace('.', '_'),
				DbType = Sql.DbTypeMapper.Parse(MemberExpression.GetReturnType(finalMember)),
				IsNullable = !RequiredValidator.IsRequired(finalMember),
				IsPrimaryKey = IsPrimaryKey(finalMember),
			};

			Column.IsAutoNumber = Column.IsNumeric && Column.IsPrimaryKey && DataType.BaseDataType == null;

			if (Column.IsString)
			{
				Column.Length = StringLengthValidator.GetMaxLength(finalMember);
			}

			DataType.Table.Columns.Add(Column);
		}

		public override string ToString()
		{
			return Member.Expression;
		}

		public static bool IsPrimaryKey(MemberInfo memberInfo)
		{
			return memberInfo.Name.ToString().ToLower() == "id" || memberInfo.CustomAttributes.Where(att => att.AttributeType.Name.ToLower().Contains("key")).Count() > 0;
		}

		/// <summary>
		/// Sets the converter to all DataMembers that have this returnType
		/// </summary>
		/// <param name="converter">Converter to use</param>
		/// <param name="returnType">Return type of the members that will have the converter set</param>
		public static void SetConverter(Conversions.ConverterBase converter, Type returnType)
		{
			foreach (DataType dtype in DataType.AllDataTypes)
			{
				foreach (DataMember dmember in dtype.DataMembers.Where(dm => dm.Member.ReturnType.Equals(returnType)))
				{
					dmember.Converter = converter;
				}
			}
		}
	}

	public class DataMember<T> : DataMember
	{
		public DataMember(): base()
		{
			DataType = typeof(T);
		}

		public DataMember(string member): base(typeof(T), member)
		{
		}

		public DataMember(string member, Column column): base(typeof(T), member, column)
		{
		}

		public DataMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression): base(typeof(T), MemberExpression<T>.GetMemberString(memberExpression))
		{
		}

		public DataMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression, Column column): base(typeof(T), MemberExpression<T>.GetMemberString(memberExpression), column)
		{
		}

		public static DataMember<T> ToGeneric(DataMember dmember)
		{
			if (dmember is DataMember<T>)
			{
				return (DataMember<T>) dmember;
			}

			Type genericDataTypeType = typeof(DataMember<>).MakeGenericType(dmember.DataType.InnerType);

			ConstructorInfo constructor = genericDataTypeType.GetTypeInfo().DeclaredConstructors.Where(c => c.GetParameters().Length == 0).Single();

			DataMember<T> genericMember = (DataMember<T>) constructor.Invoke(null);
			genericMember.Member = dmember.Member;
			genericMember.Column = dmember.Column;
			genericMember.Converter = dmember.Converter;

			return genericMember;
		}
	}
}
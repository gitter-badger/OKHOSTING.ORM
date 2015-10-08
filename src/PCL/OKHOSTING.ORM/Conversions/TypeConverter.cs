using System;
using System.Reflection;

namespace OKHOSTING.ORM.Conversions
{
	public class TypeConverter: ConverterBase<Type, string>
	{
		public override string MemberToColumn(Type memberValue)
		{
			return string.Format("{0}, {1}", memberValue.FullName, memberValue.GetTypeInfo().Assembly.FullName);
		}

		public override Type ColumnToMember(string columnValue)
		{
			return Type.GetType(columnValue);
		}

		public override object MemberToColumn(object memberValue)
		{
			return MemberToColumn((string)memberValue);
		}

		public override object ColumnToMember(object columnValue)
		{
			return ColumnToMember((string)columnValue);
		}
	}
}
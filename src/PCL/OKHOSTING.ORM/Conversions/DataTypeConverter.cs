using System;

namespace OKHOSTING.ORM.Conversions
{
	public class DataTypeConverter : ConverterBase<DataType, string>
	{
		public override string MemberToColumn(DataType memberValue)
		{
			return Serializer.ToString(memberValue);
		}

		public override DataType ColumnToMember(string columnValue)
		{
			return Serializer.ToDataType(columnValue);
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
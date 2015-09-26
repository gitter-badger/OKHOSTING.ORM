using System;

namespace OKHOSTING.ORM.Conversions
{
	public class DataMemberConverter : ConverterBase<DataMember, string>
	{
		public override string MemberToColumn(DataMember memberValue)
		{
			return Serializer.ToString(memberValue);
		}

		public override DataMember ColumnToMember(string columnValue)
		{
			return Serializer.ToDataMember(columnValue);
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
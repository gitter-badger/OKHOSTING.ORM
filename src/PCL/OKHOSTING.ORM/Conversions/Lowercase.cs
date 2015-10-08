namespace OKHOSTING.ORM.Conversions
{
	/// <summary>
	/// Trims a string value, removing white spaces from begining and end
	/// </summary>
	public class Lowercase : ConverterBase<string, string>
	{
		public override string MemberToColumn(string memberValue)
		{
			return memberValue.ToLower();
		}

		public override string ColumnToMember(string columnValue)
		{
			return columnValue.ToLower();
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
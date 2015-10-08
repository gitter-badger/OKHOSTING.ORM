using System;
using System.Collections;
using System.Linq;

namespace OKHOSTING.ORM.Conversions
{
	/// <summary>
	/// Converts a list into a comma separated string
	/// </summary>
	public class ListConverter : ConverterBase<IEnumerable, string>
	{
		public override IEnumerable ColumnToMember(string columnValue)
		{
			throw new NotImplementedException();
		}

		public override string MemberToColumn(IEnumerable memberValue)
		{
			string result = string.Empty;

			foreach (object item in memberValue)
			{
				result += Serializer.ToString(item) + ", ";
			}

			result = result.TrimEnd(' ', ',');

			return result;
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

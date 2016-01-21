using System;
using System.Collections.Generic;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Implements a filter based on the comparison
	/// of a foreign key
	/// </summary>
	public class ForeignKeyFilter : AndFilter
	{
		public DataType DataType { get; set; }

		/// <summary>
		/// Foreign Key DataObject for comparison
		/// </summary>
		public object ValueToCompare { get; set; }

		public System.Reflection.MemberInfo Member { get; set; }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used to link the local with the foreign DataObject
		/// </param>
		/// <param name="valueToCompare">
		/// Foreign DataObject
		/// </param>
		public ForeignKeyFilter(DataType dtype, System.Reflection.MemberInfo member, object valueToCompare)
		{
			if (dtype == null)
			{
				throw new ArgumentNullException(nameof(dtype));
			}

			if (member == null)
			{
				throw new ArgumentNullException(nameof(member));
			}

			DataType = dtype;
			Member = member;
			ValueToCompare = valueToCompare;

			foreach (DataMember pk in dtype.PrimaryKey)
			{
				ValueCompareFilter pkFilter = new ValueCompareFilter();
				pkFilter.Member = pk;
				pkFilter.ValueToCompare = (IComparable) pk.Member.GetValue(ValueToCompare);

				base.InnerFilters.Add(pkFilter);
			}
		}
	}
}
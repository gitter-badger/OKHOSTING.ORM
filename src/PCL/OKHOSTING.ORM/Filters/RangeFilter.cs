using System;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Implements a filter criterion based on a value range
	/// </summary>
	public class RangeFilter: MemberFilter
	{
		public RangeFilter(DataMember member, IComparable minValue, IComparable maxValue)
		{
			Member = member;
			MinValue = minValue;
			MaxValue = maxValue;
		}

		/// <summary>
		/// Minimum value of the allowed range
		/// </summary>
		public readonly IComparable MinValue;
		
		/// <summary>
		/// Maximum value of the allowed range
		/// </summary>
		public readonly IComparable MaxValue;
	}
}
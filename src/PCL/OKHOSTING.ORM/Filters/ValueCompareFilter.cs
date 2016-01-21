using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Data;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Compare a DataMember with a value
	/// </summary>
	public class ValueCompareFilter : CompareFilter
	{
		/// <summary>
		/// Value for comparison
		/// </summary>
		public IComparable ValueToCompare { get; set; }

		public ValueCompareFilter()
		{
		}

		public ValueCompareFilter(DataMember member, IComparable valueToCompare): this(member, valueToCompare, CompareOperator.Equal)
		{
		}

		public ValueCompareFilter(DataMember member, IComparable valueToCompare, CompareOperator compareOperator)
		{
			Member = member;
			ValueToCompare = valueToCompare;
			Operator = compareOperator;
		}
	}
}
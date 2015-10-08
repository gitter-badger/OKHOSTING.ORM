using System;
using System.Collections.Generic;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Filter defined with several conditions merged between them,
	/// with a logical Or operator
	/// </summary>
	public class OrFilter : LogicalOperatorFilter
	{
		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the Or operator
		/// </param>
		public OrFilter() : base(new List<FilterBase>(), OKHOSTING.Data.LogicalOperator.Or)
		{
		}
		
		public OrFilter(List<FilterBase> innerFilters) : base(innerFilters, OKHOSTING.Data.LogicalOperator.Or) 
		{ 
		}
	}
}
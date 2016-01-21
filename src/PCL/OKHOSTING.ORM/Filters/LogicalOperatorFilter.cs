using OKHOSTING.Data;
using System.Collections.Generic;

namespace OKHOSTING.ORM.Filters
{
	
	/// <summary>
	/// Base class for filters that contains a collection of filters and
	/// that compares them with a logical operator
	/// </summary>
	public class LogicalOperatorFilter : Filter
	{ 
		/// <summary>
		/// Collection of conditions or filters that will be merged 
		/// with the and operator
		/// </summary>
		public readonly List<Filter> InnerFilters;

		/// <summary>
		/// Logical operator used in the filter
		/// </summary>
		public readonly LogicalOperator LogicalOperator;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public LogicalOperatorFilter(List<Filter> innerFilters) : this(innerFilters, LogicalOperator.And) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="logicalOperator">
		/// Logical operator used in the filter
		/// </param>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the and operator
		/// </param>
		public LogicalOperatorFilter(List<Filter> innerFilters, LogicalOperator logicalOperator)
		{
			InnerFilters = innerFilters;
			LogicalOperator = logicalOperator;
		}
	}
}
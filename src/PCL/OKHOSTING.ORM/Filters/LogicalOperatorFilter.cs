using OKHOSTING.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OKHOSTING.ORM.Filters
{
	
	/// <summary>
	/// Base class for filters that contains a collection of filters and
	/// that compares them with a logical operator
	/// </summary>
	public class LogicalOperatorFilter : FilterBase
	{ 
		/// <summary>
		/// Collection of conditions or filters that will be merged 
		/// with the and operator
		/// </summary>
		public readonly List<FilterBase> InnerFilters;

		/// <summary>
		/// Logical operator used in the filter
		/// </summary>
		public readonly LogicalOperator LogicalOperator;

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public LogicalOperatorFilter(List<FilterBase> innerFilters) : this(innerFilters, LogicalOperator.And) { }

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
		public LogicalOperatorFilter(List<FilterBase> innerFilters, LogicalOperator logicalOperator)
		{
			InnerFilters = innerFilters;
			LogicalOperator = logicalOperator;
		}
	}
}
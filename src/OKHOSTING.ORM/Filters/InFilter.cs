using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Defines a filter in which the DataMember must be part
	/// of a values set
	/// </summary>
	/// <typeparam name="T">
	/// Type of the items in the values set
	/// </typeparam>
	public class InFilter: MemberFilter
	{
		/// <summary>
		/// List of values of the filter
		/// </summary>
		public readonly List<IComparable> Values = new List<IComparable>();

		/// <summary>
		/// Indicates if the filter comparison will be case sensitive
		/// when listItemsType = System.String
		/// </summary>
		public bool CaseSensitive;
	}
}
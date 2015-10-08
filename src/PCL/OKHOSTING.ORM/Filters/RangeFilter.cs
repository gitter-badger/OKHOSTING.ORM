using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Implements a filter criterion based on a value range
	/// </summary>
	public class RangeFilter: MemberFilter
	{
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
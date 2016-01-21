using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Represents a filter based on a value of a object (Field or Property)
	/// </summary>
	public abstract class MemberFilter : Filter
	{
		/// <summary>
		/// DataMember used to filter
		/// </summary>
		public DataMember Member;

		/// <summary>
		/// Optional alias used for the type's table
		/// </summary>
		public string TypeAlias;
	}
}
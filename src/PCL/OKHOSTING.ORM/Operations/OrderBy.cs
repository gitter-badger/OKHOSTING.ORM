using OKHOSTING.Data;
using OKHOSTING.Sql.Schema;
using System;
using System.Xml.Serialization;

namespace OKHOSTING.ORM.Operations
{
	/// <summary>
	/// An item of an "Order By" clause for Select operations
	/// </summary>
	public class OrderBy
	{
		/// <summary>
		/// MemberMap which determines the sorting
		/// </summary>
		public DataMember Member;

		/// <summary>
		/// The direction of the sorting
		/// </summary>
		public SortDirection Direction;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public OrderBy()
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public OrderBy(DataMember orderBy): this(orderBy, SortDirection.Ascending)
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public OrderBy(DataMember member, SortDirection direction)
		{
			this.Member = member;
			this.Direction = direction;
		}
	}
}
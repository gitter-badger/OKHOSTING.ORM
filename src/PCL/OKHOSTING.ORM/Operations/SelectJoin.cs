using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Operations
{
	/// <summary>
	/// Represents a select join
	/// </summary>
	public class SelectJoin
	{
		public DataType Type { get; set; }

		/// <summary>
		/// Aditional members that will be pat of the "Select" statement
		/// </summary>
		public readonly List<SelectMember> Members = new List<SelectMember>();

		public SelectJoinType JoinType { get; set; }
		
		/// <summary>
		/// Alias name of the joined table
		/// </summary>
		/// <remarks>
		/// Usefull in case a DataType has 2 foreign keys to the same table, son you can query both tables in a select
		/// </remarks>
		public string Alias;

		public readonly List<Filters.FilterBase> On = new List<Filters.FilterBase>();
	}
}
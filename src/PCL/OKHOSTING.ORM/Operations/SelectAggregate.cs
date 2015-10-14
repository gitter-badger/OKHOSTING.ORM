using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Operations
{
	/// <summary>
	/// A select statement that also contains aggreate functions, which requier a groupby element as well
	/// </summary>
	public class SelectAggregate : Select
	{
		public readonly List<SelectAggregateMember> AggregateMembers = new List<SelectAggregateMember>();
		public readonly List<DataMember> GroupBy = new List<DataMember>();
	}
}
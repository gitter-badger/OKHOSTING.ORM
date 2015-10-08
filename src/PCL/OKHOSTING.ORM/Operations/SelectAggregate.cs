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
		public List<SelectAggregateMember> AggregateMembers { get; set; }
		public List<DataMember> GroupBy { get; set; }
	}
}

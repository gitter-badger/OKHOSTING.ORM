using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.ORM.Operations
{
	/// <summary>
	/// Use to define paging in select operations, for example, if you only want to retrieve Objects from index 0 to 100, or 101 to 200
	/// </summary>
	public class SelectLimit
	{
		/// <summary>
		/// Starting 0-based index for the list you want to select
		/// </summary>
		public int From { get; set; }

		/// <summary>
		/// Finishing 0-based index for the list you want to select
		/// </summary>
		public int To { get; set; }

		/// <summary>
		/// Returns the count
		/// </summary>
		public int Count 
		{ 
			get
			{
				return To - From;
			}
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public SelectLimit()
		{
			From = To = 0;
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="from">
		/// </param>
		/// <param name="to">
		/// </param>
		public SelectLimit(int from, int to)
		{
			From = from;
			To = to;
		}
	}
}

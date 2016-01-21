using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Operations
{
	public class Delete : Operation
	{
		public readonly List<Filters.Filter> Where = new List<Filters.Filter>();
	}

	public class Delete<T> : Delete
	{
		public Delete()
		{
			DataType = typeof(T);
		}
	}
}
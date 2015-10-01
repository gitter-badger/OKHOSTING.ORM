using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Operations
{
	public class Delete : Operation
	{
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
	}

	public class Delete<T> : Delete
	{
		public Delete()
		{
			DataType = typeof(T);
		}
	}
}
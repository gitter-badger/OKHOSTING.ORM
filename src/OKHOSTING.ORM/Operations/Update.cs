using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Operations
{
	public class Update : Operation
    {
		public Update()
		{
		}

		public object Instance { get; set; }
		public readonly List<DataMember> Set = new List<DataMember>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
	}

	public class Update<T> : Update
	{
		public new T Instance
		{
			get
			{
				return (T) base.Instance;
			}
			set
			{
				base.Instance = value;
			}
		}

		public Update()
		{
            DataType = typeof(T);
		}
	}
}
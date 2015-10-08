using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM
{
	/// <summary>
	/// Usefull to store non persistent objects in the database
	/// </summary>
	/// <typeparam name="TKey">Type of primary key we want for this object</typeparam>
	/// <typeparam name="TType">Type of object we need to store</typeparam>
	public class PersistentProxy<TKey, TType>: PersistentClass<TKey>
	{
		public string Name { get; set; }
		public TType Instance { get; set; }
	}
}
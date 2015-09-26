using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM
{
	/// <summary>
	/// Usefull as a base class for persistent classes
	/// </summary>
	/// <typeparam name="TKey">Type you wish to use for the primary jey</typeparam>
	public abstract class PersistentClass<TKey>
	{
		public TKey Id { get; set; }

		/// <summary>
		/// Returns the current instance's DataType
		/// </summary>
		public DataType DataType
		{
			get
			{
				return GetType();
			}
		}

		#region Database Operations

		/// <summary>
		/// Inserts the current object into the DataBase
		/// </summary>
		public void Insert()
		{
			DataBase.Default.Insert(this);
		}

		/// <summary>
		/// Updates the current DataObject from the DataBase
		/// </summary>
		public void Update()
		{
			DataBase.Default.Update(this);
		}

		/// <summary>
		/// Deletes the current DataObject from the DataBase
		/// </summary>
		public void Delete()
		{
			DataBase.Default.Delete(this);
		}

		/// <summary>
		/// Loads the current DataObject from the DataBase
		/// </summary>
		/// <returns>True if the current DataObject was found in the DataBase, false otherwise</returns>
		public bool Select()
		{
			return DataBase.Default.Select(this);
		}

		public bool IsSaved()
		{
			return DataBase.Default.IsSaved(this);
		}


		/// <summary>
		/// Loads a list of related objects and populates the collection
		/// </summary>
		/// <param name="memberName">Name of the collection member</param>
		/// <returns>Number of loaded objects</returns>
		public int LoadCollection<TType>(System.Linq.Expressions.Expression<Func<TType, object>> memberExpression) where TType : PersistentClass<TKey>
		{
			return DataBase.Default.LoadCollection<TType>((TType) this, memberExpression);
		}

		#endregion
	}
}
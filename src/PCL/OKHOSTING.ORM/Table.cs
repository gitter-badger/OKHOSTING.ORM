using System;
using System.Linq;
using System.Collections.Generic;
using OKHOSTING.ORM.Operations;
using OKHOSTING.ORM.Filters;

namespace OKHOSTING.ORM
{
	/// <summary>
	/// Allows to access ORM objects in a Dictionary way
	/// </summary>
	/// <typeparam name="TKey">Type of the primary key</typeparam>
	/// <typeparam name="TType">Type of object that will be stored</typeparam>
	public class Table<TKey, TType>: Data.DictionaryBase<TKey, TType>
	{
		public DataBase DataBase { get; set; }
		public readonly DataType DataType = typeof(TType);

		public override void Add(KeyValuePair<TKey, TType> item)
		{
			DataBase.Insert(item.Value);
		}

		public override bool ContainsKey(TKey key)
		{
			Select<TType> select = new Select<TType>();
			select.DataType = DataType;
			select.Members.Add(select.DataType.PrimaryKey.First());
			select.Where.Add(GetPrimaryKeyFilter(DataType, key));

			return DataBase.Select(select).Count() > 0;
		}

		public override IEnumerator<KeyValuePair<TKey, TType>> GetEnumerator()
		{
			Select<TType> select = CreateSelect();
			DataMember pkMember = select.DataType.PrimaryKey.First();
			
			foreach (TType instance in DataBase.Select(select))
			{
				TKey key = (TKey) Convert.ChangeType(pkMember.Member.GetValue(instance), pkMember.Member.ReturnType);
				yield return new KeyValuePair<TKey, TType>(key, instance);
			}
		}

		public override bool IsReadOnly
		{
			get 
			{
				return false; ; 
			}
		}

		public override ICollection<TKey> Keys
		{
			get 
			{
				Select<TType> select = new Select<TType>();
				select.DataType = DataType;
				select.Members.Add(select.DataType.PrimaryKey.First());

				List<TKey> keys = new List<TKey>();
				DataMember pk = select.DataType.PrimaryKey.First();

				foreach(TType instance in DataBase.Select(select))
				{
					keys.Add((TKey) pk.Member.GetValue(instance));
				}

				return keys;
			}
		}

		public override bool Remove(TKey key)
		{
			return DataBase.Delete(this[key]) > 0;
		}

		public override TType this[TKey key]
		{
			get
			{
				TType instance = Activator.CreateInstance<TType>();
				DataType.PrimaryKey.First().Member.SetValue(instance, key);

				if (DataBase.Select(instance))
				{
					return instance;
				}
				else
				{
					return default(TType);
				}
			}
			set
			{
				if (ContainsKey(key))
				{
					DataBase.Update(value);
				}
				else
				{
					Add(new KeyValuePair<TKey, TType>(key, value));
				}
			}
		}

		#region Non public

		protected virtual Filters.Filter GetPrimaryKeyFilter(DataType dtype, TKey key)
		{
			return new Filters.ValueCompareFilter()
			{
				Member = dtype.PrimaryKey.First(),
				ValueToCompare = (IComparable) key,
				Operator = Data.CompareOperator.Equal,
			};
		}

		/// <summary>
		/// Creates a select operation with all DataType's members and inheritance inner joins
		/// </summary>
		/// <returns>Select operation</returns>
		protected Select<TType> CreateSelect()
		{
			Select<TType> select = new Select<TType>();
			DataType dtype = DataType;
			select.AddMembers(dtype.DataMembers.ToArray());

			return select;
		}

		#endregion
	}
}
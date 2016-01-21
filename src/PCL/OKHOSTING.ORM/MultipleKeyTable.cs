using System;
using System.Linq;
using System.Collections.Generic;
using OKHOSTING.ORM.Operations;

namespace OKHOSTING.ORM
{
	/// <summary>
	/// Allows to access multiple-primary-key ORM objects in a Dictionary way
	/// </summary>
	/// <typeparam name="TType">Type of object that will be stored</typeparam>
	public class MultipleKeyTable<TType>: Table<object[], TType>
	{
		public override ICollection<object[]> Keys
		{
			get 
			{
				Select<TType> select = new Select<TType>();
				select.DataType = DataType;
				
				foreach (DataMember pk in DataType.PrimaryKey)
				{
					select.Members.Add(pk);
				}

				var primaryKeys = DataType.PrimaryKey.ToList();
				var keys = new List<object[]>();

				foreach (TType instance in DataBase.Select(select))
				{
					object[] k = new object[primaryKeys.Count];
					
					for (int i = 0; i < primaryKeys.Count; i++)
					{
						k[i] = Convert.ChangeType(primaryKeys[i].Member.GetValue(instance), primaryKeys[i].Member.ReturnType);
					}
					
					keys.Add(k);
				}

				return keys;
			}
		}

		public override IEnumerator<KeyValuePair<object[], TType>> GetEnumerator()
		{
			Select<TType> select = CreateSelect();
			List<DataMember> primaryKeys = DataType.PrimaryKey.ToList();

			foreach (TType instance in DataBase.Select(select))
			{
				object[] key = new object[primaryKeys.Count];

				for (int i = 0; i < primaryKeys.Count; i++)
				{
					key[i] = Convert.ChangeType(primaryKeys[i].Member.GetValue(instance), primaryKeys[i].Member.ReturnType);
				}

				yield return new KeyValuePair<object[], TType>(key, instance);
			}
		}

		protected override Filters.Filter GetPrimaryKeyFilter(DataType dtype, object[] key)
		{
			Filters.AndFilter filter = new Filters.AndFilter();
			var primaryKeys = dtype.PrimaryKey.ToList();

			for (int i = 0; i < primaryKeys.Count; i++)
			{
				filter.InnerFilters.Add(new Filters.ValueCompareFilter()
				{
					Member = primaryKeys[i],
					ValueToCompare = (IComparable)key[i],
					Operator = Data.CompareOperator.Equal,
				});
			}

			return filter;
		}
	}
}
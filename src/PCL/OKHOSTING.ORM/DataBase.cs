﻿using OKHOSTING.Data;
using OKHOSTING.Data.Validation;
using OKHOSTING.ORM.Filters;
using OKHOSTING.ORM.Operations;
using OKHOSTING.Sql;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.ORM
{
	public class DataBase : IOrmDataBase, IDisposable
	{
		protected readonly Dictionary<Type, object> Tables = new Dictionary<Type, object>();

		public DataBase()
		{
		}

		public DataBase(Sql.DataBase nativeDataBase, SqlGeneratorBase sqlGenerator)
		{
			NativeDataBase = nativeDataBase;
			SqlGenerator = sqlGenerator;
		}

		#region Properties

		public Sql.DataBase NativeDataBase { get; set; }

		public SqlGeneratorBase SqlGenerator { get; set; }

		public Table<TKey, T> Table<TKey, T>()
		{
			Table<TKey, T> table = null;

			if (Tables.ContainsKey(typeof(T)))
			{
				table = (Table<TKey, T>)Tables[typeof(T)];
			}
			else
			{
				table = new Table<TKey, T>();
				table.DataBase = this;
				Tables.Add(typeof(T), table);
			}

			return table;
		}

		public MultipleKeyTable<T> MultipleKeyTable<T>()
		{
			MultipleKeyTable<T> table = null;

			if (Tables.ContainsKey(typeof(T)))
			{
				table = (MultipleKeyTable<T>)Tables[typeof(T)];
			}
			else
			{
				table = new MultipleKeyTable<T>();
				table.DataBase = this;
				Tables.Add(typeof(T), table);
			}

			return table;
		}

		IDictionary<TKey, T> IOrmDataBase.Table<TKey, T>()
		{
			return this.Table<TKey, T>();
		}

		#endregion

		#region Operations

		public int Insert(Insert insert)
		{
			OperationEventArgs args = new OperationEventArgs(insert);
			OnBeforeOperation(args);

			if (args.Cancel)
			{
				return 0;
			}

			Validate(insert.Instance);

			Command sql = SqlGenerator.Insert(Parse(insert));
			int result = NativeDataBase.Execute(sql);

			//detect auto generated ID, when applicable
			var autoNumberPK = insert.DataType.PrimaryKey.Where(pk => pk.Column.IsAutoNumber).SingleOrDefault();

			if (autoNumberPK != null)
			{
				var generatedId = NativeDataBase.GetScalar(SqlGenerator.LastAutogeneratedId(insert.DataType.Table));
				autoNumberPK.SetValueFromColumn(insert.Instance, generatedId);
			}

			args.Result = result;

			OnAfterOperation(args);

			return (int)args.Result;
		}

		public int Update(Update update)
		{
			OperationEventArgs args = new OperationEventArgs(update);
			OnBeforeOperation(args);

			if (args.Cancel)
			{
				return 0;
			}

			Validate(update.Instance);

			Command sql = SqlGenerator.Update(Parse(update));
			int result = NativeDataBase.Execute(sql);
			args.Result = result;

			OnAfterOperation(args);

			return (int)args.Result;
		}

		public int Delete(Delete delete)
		{
			OperationEventArgs args = new OperationEventArgs(delete);
			OnBeforeOperation(args);

			if (args.Cancel)
			{
				return 0;
			}

			Command sql = SqlGenerator.Delete(Parse(delete));
			int result = NativeDataBase.Execute(sql);
			args.Result = result;

			OnAfterOperation(args);

			return (int)args.Result;
		}

		public IEnumerable<object> Select(Select select)
		{
			OperationEventArgs args = new OperationEventArgs(select);
			OnBeforeOperation(args);

			if (args.Cancel)
			{
				return null;
			}

			args.Result = SelectPrivate(select);
			OnAfterOperation(args);

			return (IEnumerable<object>) args.Result;
		}

		/// <summary>
		/// Private method for select operations, that simplifies event management in the public method
		/// </summary>
		private IEnumerable<object> SelectPrivate(Select select)
		{
			Command sql = SqlGenerator.Select(Parse(select));

			using (var dataReader = NativeDataBase.GetDataReader(sql))
			{
				while (dataReader.Read())
				{
					object instance = Activator.CreateInstance(select.DataType.InnerType);
					ParseFromSelect(select, dataReader, instance);

					yield return instance;
				}
			}
		}

		public IEnumerable<object> SearchInherited(Select select)
		{
			OperationEventArgs args = new OperationEventArgs(select);
			OnBeforeOperation(args);

			if (args.Cancel)
			{
				return null;
			}

			args.Result = SearchInheritedPrivate(select);
			OnAfterOperation(args);

			return (IEnumerable<object>) args.Result;
		}

		/// <summary>
		/// Private method for search operations, that simplifies event management in the public method
		/// </summary>
		private IEnumerable<object> SearchInheritedPrivate(Select select)
		{
			Command command = new Command();
			List<Tuple<DataType, Select>> selects = new List<Tuple<DataType, Select>>();

			//Crossing the DataTypes
			foreach (DataType subType in select.DataType.SubDataTypesRecursive)
			{
				//create a copy of the select but for the subtype
				Select subSelect = new Select();
				subSelect.DataType = subType;
				subSelect.Where.AddRange(select.Where);
				subSelect.OrderBy.AddRange(select.OrderBy);
				subSelect.Joins.AddRange(select.Joins);
				subSelect.Limit = select.Limit;

				foreach (var member in select.Members)
				{
					subSelect.AddMember(member.DataMember);
				}

				//append all selects into a single command for beter performance
				Command subCommand = SqlGenerator.Select(Parse(subSelect));
				command.Append(subCommand);

				selects.Add(new Tuple<DataType, Select>(subType, subSelect));
			}

			using (var reader = NativeDataBase.GetDataReader(command))
			{
				foreach (var item in selects)
				{
					while (reader.Read())
					{
						object childInstance = Activator.CreateInstance(item.Item1.InnerType);
						ParseFromSelect(item.Item2, reader, childInstance);

						yield return childInstance;
					}

					reader.NextResult();
				}
			}
		}

		#endregion

		#region Instance & generic operations

		/// <summary>
		/// Inserts the object if it's not saved (does not have a primary key), otherwise it updates it
		/// </summary>
		/// <returns>Number of database rows affected by the operation</returns>
		public int Save<T>(T instance)
		{
			if (IsSaved(instance))
			{
				return Update(instance);
			}
			else
			{
				return Insert(instance);
			}
		}

		/// <summary>
		/// Returns a value indicating if the specified DataObject exists on the DataBase (based on its primary key)
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be searched in the DataBase
		/// </param>
		/// <returns>
		/// True if the DataObject exists, False otherwise
		/// </returns>
		public bool Exist<T>(T instance)
		{
			DataType dtype = instance.GetType();

			return Exist(dtype, instance);
		}

		/// <summary>
		/// Returns a value indicating if the specified DataObject exists (based on its primary key)
		/// as a specific DataType
		/// </summary>
		/// <remarks>
		/// Use this method to see if a DataObject exists in the Database as a base class.
		/// A DataObject could exist in the DataBase as a base class, but not as the final class.
		/// </remarks>
		/// <example>
		///	Class Dog inherits from Class Animal. In your Database, you have an Animal with Id = 5, 
		///	but it's not a Dog, it's just an animal. 
		///	Dog dog = new Dog();
		///	dog.Id = 5;
		///	DataBase.Current.Exist(dog);					//will return false
		///	DataBase.Current.Exist(dog, typeof(Dog));		//will return false
		///	DataBase.Current.Exist(dog, typeof(Animal));	//will return true
		/// </example>
		/// <param name="dobj">
		/// DataObject to be searched in the DataBase
		/// </param>
		/// <param name="dtype">
		/// DataType (must be a base class of dobj) that will be searched in the DataBase
		/// </param>
		/// <returns>
		/// True if the DataObject exists as the specified DataType, False otherwise
		/// </returns>
		public bool Exist<T>(DataType dtype, T instance)
		{
			Select<T> select = new Select<T>();

			select.AddMembers(dtype.PrimaryKey.ToArray());
			select.Where.Add(GetPrimaryKeyFilter(dtype, instance));

			return NativeDataBase.ExistsData(SqlGenerator.Select(Parse(select)));
		}

		public int InsertAll<T>(T instance)
		{
			DataType dtype = instance.GetType();
			int result = 0;
			
			//look for unsaved foreign keys to insert
			foreach (var parent in dtype.BaseDataTypes)
			{
				foreach (var member in DataType.GetMapableMembers(parent.InnerType))
				{
					var returnType = MemberExpression.GetReturnType(member);

					//found a foreign key, see if it has a primary key with values, if it does not, we will insert it
					if (DataType.IsMapped(returnType))
					{
						var fkInstance = MemberExpression.GetValue(member, instance);

						//if not saved, insert recursively
						if (fkInstance != null && !IsSaved(fkInstance))
						{
							result += InsertAll(fkInstance);
						}
					}
				}
			}

			//insert the instance itself, if it has not been saved yet
			if (!IsSaved(instance))
			{
				result += Insert(instance);
			}

			//look for unsaved collections
			foreach (var member in DataType.GetMapableMembers(dtype.InnerType))
			{
				var returnType = MemberExpression.GetReturnType(member);

				//this is a collection of items
				if (OKHOSTING.Core.TypeExtensions.IsCollection(returnType))
				{
					var collection = (System.Collections.IEnumerable)MemberExpression.GetValue(member, instance);

					if (collection == null)
					{
						continue;
					}

					//is this a collection of persistent items?
					foreach (var item in collection)
					{
						if (!DataType.IsMapped(item.GetType()))
						{
							//this is not a collection of persisten items
							break;
						}

						//insert collection item recursively
						if (!IsSaved(item))
						{
							InsertAll(item);
						}
					}
				}
			}

			return result;
		}

		public int Insert<T>(T instance)
		{
			DataType dtype = instance.GetType();
			int result = 0;

			Validate(instance);

			foreach (DataType parent in dtype.BaseDataTypes.Reverse())
			{
				Insert insert = new Insert();
				insert.DataType = parent;
				insert.Instance = instance;

				foreach (DataMember dmember in parent.DataMembers)
				{
					var value = dmember.Member.GetValue(instance);

					if (RequiredValidator.HasValue(value))
					{
						insert.Values.Add(dmember);
					}
				}

				result += Insert(insert);

				//get autoincrement id if this is the base table and on of the pk columns is autoincrement
				DataMember autoIncrementMember = parent.PrimaryKey.Where(pk => pk.Column.IsAutoNumber).FirstOrDefault();

				if (parent.BaseDataType == null && autoIncrementMember != null)
				{
					object autoincrement = OKHOSTING.Data.Convert.ChangeType(NativeDataBase.GetScalar(SqlGenerator.LastAutogeneratedId(parent.Table)), autoIncrementMember.Member.ReturnType);
					autoIncrementMember.SetValueFromColumn(instance, autoincrement);
				}
			}

			return result;
		}

		public int Update<T>(T instance)
		{
			DataType dtype = instance.GetType();
			int result = 0;

			Validate(instance);

			foreach (DataType parent in dtype.BaseDataTypes.Reverse())
			{
				Update update = new Update();
				update.DataType = parent;
				update.Instance = instance;

				//update everything but the primary key
				foreach (DataMember dmember in parent.DataMembers.Except(parent.PrimaryKey))
				{
					update.Set.Add(dmember);
				}

				update.Where.Add(GetPrimaryKeyFilter(parent, instance));

				result += Update(update);
			}

			return result;
		}

		public int Delete<T>(T instance)
		{
			DataType dtype = instance.GetType();
			int result = 0;

			//go from child to base class inheritance
			foreach (DataType parent in dtype.BaseDataTypes)
			{
				Delete delete = new Delete();
				delete.DataType = parent;
				delete.Where.Add(GetPrimaryKeyFilter(parent, instance));

				result += Delete(delete);
			}
			
			return result;
		}

		public bool Select<T>(T instance)
		{
			DataType dtype = instance.GetType();
			Select<T> select = new Select<T>();
			select.AddMembers(dtype.AllDataMembers);
			select.Where.Add(GetPrimaryKeyFilter(dtype, instance));

			Command sql = SqlGenerator.Select(Parse(select));

			using (var dataReader = NativeDataBase.GetDataReader(sql))
			{
				if (dataReader.Read())
				{
					ParseFromSelect(select, dataReader, instance);
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public IEnumerable<T> Select<T>(Select<T> select)
		{
			foreach (object result in Select((Select) select))
			{
				yield return (T) result;
			}
		}

		/// <summary>
		/// Returns all objects in a table
		/// </summary>
		public IEnumerable<T> Select<T>()
		{
			Select<T> select = new Select<T>();
			select.AddMembers(select.DataType.AllDataMembers);

			return Select(select);
		}

		/// <summary>
		/// Returns a list of objects filtered by one of the members
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="member">DataMember that will be evaluated for filtering</param>
		/// <param name="value">Value that the DataMember must match</param>
		/// <returns>List of filtered objects</returns>
		public IEnumerable<T> Select<T>(DataMember<T> member, IComparable value)
		{
			Select<T> select = new Select<T>();
			select.AddMembers(select.DataType.AllDataMembers);
			select.Where.Add(new ValueCompareFilter(member, value));

			return Select(select);
		}

		public IEnumerable<T> SearchInherited<T>(Select<T> select)
		{
			foreach (object item in SearchInherited((Select) select))
			{
				yield return (T) item;
			}
		}

		public IEnumerable<T> SearchInherited<T>(T instance)
		{
			Select select = new Select();
			select.DataType = instance.GetType();
			select.Where.Add(GetPrimaryKeyFilter(select.DataType, instance)); //Creating Primary Key filter

			foreach (var item in SearchInherited(select))
			{
				yield return (T) item;
			}
		}

		/// <summary>
		/// Loads a list of related objects and populates the collection
		/// </summary>
		/// <param name="memberName">Name of the collection member</param>
		/// <returns>Number of loaded objects</returns>
		public int LoadCollection<T>(T instance, System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
		{
			string memberString = MemberExpression<T>.GetMemberString(memberExpression);
			System.Reflection.MemberInfo memberInfo = instance.GetType().GetTypeInfo().DeclaredMembers.Where(m => m.Name == memberString).Single();
			System.Type memberReturnType = MemberExpression.GetReturnType(memberInfo);

			DataType collectionDataType = null;

			//determine the type of objects to be loaded
			if (memberReturnType.IsArray)
			{
				collectionDataType = memberReturnType.GetElementType();
			}
			else if (memberReturnType.GetTypeInfo().IsGenericType)
			{
				collectionDataType = memberReturnType.GenericTypeArguments[0];
			}

			//get the foreign key member
			var fk = collectionDataType.InnerType.GetTypeInfo().DeclaredMembers.Where(m => (m is System.Reflection.FieldInfo || m is System.Reflection.PropertyInfo) && MemberExpression.GetReturnType(m) == memberInfo.DeclaringType);

			//this method is valid only if there is only 1 foreign key in the nested datatype, so we can determine the relationship correctly
			if (fk.Count() != 1)
			{
				throw new ArgumentException("memberName", string.Format("Could not automatically determine the correct foreign key since there is more than one member with return type {0} declared in {1}", memberInfo.DeclaringType, collectionDataType));
			}

			//create select and add filter
			var select = new OKHOSTING.ORM.Operations.Select();
			select.DataType = collectionDataType;
			select.AddMembers(collectionDataType.AllDataMembers);

			DataType dtype = instance.GetType();
			foreach (var pk in dtype.PrimaryKey)
			{
				select.Where.Add(new Filters.ValueCompareFilter(collectionDataType[fk.First().Name + "." + pk.Member], (IComparable) pk.Member.GetValue(instance)));
			}

			//get list of objects
			var result = SearchInherited(select).ToList();

			//if the list is an array, try to "set"
			if (memberReturnType.IsArray)
			{
				MemberExpression.SetValue(memberInfo, this, result.ToArray());
			}
			else
			{
				//if the list is not an array, we asume it's a List and just add the results
				System.Collections.IList currentList = (System.Collections.IList) MemberExpression.GetValue(memberInfo, instance);

				//if list is null, initialize
				if (currentList == null)
				{
					currentList = (System.Collections.IList) Activator.CreateInstance(memberReturnType);
					MemberExpression.SetValue(memberInfo, instance, currentList);
				}

				//add objects
				foreach (var item in result)
				{
					currentList.Add(item);
				}
			}

			//return number of objects loaded
			return result.Count;
		}

		public IEnumerable<ValidationError> Validate<T>(T obj)
		{
			DataType dtype = obj.GetType();

			foreach (var validator in dtype.Validators)
			{
				var error = validator.Validate(obj);

				if (error != null)
				{
					yield return error;
				}
			}
		}

		#endregion

		#region Create and drop

		public void Create(IEnumerable<DataType> dtypes)
		{
			//create all tables and indexes first
			foreach (DataType dt in dtypes)
			{
				Create(dt);
			}

			//create all foreign keys next
			foreach (DataType dt in dtypes)
			{
				foreach (var fk in dt.Table.ForeignKeys)
				{
					Command sql = SqlGenerator.Create(fk);
					NativeDataBase.Execute(sql);
				}
			}
		}

		public void Create(DataType dtype)
		{
			Command sql;

			//create all tables and indexes
			sql = SqlGenerator.Create(dtype.Table);
			NativeDataBase.Execute(sql);

			foreach (Index index in dtype.Table.Indexes)
			{
				sql = SqlGenerator.Create(index);
				NativeDataBase.Execute(sql);
			}
		}

		public void Create<T>()
		{
			Create(typeof(T));
		}

		/// <summary>
		/// Drop all tables mapped to a list of DataTypes
		/// </summary>
		/// <param name="dtypes">List of DataTypes which tables will be dropped</param>
		public void Drop(IEnumerable<DataType> dtypes)
		{
			//drop all foreign keys first to avoid dependency errors
			foreach (DataType dt in dtypes)
			{
				foreach (var fk in dt.Table.ForeignKeys)
				{
					if (NativeDataBase.ExistsConstraint(fk.FullName))
					{
						Command sql = SqlGenerator.Drop(fk);

						try
						{
							NativeDataBase.Execute(sql);
						}
						catch { }
					}
				}
			}

			//now we delete all tables
			foreach (DataType dt in dtypes)
			{
				Drop(dt);
			}
		}

		public void Drop(DataType dtype)
		{
			Command sql;

			//foreach (Sql.Index index in dtype.Table.Indexes)
			//{
			//	if (NativeDataBase.ExistsIndex(index.FullName))
			//	{
			//		sql = SqlGenerator.Drop(index);
			//		NativeDataBase.Execute(sql);
			//	}
			//}

			if (NativeDataBase.ExistsTable(dtype.Table.Name))
			{
				sql = SqlGenerator.Drop(dtype.Table);
				NativeDataBase.Execute(sql);
			}
		}

		public void Drop<T>()
		{
			Drop(typeof(T));
		}

		#endregion

		#region Filter parsing

		protected Sql.Filters.FilterBase Parse(FilterBase filter)
		{
			//Validating if there are filters defined
			if (filter == null) return null;

			if (filter is CustomFilter) return Parse((CustomFilter)filter);
			if (filter is InFilter) return Parse((InFilter)filter);
			if (filter is LikeFilter) return Parse((LikeFilter)filter);
			if (filter is RangeFilter) return Parse((RangeFilter)filter);
			if (filter is MemberCompareFilter) return Parse((MemberCompareFilter)filter);
			if (filter is ValueCompareFilter) return Parse((ValueCompareFilter)filter);
			if (filter is AndFilter) return Parse((AndFilter)filter);
			if (filter is OrFilter) return Parse((OrFilter)filter);

			throw new ArgumentOutOfRangeException("filter");
		}

		protected Sql.Filters.CustomFilter Parse(CustomFilter filter)
		{
			return new Sql.Filters.CustomFilter()
			{
				Filter = filter.Filter,
			};
		}

		protected Sql.Filters.InFilter Parse(InFilter filter)
		{
			var native = new Sql.Filters.InFilter();
			native.Column = filter.Member.Column;
			native.CaseSensitive = filter.CaseSensitive;
			native.TableAlias = filter.TypeAlias;

			foreach (IComparable v in filter.Values)
			{
				IComparable converted;

				if (filter.Member.Converter != null)
				{
					converted = (IComparable) filter.Member.Converter.MemberToColumn(v);
				}
				else
				{
					converted = v;
				}

				native.Values.Add(converted);
			}

			return native;
		}

		protected Sql.Filters.LikeFilter Parse(LikeFilter filter)
		{
			string pattern;

			if (filter.Member.Converter != null)
			{
				pattern = (string) filter.Member.Converter.MemberToColumn(filter.Pattern);
			}
			else
			{
				pattern = filter.Pattern;
			}

			return new Sql.Filters.LikeFilter()
			{
				Column = filter.Member.Column,
				Pattern = filter.Pattern,
				CaseSensitive = filter.CaseSensitive,
				TableAlias = filter.TypeAlias
			};
		}

		protected Sql.Filters.RangeFilter Parse(RangeFilter filter)
		{
			IComparable minValue, maxValue;

			if (filter.Member.Converter != null)
			{
				minValue = (IComparable)filter.Member.Converter.MemberToColumn(filter.MinValue);
				maxValue = (IComparable)filter.Member.Converter.MemberToColumn(filter.MaxValue);
			}
			else
			{
				minValue = filter.MinValue;
				maxValue = filter.MaxValue;
			}

			return new Sql.Filters.RangeFilter()
			{
				Column = filter.Member.Column,
				MinValue = minValue,
				MaxValue = maxValue,
				TableAlias = filter.TypeAlias,
			};
		}

		protected Sql.Filters.ColumnCompareFilter Parse(MemberCompareFilter filter)
		{
			return new Sql.Filters.ColumnCompareFilter()
			{
				Column = filter.Member.Column, 
				ColumnToCompare = filter.MemberToCompare.Column,
				Operator = filter.Operator,
				TableAlias = filter.TypeAlias,
				ColumnToCompareTableAlias = filter.MemberToCompareTypeAlias
			};
		}

		protected Sql.Filters.ValueCompareFilter Parse(ValueCompareFilter filter)
		{
			IComparable value;

			if (filter.Member.Converter != null)
			{
				value = (IComparable)filter.Member.Converter.MemberToColumn(filter.ValueToCompare);
			}
			else
			{
				value = filter.ValueToCompare;
			}

			return new Sql.Filters.ValueCompareFilter()
			{
				Column = filter.Member.Column,
				ValueToCompare = value,
				Operator = filter.Operator,
				TableAlias = filter.TypeAlias
			};
		}

		protected Sql.Filters.AndFilter Parse(AndFilter filter)
		{
			var native = new Sql.Filters.AndFilter();

			foreach (FilterBase f in filter.InnerFilters)
			{
				native.InnerFilters.Add(Parse(f));
			}

			return native;
		}

		protected Sql.Filters.OrFilter Parse(OrFilter filter)
		{
			var native = new Sql.Filters.OrFilter();

			foreach (FilterBase f in filter.InnerFilters)
			{
				native.InnerFilters.Add(Parse(f));
			}

			return native;
		}

		#endregion

		#region Operation parsing

		protected Sql.Operations.Insert Parse(Insert insert)
		{
			if (insert == null)
			{
				return null;
			}

			var native = new OKHOSTING.Sql.Operations.Insert();
			native.Table = insert.DataType.Table;

			foreach (DataMember member in insert.Values)
			{
				native.Values.Add(new Sql.Operations.ColumnValue(member.Column, member.GetValueForColumn(insert.Instance)));
			}

			return native;
		}

		protected Sql.Operations.Update Parse(Update update)
		{
			if (update == null)
			{
				return null;
			}
			
			var native = new OKHOSTING.Sql.Operations.Update();
			native.Table = update.DataType.Table;

			foreach (DataMember member in update.Set)
			{
				native.Set.Add(new Sql.Operations.ColumnValue(member.Column, member.GetValueForColumn(update.Instance)));
			}

			foreach (Filters.FilterBase filter in update.Where)
			{
				native.Where.Add(Parse(filter));
			}

			return native;
		}

		protected Sql.Operations.Delete Parse(Delete delete)
		{
			if (delete == null)
			{
				return null;
			}
			
			var native = new OKHOSTING.Sql.Operations.Delete();
			native.Table = delete.DataType.Table;

			foreach (Filters.FilterBase filter in delete.Where)
			{
				native.Where.Add(Parse(filter));
			}

			return native;
		}

		protected Sql.Operations.Select Parse(Select select)
		{
			if (select == null)
			{
				return null;
			}
			
			if (select is SelectAggregate)
			{
				return Parse((SelectAggregate)select);
			}

			return Parse(select, new OKHOSTING.Sql.Operations.Select());
		}

		protected Sql.Operations.SelectAggregate Parse(SelectAggregate select)
		{
			if (select == null)
			{
				return null;
			}
			
			var native = (OKHOSTING.Sql.Operations.SelectAggregate)Parse(select, new OKHOSTING.Sql.Operations.SelectAggregate());

			foreach (SelectAggregateMember agregateMember in select.AggregateMembers)
			{
				native.AggregateColumns.Add(Parse(agregateMember));
			}

			foreach (DataMember groupBy in select.GroupBy)
			{
				native.GroupBy.Add(groupBy.Column);
			}

			return native;
		}

		protected Sql.Operations.SelectAggregateColumn Parse(SelectAggregateMember aggregateMember)
		{
			return new Sql.Operations.SelectAggregateColumn(aggregateMember.DataMember.Column, aggregateMember.AggregateFunction, aggregateMember.Alias, aggregateMember.Distinct);
		}

		protected Sql.Operations.OrderBy Parse(OrderBy orderBy)
		{
			if (orderBy == null)
			{
				return null;
			}
			
			var native = new OKHOSTING.Sql.Operations.OrderBy();
			native.Column = orderBy.Member.Column;
			native.Direction = orderBy.Direction;

			return native;
		}

		protected Sql.Operations.SelectLimit Parse(SelectLimit limit)
		{
			if (limit == null)
			{
				return null;
			}

			var native = new OKHOSTING.Sql.Operations.SelectLimit();
			native.From = limit.From;
			native.To = limit.To;

			return native;
		}

		protected Sql.Operations.SelectJoin Parse(SelectJoin join)
		{
			if (join == null)
			{
				return null;
			}
			
			var native = new OKHOSTING.Sql.Operations.SelectJoin();
			native.Table = join.Type.Table;
			native.JoinType = Parse(join.JoinType);
			native.Alias = join.Alias;

			foreach (SelectMember selectMember in join.Members)
			{
				native.Columns.Add(new Sql.Operations.SelectColumn(selectMember.DataMember.Column, selectMember.Alias));
			}

			foreach (Filters.FilterBase filter in join.On)
			{
				native.On.Add(Parse(filter));
			}

			return native;
		}

		protected Sql.Operations.SelectJoinType Parse(SelectJoinType joinType)
		{
			return (Sql.Operations.SelectJoinType) Enum.Parse(typeof(Sql.Operations.SelectJoinType), joinType.ToString());
		}

		protected Sql.Operations.Select Parse(Select select, Sql.Operations.Select native)
		{
			if (select == null)
			{
				return null;
			}

			if (native == null)
			{
				throw new ArgumentNullException("native");
			}
			
			native.Table = select.DataType.Table;
			native.Limit = Parse(select.Limit);
			
			if (select.Members.Count == 0)
			{
				select.AddMembers(select.DataType.AllDataMembers);
			}

			foreach (SelectMember selectMember in select.Members)
			{
				native.Columns.Add(new Sql.Operations.SelectColumn(selectMember.DataMember.Column, selectMember.Alias));
			}

			foreach (SelectJoin join in select.Joins)
			{
				native.Joins.Add(Parse(join));
			}

			foreach (Filters.FilterBase filter in select.Where)
			{
				native.Where.Add(Parse(filter));
			}

			foreach (OrderBy orderBy in select.OrderBy)
			{
				native.OrderBy.Add(Parse(orderBy));
			}

			return native;
		}

		#endregion

		#region Tools

		protected Filters.FilterBase GetPrimaryKeyFilter<T>(DataType dtype, T instance)
		{
			Filters.AndFilter filter = new Filters.AndFilter();
			var primaryKeys = dtype.PrimaryKey.ToList();

			for (int i = 0; i < primaryKeys.Count; i++)
			{
				filter.InnerFilters.Add(new Filters.ValueCompareFilter()
				{
					Member = primaryKeys[i],
					ValueToCompare = (IComparable)primaryKeys[i].Member.GetValue(instance),
					Operator = Data.CompareOperator.Equal,
				});
			}

			return filter;
		}

		protected void ParseFromSelect<T>(Select select, IDataReader dataReader, T instance)
		{
			foreach (SelectMember member in select.Members)
			{
				if (!MemberExpression.IsReadOnly(member.DataMember.Member.FinalMemberInfo))
				{
					object value = string.IsNullOrWhiteSpace(member.Alias) ? dataReader[member.DataMember.Column.Name] : dataReader[member.Alias];

					if (value != null)
					{
						member.DataMember.SetValueFromColumn(instance, value);
					}
				}
			}

			foreach (SelectJoin join in select.Joins)
			{
				foreach (SelectMember member in join.Members)
				{
					if (!MemberExpression.IsReadOnly(member.DataMember.Member.FinalMemberInfo))
					{
						string expression = member.Alias.Replace('_', '.');
						object value = string.IsNullOrWhiteSpace(member.Alias) ? dataReader[member.DataMember.Column.Name] : dataReader[member.Alias];

						if (member.DataMember.Converter != null)
						{
							value = member.DataMember.Converter.ColumnToMember(value);
						}

						if (value != null)
						{
							MemberExpression.SetValue(expression, instance, value);
						}
					}
				}
			}
		}

		public void Dispose()
		{
			NativeDataBase.Dispose();
		}

		#endregion

		#region Events

		/// <summary>
		/// Delegate for the database interaction events
		/// </summary>
		public delegate void DataBaseOperationEventHandler(DataBase sender, OperationEventArgs eventArgs);

		/// <summary>
		/// Event thrown before of execute sentences against the database
		/// </summary>
		public event DataBaseOperationEventHandler BeforeOperation;

		/// <summary>
		/// Event thrown after of execute sentences against the database
		/// </summary>
		public event DataBaseOperationEventHandler AfterOperation;

		/// <summary>
		/// Raises the BeforeExecute event
		/// </summary>
		public virtual void OnBeforeOperation(OperationEventArgs e)
		{
			if (BeforeOperation != null) BeforeOperation(this, e);
		}

		/// <summary>
		/// Raises the AfterExecute event
		/// </summary>
		public virtual void OnAfterOperation(OperationEventArgs e)
		{
			if (AfterOperation != null) AfterOperation(this, e);
		}

		#endregion

		#region Static events

		/// <summary>
		/// Delegate used for the database creation. 
		/// </summary>
		public delegate DataBase SetupDataBaseEventHandler();

		/// <summary>
		/// Subscribe to this event to create the actual database that will be used in your apps, system-wide. Should only have 1 subscriber. If it has more it will return the last subscriber's result
		/// </summary>
		public static event SetupDataBaseEventHandler Setup;

		/// <summary>
		/// Allows you (or plugins) to perform adittional configurations on newly created databases
		/// </summary>
		public delegate void SettingUpDataBaseEventHandler(DataBase dataBase);

		/// <summary>
		/// Subscribe to this event to create the actual database that will be used in your projects. Allow for "plugins" to subscribe to dabase events and affect system wide behaviour
		/// </summary>
		public static event SettingUpDataBaseEventHandler SettingUp;

		/// <summary>
		/// Will create a ready to use database. 
		/// You should subscribeto Create and (optionally) Created events to return a fully configured database. Then just call this method from everywhere else.
		/// </summary>
		public static DataBase CreateDataBase()
		{
			if (DataBase.Setup == null)
			{
				throw new NullReferenceException("DataBase.Setup event has not subsrcibed method to actually create a configured DataBase. Subscribe to this event and create your own instance.");
			}

			DataBase db = DataBase.Setup();

			if (DataBase.SettingUp != null)
			{
				DataBase.SettingUp(db);
			}

			return db;
		}

		#endregion

		#region Static methods

		public static bool IsSaved<T>(T instance)
		{
			DataType dtype = instance.GetType();

			//see if all primery key members has values
			foreach (var pk in dtype.PrimaryKey)
			{
				var value = pk.Member.GetValue(instance);

				if (!RequiredValidator.HasValue(value) || (Core.TypeExtensions.IsNumeric(pk.Member.ReturnType) && System.Convert.ToInt64(value) == 0))
				{
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}
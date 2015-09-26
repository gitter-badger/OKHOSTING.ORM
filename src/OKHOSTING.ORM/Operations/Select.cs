using OKHOSTING.Data.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Operations
{
	/// <summary>
	/// A select SQL query
	/// </summary>
	public class Select
	{
		public DataType From { get; set; }
		public SelectLimit Limit { get; set; }
		public readonly List<SelectMember> Members = new List<SelectMember>();
		public readonly List<SelectJoin> Joins = new List<SelectJoin>();
		public readonly List<Filters.FilterBase> Where = new List<Filters.FilterBase>();
		public readonly List<OrderBy> OrderBy = new List<OrderBy>();

		public void AddMember(string memberExpression)
		{
			//if this is a native datamember, just add a SelectMember
			if (From.IsMapped(memberExpression))
			{
				DataMember dmember = From[memberExpression];

				//this is a native member of this dataType
				SelectMember sm = new SelectMember(dmember, dmember.Member.Expression.Replace('.', '_'));
				Members.Add(sm);

				//finish iteration here
				return;
			}

			//see if this is a dataMember from a base type
			foreach (DataType parent in From.BaseDataTypes.Skip(1))
			{
				if (!parent.IsMapped(memberExpression))
				{
					continue;
				}

				//if this is a native datamember, just add a SelectMember
				DataMember dmember = parent[memberExpression];

				//create join for child-parent relationship
				SelectJoin join = Joins.Where(j => j.Type == parent && j.Alias == parent.InnerType.Name + "_base").SingleOrDefault();

				if (join == null)
				{
					join = new SelectJoin();
					join.JoinType = Sql.Operations.SelectJoinType.Inner;
					join.Type = parent;
					join.Alias = parent.InnerType.Name + "_base";

					var childPK = From.PrimaryKey.ToList();
					var joinPK = join.Type.PrimaryKey.ToList();

					for (int y = 0; y < joinPK.Count; y++)
					{
						//DataMember pk in join.Type.PrimaryKey
						var filter = new Filters.MemberCompareFilter()
						{
							Member = childPK[y],
							MemberToCompare = joinPK[y],
							MemberToCompareTypeAlias = join.Alias,
						};

						join.On.Add(filter);
					}

					Joins.Add(join);
				}

				//this is a native member of this dataType
				SelectMember sm = new SelectMember(dmember, dmember.Member.Expression.Replace('.', '_'));
				join.Members.Add(sm);

				//finish iteration here
				return;
			}


			//if the expression was not found as a single datamember, split it in nested members
			List<System.Reflection.MemberInfo> nestedMemberInfos = MemberExpression.GetMemberInfos(From.InnerType, memberExpression).ToList();

			//check every part of the expression
			for (int i = 0; i < nestedMemberInfos.Count; i++)
			{
				System.Reflection.MemberInfo memberInfo = nestedMemberInfos[i];
				bool isTheFirstOne = i == 0;
				bool isTheLastOne = i == nestedMemberInfos.Count - 1;
				string currentExpression = string.Empty;

				for (int y = 0; y <= i; y++)
				{
					currentExpression += '.' + nestedMemberInfos[y].Name;
				}

				currentExpression = currentExpression.Trim('.');

				//if this is a dataMember from a base type, create join for that relationship
				foreach (DataType parent in From.BaseDataTypes)
				{
					DataType referencingDataType = isTheFirstOne ? parent : MemberExpression.GetReturnType(nestedMemberInfos[i - 1]);

					//this is not a native or inherited DataMember, so we must detect the nested members and create the respective joins
					if (!isTheLastOne && DataType.IsMapped(MemberExpression.GetReturnType(memberInfo)))
					{
						DataType foreignDataType = MemberExpression.GetReturnType(memberInfo);
						bool foreignKeyIsMapped = true;

						foreach (DataMember foreignKey in foreignDataType.PrimaryKey)
						{
							DataMember localKey = referencingDataType.DataMembers.Where(m => m.Member.Expression == memberInfo.Name + "." + foreignKey.Member).SingleOrDefault();

							if (localKey == null)
							{
								foreignKeyIsMapped = false;
							}
						}

						if (foreignKeyIsMapped)
						{
							SelectJoin foreignJoin = Joins.Where(j => j.Type == foreignDataType && j.Alias == currentExpression.Replace('.', '_')).SingleOrDefault();

							if (foreignJoin == null)
							{
								foreignJoin = new SelectJoin();
								foreignJoin.JoinType = Sql.Operations.SelectJoinType.Inner;
								foreignJoin.Type = foreignDataType;
								foreignJoin.Alias = currentExpression.Replace('.', '_');

								string previousJoinAlias = string.Empty;

								if (isTheFirstOne && parent != From)
								{
									previousJoinAlias = parent.InnerType.Name + "_base";
								}
								else
								{
									for (int y = 0; y <= i - 1; y++)
									{
										previousJoinAlias += '.' + nestedMemberInfos[y].Name;
									}

									previousJoinAlias = previousJoinAlias.Trim('.');
								}

								foreach (DataMember foreignKey in foreignDataType.PrimaryKey)
								{
									DataMember localKey = referencingDataType.DataMembers.Where(m => m.Member.Expression == memberInfo.Name + "." + foreignKey.Member).SingleOrDefault();

									var filter = new Filters.MemberCompareFilter()
									{
										Member = localKey,
										MemberToCompare = foreignKey,
										TypeAlias = previousJoinAlias,
										MemberToCompareTypeAlias = foreignJoin.Alias,
									};

									foreignJoin.On.Add(filter);
								}

								Joins.Add(foreignJoin);
							}

							break;
						}
					}

					if (!isTheFirstOne && isTheLastOne)
					{
						DataMember dmember = referencingDataType[memberInfo.Name];
						SelectJoin foreignJoin = Joins.Where(j => j.Type == referencingDataType && j.Alias == currentExpression.Replace("." + memberInfo.Name, string.Empty).Replace('.', '_')).SingleOrDefault();
						SelectMember sm = new SelectMember(dmember, currentExpression.Replace('.', '_'));
						foreignJoin.Members.Add(sm);
						break;
					}
				}
			}
		}

		public void AddMember(DataMember dataMember)
		{
			AddMember(dataMember.Member.Expression);
		}

		public void AddMembers(IEnumerable<string> memberExpressions)
		{
			AddMembers(memberExpressions.ToArray());
		}

		public void AddMembers(params string[] memberExpressions)
		{
			foreach (var expression in memberExpressions)
			{
				AddMember(expression);
			}
		}

		public void AddMembers(IEnumerable<DataMember> dataMembers)
		{
			AddMembers(dataMembers.ToArray());
		}

		public void AddMembers(params DataMember[] dataMembers)
		{
			List<string> stringExpressions = new List<string>();

			foreach (var dm in dataMembers)
			{
				string dataMemberString = dm.Member.Expression;
				stringExpressions.Add(dataMemberString);
			}

			AddMembers(stringExpressions.ToArray());
		}
	}

	public class Select<T> : Select
	{
		public Select()
		{
			From = typeof(T);
		}

		public void AddMembers(params System.Linq.Expressions.Expression<Func<T, object>>[] memberExpressions)
		{
			List<string> stringExpressions = new List<string>();

			foreach (var expression in memberExpressions)
			{
				string dataMemberString = MemberExpression<T>.GetMemberString(expression);
				stringExpressions.Add(dataMemberString);
			}

			AddMembers(stringExpressions.ToArray());
		}
	}
}
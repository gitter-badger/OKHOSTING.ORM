using OKHOSTING.UI;
using OKHOSTING.UI.Controls;
using OKHOSTING.UI.Controls.Layouts;
using System;
using System.Linq;
using OKHOSTING.ORM.Operations;
using System.Collections.Generic;

namespace OKHOSTING.ORM.UI.Forms
{
	/// <summary>
	/// Creates a grid that shows a collection of persistent objects
	/// </summary>
	public class ObjectGrid
	{
		/// <summary>
		/// The grid that will be populated with all the fields. 
		/// Add this content to your page after calling DataBind()
		/// </summary>
		public IGrid Content
		{
			get;
			protected set;
		}

		/// <summary>
		/// Select operation that will be used to populate this grid
		/// </summary>
		public Select DataSource { get; set; }

		/// <summary>
		/// Constructs the grid and populates it with data
		/// </summary>
		public void DataBind()
		{
			Content = Platform.Current.Create<IGrid>();
			var db = DataBase.CreateDataBase();

			//if DataSource has no members defined, we will use the default members
			if (DataSource.Members.Count == 0)
			{
                var defaultMembers = DataSource.DataType.DataMembers.Where(dm => dm.SelectByDefault);
                
                //if there are no default members, we use all fucking members
                if (defaultMembers.Count() == 0)
                {
                    defaultMembers = DataSource.DataType.DataMembers;
                }

                foreach (var defaultDataMember in defaultMembers)
				{
					DataSource.Members.Add(new SelectMember(defaultDataMember));
				}
			}

			//find out the total number of records
			SelectAggregate count = new SelectAggregate();
			count.DataType = DataSource.DataType;
			count.Joins.AddRange(DataSource.Joins);
			count.Where.AddRange(DataSource.Where);
			count.AggregateMembers.Add(new SelectAggregateMember(DataSource.DataType.PrimaryKey.First(), SelectAggregateFunction.Count));

			long totalrecordsCount = (long) db.SelectScalar(count);
			int pageSize = DataSource.Limit == null ? 0 : DataSource.Limit.Count;
			int currentPage = DataSource.Limit == null ? 0 : DataSource.Limit.From / pageSize;
			int pagesCount = DataSource.Limit == null ? 1 : (int) Math.Ceiling((decimal) totalrecordsCount / pageSize);

			Content.ColumnCount = DataSource.Members.Count; //one column per member
			Content.RowCount = 1; //create header first

			//create header row
			int column = 0;

			foreach (DataMember member in DataSource.Members.Select(dm => dm.DataMember))
			{
				ILabelButton header = Platform.Current.Create<ILabelButton>();
				header.Text = Translator.Translate(member.Member.FinalMemberInfo);
				header.Click += Header_Click;

				Content.SetContent(0, column, header);
				column++;
			}

			//create data rows
			var result = db.Select(DataSource);
			db.Dispose();

			foreach (object instance in result)
			{
				column = 0;
				Content.RowCount++;

				foreach (DataMember member in DataSource.Members.Select(dm => dm.DataMember))
				{
					ILabel content = Platform.Current.Create<ILabel>();
					content.Text = member.Member.GetValue(instance).ToString();

					Content.SetContent(Content.RowCount - 1, column, content);
				}
			}

			//create footer and pagination
			if (pageSize != 0)
			{
				Content.RowCount++;
				IGrid pagination = Platform.Current.Create<IGrid>();
				pagination.RowCount = 1;
				pagination.ColumnCount = 2;

				//add page size picker
				IListPicker pageSizeOptions = Platform.Current.Create<IListPicker>();
				pageSizeOptions.Items = new List<string>();

				//handle page size change
				pageSizeOptions.ValueChanged += PageSizeOptions_ValueChanged;

				//create 5 options for page sizes
				for (int i = 1; i < 6; i++)
				{
					pageSizeOptions.Items.Add((pageSize * i).ToString());
				}

				//add current page picker
				IListPicker pageNumbers = Platform.Current.Create<IListPicker>();
				pageNumbers.Items = new List<string>();

				//handle paging
				pageNumbers.ValueChanged += PageNumbers_ValueChanged;
				
				//add all pages
				for (int i = 1; i < pagesCount; i++)
				{
					pageNumbers.Items.Add(i.ToString());
				}

				//set current page
				pageNumbers.Value = currentPage.ToString();

				Content.SetContent(Content.RowCount - 1, 0, pagination);
				Content.SetColumnSpan(Content.ColumnCount, pagination);
			}
		}

		private void PageNumbers_ValueChanged(object sender, string e)
		{
			int from = DataSource.Limit.Count * (int.Parse(((IListPicker) sender).Value) - 1);
			int to = from + DataSource.Limit.Count;

			DataSource.Limit = new SelectLimit(from, to);
			DataBind();
		}

		private void PageSizeOptions_ValueChanged(object sender, string e)
		{
			DataSource.Limit = new SelectLimit(0, int.Parse(((IListPicker) sender).Value));
			DataBind();
		}

		private void Header_Click(object sender, EventArgs e)
		{
			DataMember member = DataSource.Members.Where(mi => ((ILabelButton) sender).Text == Translator.Translate(mi.DataMember.Member.FinalMemberInfo)).Select(mi => mi.DataMember).Single();
			OrderBy order = DataSource.OrderBy.Where(ob => ob.Member == member).SingleOrDefault();

			DataSource.OrderBy.Clear();

			if (order != null)
			{
				if (order.Direction == Data.SortDirection.Ascending)
				{
					order.Direction = Data.SortDirection.Descending;
				}
				else
				{
					order.Direction = Data.SortDirection.Ascending;
				}
			}
			else
			{
				order = new OrderBy(member, Data.SortDirection.Ascending);
				DataSource.OrderBy.Add(order);
			}

			DataBind();
		}
	}
}
using System.Linq;
using System.Collections.Generic;
using OKHOSTING.Data.Validation;
using OKHOSTING.ORM;
using OKHOSTING.ORM.Filters;

namespace OKHOSTING.Authentication
{
	/// <summary>
	/// Represents a permission that a user or a group has on certain DataType.
	/// This permissions are made effective automatically before every DataBase operation by the User class.
	/// Web pages and controls must implement these permissions manually
	/// </summary>
	public class DbPermission
	{
		/// <summary>
		/// Unique ID of this permission
		/// </summary>
		public int Id
		{
			get; set;
		}

		/// <summary>
		/// DataType which the user or group has permission to acess
		/// </summary>
		[RequiredValidator]
		public DataType DataType
		{
			get; set;
		}

		/// <summary>
		/// Filter that restricts the DataObjects which the user or group has acces to.
		/// Can contain multiple inner filters for advanced filtering.
		/// </summary>
		public List<FilterBase> Filter
		{
			get; set;
		}

		/// <summary>
		/// Wether or not Insert operations are allowd
		/// </summary>
		[RequiredValidator]
		public bool Insert
		{
			get; set;
		}

		/// <summary>
		/// Wether or not Update operations are allowd
		/// </summary>
		[RequiredValidator]
		public bool Update
		{
			get; set;
		}

		/// <summary>
		/// Wether or not Delete operations are allowd
		/// </summary>
		[RequiredValidator]
		public bool Delete
		{
			get; set;
		}

		/// <summary>
		/// Wether or not Select operations are allowd
		/// </summary>
		[RequiredValidator]
		public bool Select
		{
			get; set;
		}

		/// <summary>
		/// List of DataMembers that are read only for the user or group. 
		/// User or group have read-only acces to this members.
		/// This setting only applies for DataValues in select and update operations. 
		/// Insert and delete operations ignore this setting.
		/// This only apply for DataValues.
		/// </summary>
		/// <example>Balance, CreditRating, Priority</example>
		public List<DataMember> ReadOnlyMembers
		{
			get; set;
		}

		/// <summary>
		/// List of DataMembers that are restricted to this user or group.
		/// User or group does not have acces to this members.
		/// This setting applies for all DataMembers
		/// </summary>
		/// <example>Balance, CreditRating, Priority</example>
		public List<DataMember> RestrictedMembers
		{
			get; set;
		}

		/// <summary>
		/// Gets a parsed list of DataValues that the user has permission to acces (including readonly values)
		/// </summary>
		public IEnumerable<DataMember> AllowedMembers
		{
			get
            {
				return DataType.AllDataMembers.Except(RestrictedMembers);
			}
		}
	}
}
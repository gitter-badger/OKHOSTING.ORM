using System.Collections.Generic;
using OKHOSTING.Data.Validation;

namespace OKHOSTING.Authentication
{
	/// <summary>
	/// Represents a DataType and a list of DataMembers that a user group have acces to
	/// </summary>
	public class DbRolePermission : DbPermission
	{
		/// <summary>
		/// Group which this permission is assigned to
		/// </summary>
		[RequiredValidator]
		public Role Role
		{
			get; set;
		}

		///// <summary>
		///// Returns the initial collection of users that should be created on system setup
		///// </summary>
		//public static IEnumerable<DbRolePermission> GetSetupDataObjects()
		//{
		//	DbRolePermission perm;

		//	//give all users acces to select some basic DataTypes

		//	perm = new DbRolePermission();
		//	perm.DataType = typeof(Country);
		//	perm.Select = true;
		//	perm.Role = Role.AllUsers;
		//	perms.Add(perm);

		//	perm = new DbRolePermission();
		//	perm.DataType = typeof(Globalization.Culture);
		//	perm.Select = true;
		//	perm.Role = Role.AllUsers;
		//	perms.Add(perm);

		//	perm = new DbRolePermission();
		//	perm.DataType = typeof(Globalization.LocalizedDictionaryItem);
		//	perm.Select = true;
		//	perm.Role = Role.AllUsers;
		//	perms.Add(perm);

		//	return perms;
		//}
	}
}
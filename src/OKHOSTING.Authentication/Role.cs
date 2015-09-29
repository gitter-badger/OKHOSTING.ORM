using OKHOSTING.Data.Validation;

namespace OKHOSTING.Authentication
{
	/// <summary>
	/// A user group in the system, allos to group users for easies acces permission management
	/// </summary>
	public class Role
	{
		/// <summary>
		/// Unique id
		/// </summary>
		public int Id;

		/// <summary>
		/// Name of the group
		/// </summary>
		/// <example>Administrators, customers, employees</example>
		[RequiredValidator]
		[StringLengthValidator(50)]
		public string Name;

		/// <summary>
		/// Description of this group
		/// </summary>
		public string Description;

		/// <summary>
		/// Returns the name of this group
		/// </summary>
		public override string ToString()
		{
			return this.Name;
		}

		///// <summary>
		///// Returns the initial collection of groups that should be created on system setup
		///// </summary>
		//public static DataObjectCollection GetSetupDataObjects()
		//{
		//	Administrators.Name = "Administrators";
		//	Administrators.Description = "Administrative users who can acces all data without restrictions";

		//	AllUsers.Name = "All Users";
		//	AllUsers.Description = "Represents all users in the system"; 
			
		//	return new DataObjectCollection(typeof(Role)) { Role.Administrators, Role.AllUsers };
		//}

		/// <summary>
		/// Administrators group that have acaces to all data, permissions are not checked for this group
		/// </summary>
		public static readonly Role Administrators;

		/// <summary>
		/// Represents all users in the system
		/// </summary>
		public static readonly Role AllUsers;
	}
}
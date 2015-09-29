using System;
using System.Collections.Generic;
using OkHosting.Core;
using OkHosting.Softosis.Core.Globalization;
using OkHosting.Softosis;
using OkHosting.Softosis.Filters;
using OKHOSTING.Data.Validation;
using OkHosting.Softosis.UI;

namespace OKHOSTING.Authentication
{
	/// <summary>
	/// A user that belongs to a group. A user may belong to one or more groups
	/// </summary>
	public class RoleSubscription
	{
		/// <summary>
		/// The user that belongs to a group
		/// </summary>
		public User User
		{
			get; set;
		}

		/// <summary>
		/// The group which the user belongs to
		/// </summary>
		public Role Role
		{
			get; set;
		}

		///// <summary>
		///// Returns the initial collection of users that should be created on system setup
		///// </summary>
		//public static DataObjectCollection GetSetupDataObjects()
		//{
		//	RoleSubscription administrators = new RoleSubscription();
		//	administrators.User = User.Administrator;
		//	administrators.Role = Role.Administrators;

		//	return new DataObjectCollection(typeof(RoleSubscription)) { administrators };
		//}
	}
}

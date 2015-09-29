using System;
using System.Collections.Generic;
using OKHOSTING.Data.Validation;
using OKHOSTING.Sql.Schema;
using OKHOSTING.ORM;

namespace OKHOSTING.Authentication
{
	/// <summary>
	/// A system user, a person who can access the system and it's data with a username and password
	/// </summary>
	public class User
	{
		#region Members

		/// <summary>
		/// Unique Id
		/// </summary>
		public int Id
		{
			get; set;
		}

		/// <summary>
		/// User name which will be used for login by this user
		/// </summary>
		/// <example>administrator, david, oneuser445</example>
		[RequiredValidator]
		[StringLengthValidator(5, 50)]
		[RegexValidator(@"[a-z0-9]+")]
		public string UserName
		{
			get; set;
		}

		/// <summary>
		/// Password of the user
		/// </summary>
		[RequiredValidator]
		[StringLengthValidator(8, 100)]
		//[Password]
		public string Password
		{
			get; set;
		}

		/// <summary>
		/// Secret question, usefull when the user forgets the password
		/// </summary>
		[StringLengthValidator(50)]
		[RequiredValidator]
		public string SecretQuestion
		{
			get; set;
		}

		/// <summary>
		/// Secret answer, usefull when the user forgets the password
		/// </summary>
		[StringLengthValidator(50)]
		[RequiredValidator]
		//[Password]
		public string SecretAnswer
		{
			get; set;
		}

		[StringLengthValidator(100)]
		[RegexValidator(OKHOSTING.Core.RegexPatterns.EmailAddress)]
		public string Email
		{
			get; set;
		}

		/// <summary>
		/// Gets a value indicating wether the current user is or not part of Administrator's group
		/// </summary>
		public bool IsAdministrator
		{
			get
			{
				foreach (Role g in Roles)
				{
					if (g.Equals(Role.Administrators)) return true;
				}
				
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating wether the current user is or not anonimous
		/// </summary>
		public bool IsAnonimous
		{
			get
			{
				return this.Equals(User.Anonimous);
			}
		}

		/// <summary>
		/// Groups that the user belongs to
		/// </summary>
		public List<Role> Roles
		{
			get; set;
		}

		#endregion

		#region DataBase Permissions

		/// <summary>
		/// Allows child classes to create DataBase permissions in run-time.
		/// Child classes can override this method to provide custom DataBase permissions.
		/// </summary>
		/// <remarks>
		/// This list can override group and user permissions
		/// This list is NOT stored in the DataBase, it must be filled at run-time.
		/// </remarks>
		protected virtual DataObjectCollection CustomDbPermissions
		{
			get
			{
				DataObjectCollection permissions = new DataObjectCollection(typeof(DbPermission));
				return permissions;
			}
		}

		/// <summary>
		/// All DataBase permissions that must be applied to the current user, including user, group and custom permissions
		/// </summary>
		public DataObjectCollection EffectiveDbPermissions;

		/// <summary>
		/// Returns the permission that the current User can access for the indicated operation
		/// </summary>
		public DbPermission GetDbPermissionFor(DataType dtype)
		{
			//if user is administrator, do not check permissions and create all inclusive permission
			if (this.IsAdministrator)
			{
				DbPermission perm = new DbPermission();
				perm.DataType = dtype;
				perm.Insert = perm.Update = perm.Delete = perm.Select = true;
				perm.Filter = null;
				return perm;
			}

			//Search in permissions list
			foreach (DbPermission perm in this.EffectiveDbPermissions)
			{
				//If it's not the same datatype, continue searching
				if (!perm.DataType.Equals(dtype)) continue;

				//finally, if acces is for the current datatype, return current permission
				return perm;
			}

			//finally return null
			return null;
		}

		/// <summary>
		/// Returns the permission that the current User can access for the indicated operation
		/// </summary>
		public DbPermission GetDbPermissionFor(DataObject dobj)
		{
			DbPermission perm;

			//if user is administrator, do not check permissions and create all inclusive permission
			if (this.IsAdministrator)
			{
				perm = new DbPermission();
				perm.DataType = dobj.DataType;
				perm.Insert = perm.Update = perm.Delete = perm.Select = true;
				perm.Filter = null;
				return perm;
			}

			//Search in permissions list
			perm = GetDbPermissionFor(dobj.DataType);
			
			//permission not found
			if (perm == null) return null;

			//if filter doesn't match, continue
			if (perm.Filter != null && !perm.Filter.Match(dobj)) return null;

			//finally return permission
			return perm;
		}

		/// <summary>
		/// Returns a bool value indicating if the current User has permission to perform the indicated operation on the indicated DataType
		/// </summary>
		public bool HasDbPermissionTo(DataBaseOperation operation, DataType dtype)
		{
			//look for permission on this dataType
			DbPermission perm = GetDbPermissionFor(dtype);

			//if no permissions is found, return false
			if (perm == null) return false;

			//return permission for this operation
			switch (operation)
			{
				case DataBaseOperation.Insert:
					return perm.Insert;

				case DataBaseOperation.Update:
					return perm.Update;

				case DataBaseOperation.Delete:
					return perm.Delete;

				case DataBaseOperation.Select:
					return perm.Select;
			}

			//if operation is not one of the preioes, return false
			return false;
		}

		/// <summary>
		/// Returns a bool value indicating if the current User has permission to perform the indicated operation on the indicated Dataobject
		/// </summary>
		public bool HasDbPermissionTo(DataBaseOperation operation, DataObject dobj)
		{
			//look for permission on this dataType
			DbPermission perm = GetDbPermissionFor(dobj);

			//if no permissions is found, return false
			if (perm == null) return false;

			//return permission for this operation
			switch (operation)
			{
				case DataBaseOperation.Insert:
					return perm.Insert;

				case DataBaseOperation.Update:
					return perm.Update;

				case DataBaseOperation.Delete:
					return perm.Delete;

				case DataBaseOperation.Select:
					return perm.Select;
			}

			//if operation is not one of the preioes, return false
			return false;
		}

		/// <summary>
		/// Returns a bool value indicating if the current User has permission to perform the indicated operation on the indicated DataType
		/// </summary>
		public bool HasDbPermissionTo(DataMember dmember)
		{
			//look for permission on this dataType
			DbPermission perm = GetDbPermissionFor(dmember.DataType);

			//if no permissions is found, return false
			if (perm == null) return false;

			//search in restricted members collection
			if (perm.RestrictedMembers.Contains(dmember)) return false;

			//if nothing was found, return true
			return true;
		}

		/// <summary>
		/// Returns a bool value indicating if the current User has permission to perform the indicated operation on the indicated DataType
		/// </summary>
		public bool HasDbReadOnlyPermissionTo(DataMember dmember)
		{
			//look for permission on this dataType
			DbPermission perm = GetDbPermissionFor(dmember.DataType);

			//if no permissions is found, return false
			if (perm == null) return false;

			//search in restricted members collection
			if (perm.RestrictedMembers.Contains(dmember)) return false;

			//search in readonly members collection
			if (perm.ReadOnlyMembers.Contains(dmember)) return true;
			
			//if nothing was found, return false
			return false;
		}

		/// <summary>
		/// Adds a new permission to the effective permissions collection.
		/// </summary>
		/// <param name="permission">DbPermission to be added</param>
		/// <remarks>
		/// If a permission with the same AllowedDataType acces is found, 
		/// it is deleted and overriden by this new permission
		/// </remarks>
		public void AddDbEffectivePermission(DbPermission permission)
		{
			//search for a permission with the same AllowedDataType
			//and if it is found, delete it so it can be overriden
			//with this new permission
			foreach (DbPermission p in EffectiveDbPermissions)
			{
				if (p.DataType.Equals(permission.DataType))
				{
					EffectiveDbPermissions.Remove(p);
					break;
				}
			}

			//finally add permission
			EffectiveDbPermissions.Add(permission);
		}

		/// <summary>
		/// Adds a list of permissions to the effective permissions collection.
		/// </summary>
		/// <param name="permissions">Collection of DbPermission to be added</param>
		/// <remarks>
		/// If a permission with the same AllowedDataType acces is found, 
		/// it is deleted and overriden by this new permission
		/// </remarks>
		public void AddDbEffectivePermissions(DataObjectCollection permissions)
		{
			foreach (DbPermission p in permissions)
			{
				AddDbEffectivePermission(p);
			}
		}

		#endregion

		#region Methods and event handling

		/// <summary>
		/// Returns the current instance's UserName
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return UserName;
		}

		/// <summary>
		/// Restricts every write operation on the database by user's acces permissions
		/// </summary>
		void DataBase_BeforeOperation(DataBase sender, DataBaseOperationEventArgs e)
		{
			//if  IgnoreSecurity is set to true, exit
			if (DisableSecurity) return;

			//if user doesn't have permission, trhow exception
			if (!HasDbPermissionTo(e.Operation, e.DataObject))
			{
				throw new DbSecurityException(this, e.DataObject.DataType, e.Operation, "User does not have permission to perform this operation");
			}

			//get permission details
			DbPermission perm = GetDbPermissionFor(e.DataObject);

			//check which DataMembers are restricted (only applies to UPDATE and SELECT operations)
			if (e.Operation == DataBaseOperation.Select || e.Operation == DataBaseOperation.Update)
			{
				//get restricted members
				List<DataMember> restricted = perm.GetRestrictedMembers();

				foreach (DataValue dv in e.AffectedValues)
				{
					if (restricted.Contains(dv))
					{
						throw new DbSecurityException(this, e.DataObject.DataType, e.Operation, "User does not have permission to perform this operation on DataMember '" + dv.Name + "'");
					}
				}
			}

			//check which DataValues are readonly (only applies to UPDATE operations)
			if (e.Operation == DataBaseOperation.Update)
			{
				//get restricted members
				List<DataValue> readOnly = perm.GetReadOnlyValues(); 
				
				foreach (DataValue dv in e.AffectedValues)
				{
					if (readOnly.Contains(dv))
					{
						throw new DbSecurityException(this, e.DataObject.DataType, e.Operation, "User does not have permission to perform this operation on DataMember '" + dv.Name + "'");
					}
				}
			}
		}

		/// <summary>
		/// Restricts every select operation on the database by user's acces permissions
		/// </summary>
		void DataBase_BeforeSelect(DataBase sender, BeforeSelectEventArgs e)
		{
			//if  IgnoreSecurity is set to true, exit
			if (DisableSecurity) return;

			//if user doesn't have permission, trhow exception
			if (!HasDbPermissionTo(DataBaseOperation.Select, e.DataType))
			{
				throw new DbSecurityException(this, e.DataType, DataBaseOperation.Select, "User does not have permission to perform this operation");
			}

			//get permission details
			DbPermission perm = GetDbPermissionFor(e.DataType);

			//get restricted members
			List<DataMember> restricted = perm.GetRestrictedMembers();

			//check which DataMembers are restricted (only applies to UPDATE and SELECT operations)
			foreach (DataValue dv in e.SelectedValues)
			{
				if (restricted.Contains(dv))
				{
					throw new DbSecurityException(this, e.DataType, DataBaseOperation.Select, "User does not have permission to perform this operation on DataMember '" + dv.Name + "'");
				}
			}
		}

		/// <summary>
		/// Runs after the user logs in
		/// </summary>
		protected virtual void OnLogin()
		{
			//Temporally disable authentication to get user info
			bool isSecurityDisabled = User.DisableSecurity;
			DisableSecurity = true;

			#region Load roles from database and add

			//load group subscriptions
			this.Roles = new DataObjectCollection();
			DataObjectCollection subscriptions = DataBase.Current.SelectByForeignKey((DataValue)DataType.From(typeof(RoleSubscription))["User"], this);

			//load all roles
			foreach (RoleSubscription subscription in subscriptions)
			{
				subscription.Role.Select();
				this.Roles.Add(subscription.Role);
			}

			//add AllUsers role and permissions
			if (!this.Roles.Contains(Role.AllUsers)) this.Roles.Add(Role.AllUsers);

			#endregion

			#region Load DataBase effective permissions

			//initialize all efective permissions
			EffectiveDbPermissions = new DataObjectCollection(typeof(DbPermission));

			//add role permissions 
			//role permissions are asigned first so they can be overriden by user and custom permissions
			foreach (Role role in this.Roles)
			{
				AddDbEffectivePermissions(DataBase.Current.SelectByForeignKey((DataValue)DataType.From(typeof(DbRolePermission))["Role"], role));
			}

			//add user permissions
			//User permissions are asigned second so they override role permissions
			AddDbEffectivePermissions(DataBase.Current.SelectByForeignKey((DataValue)DataType.From(typeof(DbUserPermission))["User"], this));

			//add custom permissions
			//custom permissions are asigned first so they override all other permissions
			AddDbEffectivePermissions(this.CustomDbPermissions);

			#endregion

			#region Subscribe to DataBase events for security checking

			//avoid duplicate subscriptions
			DataBase.Current.BeforeSelect -= DataBase_BeforeSelect;
			DataBase.Current.BeforeInsert -= DataBase_BeforeOperation;
			DataBase.Current.BeforeUpdate -= DataBase_BeforeOperation;
			DataBase.Current.BeforeDelete -= DataBase_BeforeOperation;

			//Sign in for database events, to restrict operations by this user's permissions
			DataBase.Current.BeforeSelect += new DataBase.BeforeSelectEventHandler(this.DataBase_BeforeSelect);
			DataBase.Current.BeforeInsert += new DataBase.OperationEventHandler(this.DataBase_BeforeOperation);
			DataBase.Current.BeforeUpdate += new DataBase.OperationEventHandler(this.DataBase_BeforeOperation);
			DataBase.Current.BeforeDelete += new DataBase.OperationEventHandler(this.DataBase_BeforeOperation);

			#endregion

			//re-enable security (if it was enabled at the beginning)
			DisableSecurity = isSecurityDisabled;
		}

		/// <summary>
		/// Runs after the user logs in
		/// </summary>
		protected virtual void OnLogout()
		{
			//de-subscribe from database events
			try
			{
				DataBase.Current.BeforeSelect -= this.DataBase_BeforeSelect;
				DataBase.Current.BeforeInsert -= this.DataBase_BeforeOperation;
				DataBase.Current.BeforeUpdate -= this.DataBase_BeforeOperation;
				DataBase.Current.BeforeDelete -= this.DataBase_BeforeOperation;
			}
			catch { }
		}

		#endregion

		#region Static

		/// <summary>
		/// If set to true, ignores all security and gives the user acces to all data. Usefull to perform system operations
		/// IMPORTANT: if you set this to true, remember to set it back to false once your operation is finished, otherwise you will give full access to the current user
		/// </summary>
		public static bool DisableSecurity = false;

		/// <summary>
		/// Logs in anonimous user
		/// </summary>
		private static void LoginAnonimous()
		{
			Login("anonimous", "123456");
		}

		/// <summary>
		/// Starts the security engine by calling User.StartSecutiry()
		/// </summary>
		public static void PlugIn_OnSessionStart()
		{
			StartSecutiry();
		}

		/// <summary>
		/// Starts monitoring the current DataBase (and Web pages if this is a web application) 
		/// and restricting operations using the current user permissions
		/// </summary>
		/// <remarks>
		/// If this method is not called, no security checks are performed
		/// and users get all acces to application and database
		/// </remarks>
		public static void StartSecutiry()
		{
			//if security is already started, do nothing and exit
			if (IsSecurityStarted) return;

			//login as sanonimous so anonimous user subscribes to database and web events and
			//restricts acces by this user
			LoginAnonimous();
		}

		/// <summary>
		/// Stops monitoring the current DataBase (and Web pages if this is a web application) 
		/// and allows full acces to everyone to all resources
		/// </summary>
		public static void StopSecutiry()
		{
			//simply logout to desubscribe from database and web events
			if (User.Current != null)
			{
				User.Current.OnLogout();
				Session.Current.Remove("User");
			}
		}

		/// <summary>
		/// Gets a value indicateing if security restrictions are running right now
		/// </summary>
		public static bool IsSecurityStarted
		{
			get
			{
				return User.Current != null;
			}
		}

		/// <summary>
		/// Tries to authenticate in the DataBase with the given credentials. If authentication is succesfull, User.Current is set with the authenticated user
		/// </summary>
		/// <returns>True if authentication was succesfull and User.Current was set, false otherwise</returns>
		public static bool Login(string username, string password)
		{
			User user;
			DataType dtype;
			ValueCompareFilter filter;
			DataObjectCollection result;

			//validate arguments
			if (username == null) throw new ArgumentNullException("username", "Argument can not be null");
			if (password == null) throw new ArgumentNullException("password", "Argument can not be null");

			//Temporally disable authentication to get user info
			bool isSecurityDisabled = User.DisableSecurity;
			DisableSecurity = true;

			//close previous sessions
			if (User.Current != null)
			{
				User.Current.OnLogout();
				Session.Current.Remove("User");
			}

			#region Authenticate

			//Create filter by username
			dtype = typeof(User);
			filter = new ValueCompareFilter();
			filter.DataValue = (DataValue) dtype["UserName"];
			filter.Operator = CompareOperator.Equal;
			filter.ValueToCompare = username;

			//Search database
			result = DataBase.Current.Select(dtype, new FilterCollection(filter));

			//If there is not a result, authentication failed
			if (result.Count == 0)
			{
				Log.Write("User.Authenticate", "Authentication failed. Username not found: '" + username + "'", Log.LogType_Security);
				return false;
			}

			user = (User) result[0];
			
			//Compare passwords
			if (user.Password != password)
			{
				Log.Write("User.Authenticate", "Authentication failed. Username: '" + username + "' Wrong Password: '" + password + "'", Log.LogType_Security);
				return false;
			}

			//Setting values to null while object is in memory, for security
			user.Password = user.SecretQuestion = user.SecretAnswer = null;

			#endregion

			//Look for child DataObjects
			DataObjectCollection childs = DataBase.Current.SearchInheritedFrom(user);
			if (childs.Count > 0)
			{
				user = (User) childs[0];
			}

			//Assign current user
			Session.Current["User"] = user;

			//set culture
			if (!NullValues.IsNull(user.BelongsTo))
			{
				user.BelongsTo.Select();
				user.BelongsTo.Culture.Select();
				Culture.Current = user.BelongsTo.Culture;
			}

			//insert session object only if user is not anonimous
			if (!user.Equals(User.Anonimous))
			{
				UserSession session = new UserSession();
				session.User = user;
				session.StartDate = DateTime.Now;
				session.EndDate = NullValues.DateTime;
				session.Insert();
			}

			//re-enable security (if it was enabled at the beginning)
			DisableSecurity = isSecurityDisabled;
			
			//allow user to perform custom actions
			user.OnLogin();

			return true;
		}

		/// <summary>
		/// Logs out the current user and logs in as anonimous user
		/// </summary>
		/// <remarks>Does not stops the security engine. If you want to stop security at all you must call StopSecurity()</remarks>
		public static void Logout()
		{
			//if user is not anonimous, perform logout operations
			if (User.Current != null && !User.Current.Equals(User.Anonimous))
			{
				User.Current.OnLogout();
				Session.Current.Remove("User");

				//search for open UserSessions
				DataType dtype = typeof(UserSession);
				DataObjectCollection openSessions;
				
				//search where EndDate == null
				ValueCompareFilter endDateFilter = new ValueCompareFilter();
				endDateFilter.DataValue = (DataValue)dtype["EndDate"];
				endDateFilter.Operator = CompareOperator.Equal;
				endDateFilter.ValueToCompare = NullValues.DateTime;

				//search where User == User.Current
				ForeignKeyFilter userFilter = new ForeignKeyFilter();
				userFilter.DataValue = (DataValue)dtype["User"];
				userFilter.ValueToCompare = User.Current;

				//do search
				openSessions = DataBase.Current.Select(dtype, new FilterCollection(endDateFilter, userFilter));

				//close open sessions
				foreach (UserSession s in openSessions)
				{
					s.EndDate = DateTime.Now;
					s.Update();
				}
			}

			LoginAnonimous();
		}

		/// <summary>
		/// Registers a new user into the database and logs in as this user
		/// </summary>
		/// <param name="user">User to register in the database</param>
		public static void Register(User user)
		{
			//disable security
			bool isSecurityDisabled = DisableSecurity;
			DisableSecurity = true;

            //insert into database
            DataBase db = DataBase.Default
            DataBase.Current.Insert(user);

			//login
			Login(user.UserName, user.Password);

			//re-enable security (if it was enabled at the beginning)
			DisableSecurity = isSecurityDisabled;
		}

		/// <summary>
		/// Gets the current session's user
		/// </summary>
		public static User Current
		{
			get
			{
				if (Session.Current.ContainsKey("User"))
				{
					//return current user
					return (User)Session.Current["User"];
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Administrator user with full acces.
		/// </summary>
		public static readonly User Administrator;

		/// <summary>
		/// Anonimous user with limited acces. 
		/// This is the default user if no user is authenticated in the system
		/// </summary>
		public static readonly User Anonimous;

		/// <summary>
		/// Returns the initial collection of users that should be created on system setup
		/// </summary>
		public static IEnumerable<User> GetSetupDataObjects()
		{
			//create administrator and anonimous users on startup

			Administrator.UserName = "administrator";
			Administrator.Password = "123456";
			Administrator.SecretQuestion = "secret question";
			Administrator.SecretAnswer = "secret answer";

			Anonimous.UserName = "anonimous";
			Anonimous.Password = "123456";
			Anonimous.SecretQuestion = "secret question";
			Anonimous.SecretAnswer = "secret answer";

            yield return Administrator;
            yield return Anonimous;
		}

		/// <summary>
		/// Initiates static properties
		/// </summary>
		static User()
		{
			Administrator = new User();
			Administrator.Id = 1;
			try
			{
				Administrator.Select();
			}
			catch { }

			Anonimous = new User();
			Anonimous.Id = 2;
			try
			{
				Anonimous.Select();
			}
			catch { }
		}

		#endregion
	}
}
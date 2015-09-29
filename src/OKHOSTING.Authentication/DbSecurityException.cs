using System;
using OKHOSTING.Sql.Schema;

namespace OKHOSTING.Authentication
{
	/// <summary>
	/// Exception that is thrown when a database security violation is performed
	/// </summary>
	public class DbSecurityException : Exception
	{
		/// <summary>
		/// User that performs the security violation
		/// </summary>
		public readonly User User;

		/// <summary>
		/// DataType that was being accesed by the user
		/// </summary>
		public readonly DataType DataType;

		/// <summary>
		/// Operation the user was trying to perform
		/// </summary>
		public readonly DataBaseOperation Operation;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="user">User that performs the security violation</param>
		/// <param name="dtype">DataType that was being accesed by the user</param>
		/// <param name="operation">Operation the user was trying to perform</param>
		public DbSecurityException(User user, DataType dtype, DataBaseOperation operation)
		{
			this.User = user;
			this.DataType = dtype;
			this.Operation = operation;
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="user">User that performs the security violation</param>
		/// <param name="dtype">DataType that was being accesed by the user</param>
		/// <param name="operation">Operation the user was trying to perform</param>
		/// <param name="message">Custom message for this exception</param>
		public DbSecurityException(User user, DataType dtype, DataBaseOperation operation, string message): base(message)
		{
			this.User = user;
			this.DataType = dtype;
			this.Operation = operation;
		}

		public override string Message
		{
			get
			{
				return
					base.Message +
					". " +
					"User : " + User +
					", " +
					"DataType : " + DataType +
					", " +
					"Operation : " + Operation;
			}
		}
	}
}
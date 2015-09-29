using System;
using OKHOSTING.Data.Validation;

namespace OKHOSTING.Authentication
{
	/// <summary>
	/// A session when a user logs into the system
	/// </summary>
	public class UserSession
	{
		/// <summary>
		/// Unique Id
		/// </summary>
		public int Id
		{
			get; set;
		}

		/// <summary>
		/// User that started the session
		/// </summary>
		[RequiredValidator]
		public User User
		{
			get; set;
		}

		/// <summary>
		/// Date and time when the session started
		/// </summary>
		[RequiredValidator]
		public DateTime StartDate
		{
			get; set;
		}

		/// <summary>
		/// Date and time when the session ended
		/// </summary>
		public DateTime EndDate
		{
			get; set;
		}
	}
}
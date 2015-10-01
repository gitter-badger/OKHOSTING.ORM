using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Tests.Model
{
	public class Person
	{
		public int Id;
		
		[OKHOSTING.Data.Validation.StringLengthValidator(100)]
		public string Firstname;

		[OKHOSTING.Data.Validation.StringLengthValidator(100)]
		public string LastName;

		public DateTime BirthDate { get; set; }
		
		public Address Address1 { get; set; }
		public Address Address2 { get; set; }

		public string FullName
		{
			get
			{
				return Firstname + " " + LastName;
			}
		}
	}
}
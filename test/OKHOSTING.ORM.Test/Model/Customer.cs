using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Tests.Model
{
	public class Customer
	{
		public int Id;

		[OKHOSTING.Data.Validation.StringLengthValidator(100)]
		public string LegalName { get; set; }

		[OKHOSTING.Data.Validation.StringLengthValidator(100)]
		public string Phone { get; set; }
		
		public string Email { get; set; }
	}
}
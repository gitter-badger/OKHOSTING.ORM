using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Tests.Model
{
	public class Address
	{
		public int Id;
		
		[OKHOSTING.Data.Validation.StringLengthValidator(100)]
		public string Street;

		[OKHOSTING.Data.Validation.StringLengthValidator(100)]
		public string Number;
		
		public Country Country { get; set; }
	}
}
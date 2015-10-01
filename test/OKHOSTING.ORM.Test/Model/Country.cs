using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Tests.Model
{
	public class Country
	{
		public int Id;
		
		[OKHOSTING.Data.Validation.StringLengthValidator(50)]
		public string Name;
	}
}
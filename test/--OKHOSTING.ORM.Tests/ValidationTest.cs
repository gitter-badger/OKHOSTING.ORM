using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using OKHOSTING.Data.Validation;
using OKHOSTING.ORM;

namespace OKHOSTING.Test111
{
	[TestFixture]
	public class ValidationTest
	{
		[Test]
		public void DataTypeTest()
		{
			var dtype = new DataType<OKHOSTING.ORM.Tests.Model.Person>();
			dtype.Validators.Add(new StringLengthValidator() { Member = dtype[m => m.Firstname].Member, MaxLength = 100 });
		}
	}
}
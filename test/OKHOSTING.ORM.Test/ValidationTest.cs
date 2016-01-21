using System;
using System.Linq;
using System.Collections.Generic;
using OKHOSTING.Data.Validation;
using OKHOSTING.ORM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OKHOSTING.ORM.Test
{
	[TestClass]
	public class ValidationTest
	{
		[TestMethod]
		public void DataTypeTest()
		{
			var dtype = new DataType<OKHOSTING.ORM.Tests.Model.Person>();
			dtype.Validators.Add(new MemberValidator(dtype[m => m.Firstname].Member, new StringLengthValidator(100)));
		}
	}
}
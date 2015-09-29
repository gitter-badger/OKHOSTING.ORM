using System;
using System.Collections.Generic;

namespace OKHOSTING.ORM.Operations
{
    /// <summary>
    /// Base class for all operations
    /// </summary>
    public abstract class Operation
    {
        /// <summary>
        /// The datatype that will be affected by the oepration
        /// </summary>
		public DataType DataType { get; set; }
    }
}
namespace OKHOSTING.ORM.Operations
{
	/// <summary>
	/// Argument used for allow to the users of database executors 
	/// custom the process of execution of sentecences, DataTables
	/// and DataReaders creation
	/// </summary>
	public class OperationEventArgs : System.EventArgs
	{
		/// <summary>
		/// Script that is being executed
		/// </summary>
		public readonly Operation Operation;

		/// <summary>
		/// Indicates if the execution of the script must be canceled (Only works for OnBefore events)
		/// </summary>
		public bool Cancel = false;

		/// <summary>
		/// Contains the result of the operation. Can be an Int32, a DataReader or a DataTable, depending on the operation performed
		/// </summary>
		public object Result;

		/// <summary>
		/// Construct the argument
		/// </summary>
		/// <param name="script">
		/// Script that was or is about to be executed
		/// </param>
		public OperationEventArgs(Operation operation)
		{
			this.Operation = operation;
		}
	}
}
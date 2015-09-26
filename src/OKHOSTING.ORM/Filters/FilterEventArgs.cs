using System;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Argument for custom filter event
	/// </summary>
	public class FilterEventArgs: EventArgs
	{
		/// <summary>
		/// Indicates if the filter was successfully approved
		/// </summary>
		public bool Match = false;

		/// <summary>
		/// object in wich the filter must be applied
		/// </summary>
		public readonly object ObjectToFilter;

		/// <summary>
		/// Constructs the argument
		/// </summary>
		/// <param name="objectToFilter">
		/// object in wich the filter must be applied
		/// </param>
		public FilterEventArgs(object objectToFilter)
		{
			this.ObjectToFilter = objectToFilter;
			this.Match = false;
		}
	}
}
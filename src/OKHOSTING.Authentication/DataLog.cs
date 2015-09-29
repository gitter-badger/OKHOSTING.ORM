using System;
using OKHOSTING.Data.Validation;
using System.Xml.Serialization;
using System.IO;

namespace OKHOSTING.Authentication
{
	/// <summary>
	/// A log for a DataBase operation. Writes a log in the DataBase indicating a a DataObject was inserted, updated or deleted
	/// </summary>
	public class DataLog
	{
		/// <summary>
		/// Unique id
		/// </summary>
		public int Id
		{
			get; set;
		}

		/// <summary>
		/// User that performs an operation
		/// </summary>
		public User User
		{
			get; protected set;
		}

		/// <summary>
		/// Date and time of the operation
		/// </summary>
		[RequiredValidator]
		public DateTime Date
		{
			get; protected set;
		}

		/// <summary>
		/// Operation performed by the user
		/// </summary>
		[RequiredValidator]
		public DataBaseOperation Operation
		{
			get; set;
		}

		/// <summary>
		/// DataObject on which the operation was performed
		/// </summary>
		[RequiredValidator]
		public DataObject DataObject
		{
			get; set;
		}

		/// <summary>
		/// A backup of the state of the DataObject before the operation was performed (in XML format)
		/// </summary>
		public string Backup
		{
			get; set;
		}

		/// <summary>
		/// Subscribes to DataBase events so datalogs can be created for every database-write operation
		/// </summary>
		public static void PlugIn_OnSessionStart()
		{
			//avoid duplicate subscriptions
			DataBase.Current.AfterInsert -= DataBase_AfterOperation;
			DataBase.Current.AfterUpdate -= DataBase_AfterOperation;
			DataBase.Current.AfterDelete -= DataBase_AfterOperation;

			DataBase.Current.AfterInsert += new DataBase.OperationEventHandler(DataBase_AfterOperation);
			DataBase.Current.AfterUpdate += new DataBase.OperationEventHandler(DataBase_AfterOperation);
			DataBase.Current.AfterDelete += new DataBase.OperationEventHandler(DataBase_AfterOperation);
		}

		/// <summary>
		/// After any database write operation, a log is written in the database describing the operation
		/// and the current object data. Usefull for versioning of dataobjects and as a backup in case of data loss
		/// </summary>
		static void DataBase_AfterOperation(DataBase sender, DataBaseOperationEventArgs e)
		{
			DataLog log;
			bool isSecurityDisabled = User.DisableSecurity;

			//if object being inserted is a datalog, exit
			if (e.DataObject is DataLog) return;
			
			//validate null values
			if (NullValues.IsNull(e.DataObject)) return;

			//fill up the datalog with the operation info
			log = new DataLog();
			log.DataObject = e.DataObject;
			log.Operation = e.Operation;

			//serialize dataobject as xml to be written in the log
			XmlSerializer serializer = new XmlSerializer(e.DataObject.GetType());
			StringWriter writer = new StringWriter();
			serializer.Serialize(writer, e.DataObject);
			log.Backup = writer.ToString();

			//disable security
			User.DisableSecurity = true;

			//insert datalog into the database
			DataBase.Current.Insert(log);

			//re-enable security (if it was enabled at the beginning)
			User.DisableSecurity = isSecurityDisabled;
		}

		/// <summary>
		/// Sets User and Date before inserting
		/// </summary>
		protected override void OnBeforeInsert()
		{
			User = User.Current;
			Date = DateTime.Now;

			base.OnBeforeInsert();
		}
	}
}
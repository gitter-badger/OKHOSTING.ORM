using OKHOSTING.Core.Data.Validation;
using System.Globalization;

namespace OKHOSTING.Authentication.Localization
{
	/// <summary>
	/// A culture that the system supports
	/// </summary>
	/// <example>Spanish, english, french</example>
	public class Culture
	{
		int id;
		string shortName, name;
		System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.InvariantCulture;

		/// <summary>
		/// Unique id. Should be the same as the LCID of the underlaying System.Globalization.CultureInfo
		/// </summary>
		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;

				try
				{
					cultureInfo = new System.Globalization.CultureInfo(value);
					this.name = cultureInfo.NativeName;
					this.shortName = cultureInfo.Name;
				}
				catch { }
			}
		}

		/// <summary>
		/// Short name of this culture
		/// </summary>
		/// <example>es-MX, en, en-US</example>
		[StringLengthValidator(20)]
		public string ShortName
		{
			get
			{
				return shortName;
			}
			set
			{
				shortName = value;
				try
				{
					cultureInfo = new System.Globalization.CultureInfo(shortName);
				}
				catch { }
			}
		}

		/// <summary>
		/// Name of this culture
		/// </summary>
		/// <example>Spanish, English, English (USA)</example>
		[StringLengthValidator(50)]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		/// <summary>
		/// Underlaying System.Globalization.CultureInfo that provides custom formatting for this culture
		/// </summary>
		public System.Globalization.CultureInfo CultureInfo
		{
			get
			{
				return cultureInfo;
			}
		}

		/// <summary>
		/// Returns the name of the culture
		/// </summary>
		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public Culture()
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="id">Unique id. Should be the same as the LCID of the underlaying System.Globalization.CultureInfo</param>
		public Culture(int id)
		{
			this.Id = id;
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="shortName">Short name of this culture</param>
		public Culture(string shortName)
		{
			this.Id = System.Globalization.CultureInfo.GetCultureInfo(shortName).LCID;
		}

		/// <summary>
		/// Default application culture
		/// </summary>
		public static Culture Default
		{
			get; set;
		}

		/// <summary>
		/// Initializes static DefaultCulture property
		/// </summary>
		static Culture()
		{
			Default = new Culture(System.Globalization.CultureInfo.InvariantCulture.LCID);
		}

		/// <summary>
		/// Returns the initial collection of cultures that should be created on system setup
		/// </summary>
		public static System.Collections.Generic.IEnumerable<Culture> GetSetupObjects()
		{
			foreach (CultureInfo culture in CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures))
			{
				yield return new Culture(culture.LCID);
			}
		}
	}
}
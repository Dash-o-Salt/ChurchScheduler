using System;
using System.Collections.Generic;

namespace ChurchScheduler
{


	public class Person
	{
		private string m_firstName;
		private string m_lastName;
		private RoleTypeHelper.RoleType m_roleType;
		private PreferredTime m_preferredTime;
		private DateTime m_lastServed;
		private string m_homePhone;
		private string m_email;
		private string m_altEmail;
		private string m_address;
		private string m_cityState;
		private string m_zipCode;
		private string m_mailingLabel;
		private string m_status;
		private Dictionary<string, Availability> m_availability = new Dictionary<string, Availability>();

		public virtual string FirstName
		{
			set
			{
				this.m_firstName = value;
			}
			get
			{
				return m_firstName;
			}
		}


		public virtual string LastName
		{
			set
			{
				this.m_lastName = value;
			}
			get
			{
				return m_lastName;
			}
		}


		public virtual RoleTypeHelper.RoleType RoleType
		{
			set
			{
				this.m_roleType = value;
			}
			get
			{
				return m_roleType;
			}
		}


		public virtual PreferredTime PreferredTime
		{
			set
			{
				this.m_preferredTime = value;
			}
			get
			{
				return m_preferredTime;
			}
		}


		public virtual DateTime LastServed
		{
			set
			{
				this.m_lastServed = value;
			}
			get
			{
				return m_lastServed;
			}
		}


		public virtual string HomePhone
		{
			set
			{
				this.m_homePhone = value;
			}
			get
			{
				return m_homePhone;
			}
		}


		public virtual string Email
		{
			set
			{
				this.m_email = value;
			}
			get
			{
				return m_email;
			}
		}


		public virtual string AltEmail
		{
			set
			{
				this.m_altEmail = value;
			}
			get
			{
				return m_altEmail;
			}
		}


		public virtual string Address
		{
			set
			{
				this.m_address = value;
			}
			get
			{
				return m_address;
			}
		}


		public virtual string CityState
		{
			set
			{
				this.m_cityState = value;
			}
			get
			{
				return m_cityState;
			}
		}


		public virtual string ZipCode
		{
			set
			{
				this.m_zipCode = value;
			}
			get
			{
				return m_zipCode;
			}
		}


		public virtual string MailingLabel
		{
			set
			{
				this.m_mailingLabel = value;
			}
			get
			{
				return m_mailingLabel;
			}
		}


		public virtual Dictionary<string, Availability> Availability
		{
			set
			{
				this.m_availability = value;
			}
			get
			{
				return m_availability;
			}
		}


		public virtual string Status
		{
			get
			{
				return m_status;
			}
			set
			{
				m_status = value;
			}
		}

	}

}
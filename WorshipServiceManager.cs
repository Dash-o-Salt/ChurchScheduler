using System.Collections.Generic;
using System.Text;

namespace ChurchScheduler
{

	using RoleType = RoleTypeHelper.RoleType;

	public class WorshipServiceManager
	{
		private static WorshipService m_eight = new WorshipService(WorshipService.WorshipTime.Eight);
		private static WorshipService m_nineThirty = new WorshipService(WorshipService.WorshipTime.NineThirty);
		private static WorshipService m_eleven = new WorshipService(WorshipService.WorshipTime.Eleven);
		private static string m_errors = "";

		public static void LoadWorshipService(string date, List<Person> people)
		{
			//Clear out any old worship service we have loaded
			m_eight = new WorshipService(WorshipService.WorshipTime.Eight);
			m_nineThirty = new WorshipService(WorshipService.WorshipTime.NineThirty);
			m_eleven = new WorshipService(WorshipService.WorshipTime.Eleven);
			m_errors = "";

			foreach (Person person in people)
			{
				foreach (string availableDate in person.Availability.Keys)
				{
					if (availableDate.Equals(date))
					{
						if (person.Availability[availableDate] == Availability.ScheduledAtEight)
						{
							AddByRoleType(m_eight, person);
						}
						else if (person.Availability[availableDate] == Availability.ScheduledAtNineThirty)
						{
							AddByRoleType(m_nineThirty, person);
						}
						else if (person.Availability[availableDate] == Availability.ScheduledAtEleven)
						{
							AddByRoleType(m_eleven, person);
						}
					}
				}
			}
		}

		public static string toStaticString()
		{
			StringBuilder output = new StringBuilder();

			output.Append(m_eight.ToString());
			output.Append(m_nineThirty.ToString());
			output.Append(m_eleven.ToString());

			if (!m_eight.ValidateService() || !m_nineThirty.ValidateService() || !m_eleven.ValidateService())
			{
				output.Append("Errors by service below.\n\n");
			}

			if (!m_eight.ValidateService())
			{
				output.Append("8:00 AM\n\n");
				output.Append(m_eight.GetErrorMessage());
			}

			if (!m_nineThirty.ValidateService())
			{
				output.Append("9:30 AM\n\n");
				output.Append(m_nineThirty.GetErrorMessage());
			}

			if (!m_eleven.ValidateService())
			{
				output.Append("11:00 AM\n\n");
				output.Append(m_eleven.GetErrorMessage());
			}

			if (!ValidateServices())
			{
				output.Append("Errors for all services this Sunday below:\n\n");
				output.Append(m_errors);
			}

			return output.ToString();
		}

		private static bool ValidateServices()
		{
			List<string> scheduledPeople = new List<string>();
			List<string> checkForDuplicates = new List<string>();
			bool isSundayValid = true;

			scheduledPeople.AddRange(m_eight.GetScheduledPeople());
			scheduledPeople.AddRange(m_nineThirty.GetScheduledPeople());
			scheduledPeople.AddRange(m_eleven.GetScheduledPeople());

			//Check to see that we haven't scheduled the same person to more 
			//than one service this Sunday
			foreach (string name in scheduledPeople)
			{
				if (checkForDuplicates.Contains(name))
				{
					m_errors += "This name has been scheduled twice: " + name + "\n\n";

					isSundayValid = false;
				}
				else
				{
					checkForDuplicates.Add(name);
				}
			}

			return isSundayValid;
		}

		private static void AddByRoleType(WorshipService service, Person person)
		{
			if (person.RoleType == RoleType.Acolyte)
			{
				service.AddAcolyte(person);
			}
			else if (person.RoleType == RoleType.AssistingMinister)
			{
				service.AddAssistingMinister(person);
			}
			else if (person.RoleType == RoleType.Greeter)
			{
				service.AddGreeter(person);
			}
			else if (person.RoleType == RoleType.Usher)
			{
				service.AddUsher(person);
			}
		}
	}

}
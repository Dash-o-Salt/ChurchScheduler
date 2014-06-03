using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ChurchScheduler
{

	public class WorshipService
	{

		public enum WorshipTime
		{
			None,
			Eight,
			NineThirty,
			Eleven
		}

		private string m_errorMessage = "";
		private int m_numPeopleScheduled = 0;
		private WorshipTime m_worshipTime = WorshipTime.None;
		private List<Person> m_acolytes = new List<Person>();
		private List<Person> m_assistingMinisters = new List<Person>();
		private List<Person> m_ushers = new List<Person>();
		private List<Person> m_greeters = new List<Person>();

		public WorshipService(WorshipTime time)
		{
			m_worshipTime = time;
		}

		public virtual void SetTime(WorshipTime time)
		{
			m_worshipTime = time;
		}

		public virtual void AddAcolyte(Person person)
		{
			m_acolytes.Add(person);
		}

		public virtual void AddAssistingMinister(Person person)
		{
			m_assistingMinisters.Add(person);
		}

		public virtual void AddUsher(Person person)
		{
			m_ushers.Add(person);
		}

		public virtual void AddGreeter(Person person)
		{
			m_greeters.Add(person);
		}

		public virtual List<string> GetScheduledPeople()
		{
			List<Person> allPeople = new List<Person>();
			List<string> scheduledNames = new List<string>();

			allPeople.AddRange(m_acolytes);
			allPeople.AddRange(m_assistingMinisters);
			allPeople.AddRange(m_ushers);
			allPeople.AddRange(m_greeters);

			foreach (Person person in allPeople)
			{
				SplitNames(person, scheduledNames);
			}

			return scheduledNames;
		}

		public virtual void ClearService()
		{
			m_acolytes.Clear();
			m_assistingMinisters.Clear();
			m_ushers.Clear();
			m_greeters.Clear();
		}

		public virtual string GetErrorMessage()
		{
			return m_errorMessage;
		}

		//Determines if the correct number of people are scheduled for this service,
		//and if there are duplicate people scheduled for this service.
		public virtual bool ValidateService()
		{
			bool serviceIsValid = true;
			List<string> scheduledNames = new List<string>();
			List<string> checkForDuplicates = new List<string>();

			//Reset error message for this run through of validation
			m_errorMessage = "";

			foreach (Person person in m_acolytes)
			{
				SplitNames(person, scheduledNames);
			}

			if (m_numPeopleScheduled > 1)
			{
				serviceIsValid = false;
				m_errorMessage += "There are too many Acolytes - " + m_numPeopleScheduled + "\r\n";
			}
			else if (m_numPeopleScheduled < 1)
			{
				serviceIsValid = false;
				m_errorMessage += "There are too few Acolytes - " + m_numPeopleScheduled + "\r\n";
			}

			m_numPeopleScheduled = 0;

			foreach (Person person in m_assistingMinisters)
			{
				SplitNames(person, scheduledNames);
			}

			if (m_numPeopleScheduled > 1)
			{
				serviceIsValid = false;
				m_errorMessage += "There are too many Assisting Ministers - " + m_numPeopleScheduled + "\r\n";
			}
			else if (m_numPeopleScheduled < 1)
			{
				serviceIsValid = false;
				m_errorMessage += "There are too few Assisting Ministers - " + m_numPeopleScheduled + "\r\n";
			}

			m_numPeopleScheduled = 0;

			foreach (Person person in m_greeters)
			{
				SplitNames(person, scheduledNames);
			}

			if (m_numPeopleScheduled > 2)
			{
				serviceIsValid = false;
				m_errorMessage += "There are too many Greeters - " + m_numPeopleScheduled + "\r\n";
			}
			else if (m_numPeopleScheduled < 2)
			{
				serviceIsValid = false;
				m_errorMessage += "There are too few Greeters - " + m_numPeopleScheduled + "\r\n";
			}

			m_numPeopleScheduled = 0;

			foreach (Person person in m_ushers)
			{
				SplitNames(person, scheduledNames);
			}

			if (m_numPeopleScheduled > 2)
			{
				serviceIsValid = false;
				m_errorMessage += "There are too many Ushers - " + m_numPeopleScheduled + "\r\n";
			}
			else if (m_numPeopleScheduled < 2)
			{
				serviceIsValid = false;
				m_errorMessage += "There are too few Ushers - " + m_numPeopleScheduled + "\r\n";
			}

			m_numPeopleScheduled = 0;

			//Check to see that we haven't scheduled the same person to the same service twice
			//in more than one position
			foreach (string name in scheduledNames)
			{
				if (checkForDuplicates.Contains(name))
				{
					serviceIsValid = false;

					m_errorMessage += "This name has been scheduled twice: " + name + "\r\n";
				}
				else
				{
					checkForDuplicates.Add(name);
				}
			}

			return serviceIsValid;
		}

		//Prints out who's going to be at this service
		public override string ToString()
		{
			StringBuilder output = new StringBuilder();

			output.Append(TranslateTime(m_worshipTime));

			foreach (Person person in m_acolytes)
			{
				output.Append(GenerateHelperRoleString(person));
			}

			foreach (Person person in m_assistingMinisters)
			{
				output.Append(GenerateHelperRoleString(person));
			}

			foreach (Person person in m_greeters)
			{
				output.Append(GenerateHelperRoleString(person));
			}

			foreach (Person person in m_ushers)
			{
				output.Append(GenerateHelperRoleString(person));
			}

			output.Append("\r\n");

			return output.ToString();
		}

		private string GenerateHelperRoleString(Person person)
		{
			StringBuilder roleString = new StringBuilder();

			roleString.Append(person.FirstName + " " + person.LastName);
			roleString.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
			roleString.Append("\r\n");

			return roleString.ToString();
		}

		private void SplitNames(Person person, List<string> scheduledNames)
		{
            //If the last name contains 'and family' we need to add that as a separate scheduled person
            if (person.LastName.Contains("and") && person.LastName.IndexOf("family", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                string[] splitLastName = Regex.Split(person.LastName, " and ");

                scheduledNames.Add(person.FirstName + " " + splitLastName[0]);
                scheduledNames.Add(" and Family");
                m_numPeopleScheduled += 2;
            }
            else
            {
                //Now we have a list with "first name", "second first name", etc.
                string[] splitFirstNames = Regex.Split(person.FirstName, " and ");

                for (int j = 0; j < splitFirstNames.Length; j++)
                {
                    //Clean input so we don't have leading or trailing spaces
                    splitFirstNames[j] = splitFirstNames[j].Trim();

                    //This means the name we have has a full name, rather than just
                    //a first name (E.g. "John Smith" with a space).
                    if (splitFirstNames[0].IndexOf(" ", StringComparison.Ordinal) > 0)
                    {
                        scheduledNames.Add(splitFirstNames[j]);
                    }
                    else
                    {
                        //Otherwise, we add on the last name
                        scheduledNames.Add(splitFirstNames[j] + " " + person.LastName);
                    }

                    m_numPeopleScheduled++;
                }
            }
		}

		//TODO: Case statement?
		private static string TranslateTime(WorshipTime time)
		{
			if (time == WorshipTime.Eight)
			{
				return "8:00 AM\r\n\r\n";
			}
			else if (time == WorshipTime.NineThirty)
			{
                return "9:30 AM\r\n\r\n";
			}
			else if (time == WorshipTime.Eleven)
			{
                return "11:00 AM\r\n\r\n";
			}
			else
			{
                return "Unknown time\r\n\r\n";
			}
		}
	}

}
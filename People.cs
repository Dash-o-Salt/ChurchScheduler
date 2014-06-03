using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ChurchScheduler
{
	public class People
	{
		private List<Person> m_people = new List<Person>();

		//Given a list of the columns, and the raw data rows, build a list of people
		public People(Dictionary<string, int> columnsByColumnName, List<List<string>> data)
		{
            Dictionary<string, int> possibleDates = new Dictionary<string, int>();
            DateTime monthDayFormat;

			//Attempt to find out the times people can be scheduled for.
			//To do this, attempt to parse each column name into a
			//date. If it's possible to parse the date, we know that it's
			//a valid date people can be scheduled  for, as well as the
			//column that date corresponds to
			foreach (string columnName in columnsByColumnName.Keys)
			{
                //Try to parse the potential date
                if (DateTime.TryParse(columnName, out monthDayFormat))
                {
                    //If the parse succeeded, we know the date is valid,
                    //add it to the hash map which stores which columns
                    //are valid dates.
                    possibleDates[columnName] = columnsByColumnName[columnName];
                }
            }

			//Iterate through the data and assign data to person objects
			//based on what each column represents
            for (int i = 0; i < data.Count; i++)
            {
                //Stores the availability of each person on the specified d ate
                Dictionary<string, Availability> availability = new Dictionary<string, Availability>();

                //Get the array list representing the data row
                List<string> dataRowList = data[i];

                //Create a new person
                Person person = new Person();

                //Index into the data row list via column to populate
                //various details about a person.

                try
                {
                    person.FirstName = dataRowList[columnsByColumnName["First name"]];
                    person.LastName = dataRowList[columnsByColumnName["Last name"]];

                    person.Status = dataRowList[columnsByColumnName["Status"]];

                    //TODO: Find a better way to handling this enum parsing

                    //Set role type
                    string role = dataRowList[columnsByColumnName["Role"]];

                    if (role.Equals("AC"))
                    {
                        person.RoleType = RoleTypeHelper.RoleType.Acolyte;
                    }
                    else if (role.Equals("AM"))
                    {
                        person.RoleType = RoleTypeHelper.RoleType.AssistingMinister;
                    }
                    else if (role.Equals("G"))
                    {
                        person.RoleType = RoleTypeHelper.RoleType.Greeter;
                    }
                    else if (role.Equals("U"))
                    {
                        person.RoleType = RoleTypeHelper.RoleType.Usher;
                    }

                    //Set preferred time
                    string preferredTime = dataRowList[columnsByColumnName["Time"]];

                    //TODO: Needs to be able to handle multiples (e.g. 8 & 9:30)
                    if (preferredTime.Equals("8"))
                    {
                        person.PreferredTime = PreferredTime.Eight;
                    }
                    else if (preferredTime.Equals("9"))
                    {
                        person.PreferredTime = PreferredTime.NineThirty;
                    }
                    else if (preferredTime.Equals("11"))
                    {
                        person.PreferredTime = PreferredTime.Eleven;
                    }
                    else if (preferredTime.Equals("Chr"))
                    {
                        person.PreferredTime = PreferredTime.Choir;
                    }

                    //Set last served date
                    DateTime parseTime;

                    if (DateTime.TryParse(dataRowList[columnsByColumnName["Last Served"]], out parseTime))
                    {
                        person.LastServed = parseTime;
                    }

                    try
                    {
                        person.HomePhone = dataRowList[columnsByColumnName["Home phone"]];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Ignore
                    }

                    try
                    {
                        person.HomePhone = dataRowList[columnsByColumnName["Home phone"]];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Ignore
                    }

                    try
                    {
                        person.AltEmail = dataRowList[columnsByColumnName["email address 2"]];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Ignore
                    }

                    try
                    {
                        person.Address = dataRowList[columnsByColumnName["Address1"]];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Ignore
                    }

                    try
                    {
                        person.CityState = dataRowList[columnsByColumnName["City State"]];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Ignore
                    }

                    try
                    {
                        person.ZipCode = dataRowList[columnsByColumnName["Zip code"]];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Ignore
                    }

                    try
                    {
                        person.MailingLabel = dataRowList[columnsByColumnName["Mailing label"]];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        //Ignore
                    }

                    person.Email = dataRowList[columnsByColumnName["email address 1"]];

                    //Populate availability for all the recorded dates we have
                    foreach (string date in possibleDates.Keys)
                    {
                        int column = possibleDates[date];

                        string rawAvailability = dataRowList[column];

                        if (rawAvailability.Equals("A"))
                        {
                            availability[date] = Availability.Available;
                        }
                        else if (rawAvailability.Equals("I"))
                        {
                            availability[date] = Availability.Inactive;
                        }
                        else if (rawAvailability.Equals("C"))
                        {
                            availability[date] = Availability.Choir;
                        }
                        else if (rawAvailability.Equals("S"))
                        {
                            availability[date] = Availability.Scheduled;
                        }
                        else if (rawAvailability.Equals("8"))
                        {
                            availability[date] = Availability.ScheduledAtEight;
                        }
                        else if (rawAvailability.Equals("9"))
                        {
                            availability[date] = Availability.ScheduledAtNineThirty;
                        }
                        else if (rawAvailability.Equals("11"))
                        {
                            availability[date] = Availability.ScheduledAtEleven;
                        }
                    }

                    person.Availability = availability;

                    m_people.Add(person);
                } catch(Exception ex){ 
                
                } //Read errors here likely mean invalid rows, we skip them
            }
		}

		//Given a date, returns a formatted list of who is scheduled
		//for a given week - this version requires the time a person is scheduled
		//to be in the cell they are scheduled for.
		public virtual string getScheduledPeopleByScheduledTime(string date)
		{
			//Loads all worship services for the designated Sunday
			WorshipServiceManager.LoadWorshipService(date, m_people);

			//Returns a print-out of who is actually scheduled for the service
			return WorshipServiceManager.toStaticString();
		}

		//Given a date, returns a comma delimited list of email addresses for people
		//scheduled for a given week.
		public virtual string getEmailListForScheduledPeople(string date)
		{
			List<string> emails = new List<string>();
			StringBuilder emailList = new StringBuilder();

			foreach (Person person in m_people)
			{
				foreach (string availableDate in person.Availability.Keys)
				{
					if (availableDate.Equals(date) && (person.Availability[availableDate] == Availability.Scheduled || person.Availability[availableDate] == Availability.ScheduledAtEight || person.Availability[availableDate] == Availability.ScheduledAtNineThirty || person.Availability[availableDate] == Availability.ScheduledAtEleven))
					{
						if (!person.Email.Equals(""))
						{
							//Add to list of emails, eliminating duplicates
							if (!emails.Contains(person.Email))
							{
								emails.Add(person.Email);
							}
						}
					}
				}
			}

			//Write out emails to string
			foreach (string email in emails)
			{
				emailList.Append(email + ", ");
			}

			//Subtract off the last comma
			if (emailList.Length > 2)
			{
				emailList.Remove(emailList.Length - 2, emailList.Length - emailList.Length - 2);
			}

			return emailList.ToString();
		}

		public virtual string getEmailListByRoleType(RoleTypeHelper.RoleType roleType)
		{
			List<string> emails = new List<string>();
			StringBuilder emailList = new StringBuilder();

			foreach (Person person in m_people)
			{
				if (person.RoleType == roleType && person.Status.Equals("A"))
				{
					if (!person.Email.Equals(""))
					{
						//Add to list of emails, eliminating duplicates
						if (!emails.Contains(person.Email))
						{
							emails.Add(person.Email);
						}
					}
				}
			}

			//Write out emails to string
			foreach (string email in emails)
			{
				emailList.Append(email + ", ");
			}

			//Subtract off the last comma
			emailList.Remove(emailList.Length - 2, emailList.Length - emailList.Length - 2);

			return emailList.ToString();
		}

		public virtual string EmailListActiveMembers
		{
			get
			{
                HashSet<string> emails = new HashSet<string>();
				StringBuilder emailList = new StringBuilder();
    
				foreach (Person person in m_people)
				{
					if (person.Status.Equals("A"))
					{
                        if (!String.IsNullOrEmpty(person.Email))
                        {
                            emails.Add(person.Email);
                        }

                        if (!String.IsNullOrEmpty(person.AltEmail))
                        {
                            emails.Add(person.AltEmail);
                        }
					}
				}
    
				//Write out emails to string
				foreach (string email in emails)
				{
					emailList.Append(email + ", ");
				}
    
				//Subtract off the last comma
				emailList.Remove(emailList.Length - 2, emailList.Length - (emailList.Length - 2));
    
				return emailList.ToString();
			}
		}

		public virtual string getEmailMailingListByRoleType(RoleTypeHelper.RoleType roleType)
		{
			List<string> emailMailingList = new List<string>();
			StringBuilder emailAndAddressList = new StringBuilder();

			foreach (Person person in m_people)
			{
				if (person.RoleType == roleType && person.Status.Equals("A"))
				{
					string email = person.Email;

					if (email.Equals(""))
					{
						email = "No Email";
					}

					emailMailingList.Add(person.FirstName + " " + person.LastName + " - " + email + " - " + person.Address);
				}
			}

			emailMailingList.Sort();

			//Write out emails to string
			foreach (string emailAndMailingAddress in emailMailingList)
			{
				emailAndAddressList.Append(emailAndMailingAddress + "\n");
			}

			return emailAndAddressList.ToString();
		}
	}

}
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
			//a valid date people can be scheduled for, as well as the
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
		//for a given week. NOTE: This method is deprecated as of 4/30/2008.
		public virtual string getScheduledPeople(string date, Dictionary<int, string> choirSchedule, Dictionary<string, int> dataColumnsByColumnName)
		{
			StringBuilder eightList = new StringBuilder();
			StringBuilder nineThirtyList = new StringBuilder();
			StringBuilder elevenList = new StringBuilder();
			StringBuilder unknown = new StringBuilder();

			//Set up initial buffer contents
			eightList.Append("8:00 AM\n\n");
			nineThirtyList.Append("9:30 AM\n\n");
			elevenList.Append("11:00 AM\n\n");
			unknown.Append("Unknown\n\n");

			//Find out what the choir schedule time is for this week
			int choirScheduleColumn = dataColumnsByColumnName[date];
			string choirScheduleTime = choirSchedule[choirScheduleColumn];

			foreach (Person person in m_people)
			{
				foreach (string availableDate in person.Availability.Keys)
				{
					if (availableDate.Equals(date) && person.Availability[availableDate] == Availability.Scheduled)
					{
						if (person.PreferredTime == PreferredTime.Eight)
						{
							eightList.Append(person.FirstName + " " + person.LastName);
							eightList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
							eightList.Append("\n");
						}
						else if (person.PreferredTime == PreferredTime.NineThirty)
						{
							nineThirtyList.Append(person.FirstName + " " + person.LastName);
							nineThirtyList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
							nineThirtyList.Append("\n");
						}
						else if (person.PreferredTime == PreferredTime.Eleven)
						{
							elevenList.Append(person.FirstName + " " + person.LastName);
							elevenList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
							elevenList.Append("\n");
						}
						else if (person.PreferredTime == PreferredTime.Choir)
						{
							//Use the scheduled choir time for this week to determine what
							//service this person is serving
							if (choirScheduleTime.Equals("8"))
							{
								eightList.Append(person.FirstName + " " + person.LastName);
								eightList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
								eightList.Append("\n");
							}
							else if (choirScheduleTime.Equals("9"))
							{
								nineThirtyList.Append(person.FirstName + " " + person.LastName);
								nineThirtyList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
								nineThirtyList.Append("\n");
							}
							else if (choirScheduleTime.Equals("11"))
							{
								elevenList.Append(person.FirstName + " " + person.LastName);
								elevenList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
								elevenList.Append("\n");
							}
							else
							{
								//Choir is not scheduled this week, we don't know
								//what time the person is scheduled for
								unknown.Append(person.FirstName + " " + person.LastName);
								unknown.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
								unknown.Append("\n");
							}
						}
					}
				}
			}

			eightList.Append("\n");
			nineThirtyList.Append("\n");
			elevenList.Append("\n");

			eightList.Append(nineThirtyList.ToString());
			eightList.Append(elevenList.ToString());

			//This really needs to use a list of unknown people and
			//check the size instead
			if (!unknown.ToString().Equals("Unknown\n\n"))
			{
				eightList.Append(unknown.ToString());
			}

			return eightList.ToString();
		}

		//Given a date, returns a formatted list of who is scheduled
		//for a given week - this version requires the time a person is scheduled
		//to be in the cell they are scheduled for.
		public virtual string getScheduledPeopleByScheduledTimeOld(string date)
		{
			StringBuilder eightList = new StringBuilder();
			StringBuilder nineThirtyList = new StringBuilder();
			StringBuilder elevenList = new StringBuilder();
			StringBuilder errorsFound = new StringBuilder();
			StringBuilder errorOutput = new StringBuilder();
			List<string> qualityControlSchedule = new List<string>();
			List<string> checkForDuplicates = new List<string>();
			List<string> scheduledNames = new List<string>();

			bool passedQC = true;

			//Set up initial buffer contents
			eightList.Append("8:00 AM\n\n");
			nineThirtyList.Append("9:30 AM\n\n");
			elevenList.Append("11:00 AM\n\n");
			errorOutput.Append("Passed QC:\n\n");

			foreach (Person person in m_people)
			{
				foreach (string availableDate in person.Availability.Keys)
				{
					if (availableDate.Equals(date))
					{
						//TODO: This double check is pointless. Remove it.
						if (person.Availability[availableDate] == Availability.ScheduledAtEight || person.Availability[availableDate] == Availability.ScheduledAtNineThirty || person.Availability[availableDate] == Availability.ScheduledAtEleven)
						{
							//TODO: Instead of adding to a string, this could insert into a WorshipService object.
							//Said object should be able to generate through toString a listing of who will be
							//helping out at the service, and will also be able to QC itself.
							if (person.Availability[availableDate] == Availability.ScheduledAtEight)
							{
								eightList.Append(person.FirstName + " " + person.LastName);
								eightList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
								eightList.Append("\n");
							}
							else if (person.Availability[availableDate] == Availability.ScheduledAtNineThirty)
							{
								nineThirtyList.Append(person.FirstName + " " + person.LastName);
								nineThirtyList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
								nineThirtyList.Append("\n");
							}
							else if (person.Availability[availableDate] == Availability.ScheduledAtEleven)
							{
								elevenList.Append(person.FirstName + " " + person.LastName);
								elevenList.Append(" - " + RoleTypeHelper.getRoleTypeString(person.RoleType));
								elevenList.Append("\n");
							}

							qualityControlSchedule.Add(person.FirstName + " " + person.LastName);
						}
					}
				}
			}

			eightList.Append("\n");
			nineThirtyList.Append("\n");
			elevenList.Append("\n");

			errorsFound.Append("Errors found:\n\n");

			//We need to split up names when we have a couple to check if they've
			//been scheduled twice.
			//This ASSUMES that the format is "first name and second first name + last name",
			//or "first name and second name and third name + last name".
			//It can also handle "first name + last name + second first name + second last name etc."
			//TODO: It's silly that we have to split out a last name when we have the last name on a Person
			//object to start with. Think about how to clean this up.
			//TODO: All this QC stuff needs to be migrated out to a WorshipService object so
			//we can have it objectified all in one place (and make this MUCH cleaner)
			//TODO: Need to add a QC check for if we've scheduled more people than we need to
			//for a particular service type. This would also work out best in a WorshipService object.
			//TODO: Create WorshipService object.
			//TODO: Also need a ServiceManager object that manages worship services - some QC
			//checks are across services.
			foreach (string name in qualityControlSchedule)
			{
				//Now we have a list with "first name", "second first name + last name"
				string[] splitNames = Regex.Split(name, "and");

				//This will only happen if we have more than one name
				if (splitNames.Length > 1)
				{
					//Clean input so we don't have leading or trailing spaces
					for (int j = 0; j < splitNames.Length; j++)
					{
						splitNames[j] = splitNames[j].Trim();
					}

					//Uh oh, we have a last name for the first person as well.
					//E.g. we have a couple with different last names.
					//In this case, we just add all names in the split list.
					if (splitNames[0].IndexOf(" ", StringComparison.Ordinal) > 0)
					{
						foreach (string fullName in splitNames)
						{
							scheduledNames.Add(fullName);
						}
					}
					else
					{
						//The splitSecondName will have "second first name", "last name"
                        string[] splitSecondName = Regex.Split(splitNames[1], " ");
						//This will store the "last name"
						string lastName = splitSecondName[1];

						//We want to grab all names in the split list except for the last one
						for (int i = 0; i < splitNames.Length - 1; i++)
						{
							scheduledNames.Add(splitNames[i] + " " + lastName);
						}

						scheduledNames.Add(splitSecondName[0] + " " + lastName);
					}
				}
				else
				{
					//One name case
					scheduledNames.Add(splitNames[0]);
				}
			}

			//Check to see that we haven't scheduled the same person to the same service twice
			foreach (string name in scheduledNames)
			{
				if (checkForDuplicates.Contains(name))
				{
					passedQC = false;

					errorsFound.Append("This name has been scheduled twice: " + name + "\n");
				}
				else
				{
					checkForDuplicates.Add(name);
				}
			}

			errorOutput.Append(passedQC + "\n\n");

			if (passedQC)
			{
				errorsFound = new StringBuilder();

				errorsFound.Append("No errors found.\n\n");
			}

			errorOutput.Append(errorsFound);
			errorOutput.Append("\n");

			eightList.Append(nineThirtyList.ToString());
			eightList.Append(elevenList.ToString());
			eightList.Append(errorOutput.ToString());

			return eightList.ToString();
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
				List<string> emails = new List<string>();
				StringBuilder emailList = new StringBuilder();
    
				foreach (Person person in m_people)
				{
					if (person.Status.Equals("A") && !person.Email.Equals(""))
					{
						//Add to list of emails, eliminating duplicates
						if (!emails.Contains(person.Email))
						{
							emails.Add(person.Email);
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
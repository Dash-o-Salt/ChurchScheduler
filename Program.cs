using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ChurchScheduler
{
	public class Program
	{

		private static Dictionary<string, int> m_dataColumnsByColumnName = new Dictionary<string, int>();
		private static Dictionary<int, string> m_dataColumnsByColumnID = new Dictionary<int, string>();
		private static Dictionary<int, string> m_choirSchedule = new Dictionary<int, string>();
		private static List<List<string>> m_data = new List<List<string>>();

		/// <param name="args"> </param>
		public static void Main(string[] args)
		{
			bool printAllActiveEmails = false;
			bool qcMode = false;
            RoleTypeHelper.RoleType emailRoleType = RoleTypeHelper.RoleType.Acolyte;
			string date = null;
			string filePath = null;

			if (args.Length < 1)
			{
				PrintUsageMethod();
				return;
			}

			//Parse the command line arguments
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].Equals("-a"))
				{
					printAllActiveEmails = true;
				}
				else if (args[i].Equals("-qc"))
				{
					qcMode = true;
				}
				else if (args[i].Equals("-emails"))
				{
					if (args[i + 1].Equals("AC"))
					{
						emailRoleType = RoleTypeHelper.RoleType.Acolyte;
					}
					else if (args[i + 1].Equals("AM"))
					{
						emailRoleType = RoleTypeHelper.RoleType.AssistingMinister;
					}
					else if (args[i + 1].Equals("G"))
					{
						emailRoleType = RoleTypeHelper.RoleType.Greeter;
					}
					else if (args[i + 1].Equals("U"))
					{
						emailRoleType = RoleTypeHelper.RoleType.Usher;
					}
				}
				else if (args[i].Equals("-date"))
				{
					date = args[i + 1];

					i++;
				}
				else
				{
					filePath = args[i];
				}
			}

			//No filepath inputted
			if (filePath == null)
			{
				PrintUsageMethod();
				return;
			}

			//Load the contents of the CSV file.
			string fileContents = File.ReadAllText(filePath, Encoding.UTF8);

			//Populate hash tables with contents of the spread sheet
			//TODO: Work on getting this into a separate class
            LoadContents(fileContents);

			//Pull data from the populated hash tables to populate a list of people
			//who are volunteering their time
			People people = new People(m_dataColumnsByColumnName, m_data);

			if (printAllActiveEmails)
			{
				Console.WriteLine(people.EmailListActiveMembers);
			}
			else if (qcMode)
			{
                DateTime inputDate;
				int startMonth;
				int incrementMonth;
				int lastMonth;

                if (DateTime.TryParse(date, out inputDate))
                {
                    startMonth = inputDate.Month;
                    incrementMonth = startMonth;

                    while (incrementMonth < startMonth + 4)
                    {
                        String currentDate = String.Format("{0}/{1}", inputDate.Month, inputDate.Day);

                        Console.WriteLine(String.Format("Schedule for {0}", currentDate));

                        //Print out schedule info for this week
                        Console.WriteLine(people.getScheduledPeopleByScheduledTime(currentDate));

                        //Add week
                        inputDate = inputDate.AddDays(7);

                        if (inputDate.Month != incrementMonth)
                        {
                            incrementMonth++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Input date was not in a correct format.");
                }
			}
			else
			{
				if (date != null)
				{
					Console.WriteLine("Reminder for Worship Assistants Sunday, " + date + "\n");

					Console.WriteLine(people.getEmailListForScheduledPeople(date) + "\n");

					Console.WriteLine("Greetings! Here is the reminder for the upcoming Sunday services.\n");

					Console.WriteLine("Readings for the assistant ministers are located here: http://hslckirkland.org/Worship/worshipasst.aspx \n");

					Console.WriteLine(people.getScheduledPeopleByScheduledTime(date));

					Console.WriteLine("I have included a copy of the " + "upcoming schedule.\n\n" + "Thanks,\nThomas Wiese");
				}

				if (emailRoleType != null)
				{
					Console.WriteLine("Emails for active people with the role of " + RoleTypeHelper.getRoleTypeString(emailRoleType) + ":\n");

					Console.WriteLine(people.getEmailListByRoleType(emailRoleType) + "\n");

					Console.WriteLine("Name - Email - Mailing Address: ");

					Console.WriteLine(people.getEmailMailingListByRoleType(emailRoleType));
				}
			}
		}

		//TODO: Update usage method with all the flags that can be used. Add
		//a help or ? flag to allow users to see possible flags.
		private static void PrintUsageMethod()
		{
			Console.WriteLine("This application requires the file path of the comma separated value schedule spreadsheet to be processed.");
			Console.WriteLine("Usage: Scheduler [-emails RoleType] [-date scheduledDate] filename");
		}

		private static void LoadContents(string csvFile)
		{            
			int endRows = 0;
			int blankCount = 0;
			int columnCounter = 0;

            List<string> dataRows = new List<string>(Regex.Split(csvFile, "\r\n"));

			//Tear through everything in the file, populating hash tables
			//as we go. First, we'll populate a hash table for the first row
			//so we can figure out what each column is. Next, we'll populate
            //a hash table with the choir schedule. Then, we'll populate
            //a hash table with the actual data. Finally, we'll use the column
            //mapping hash table to figure out where the data is stored in
			//the data hash map. Note that we trim all contents to prevent input
			//errors from messing with the program later.
            for (int i = 0; i < dataRows.Count; i++)
			{
                string[] splitRow = Regex.Split(dataRows[i], ",");

                for (int j = 0; j < splitRow.Length; j++)
                {
                    if (splitRow[j].Equals(""))
                    {
                        continue;
                    }

                    if (i == 0)
                    {
                        //Populate list of columns
                        m_dataColumnsByColumnName.Add(splitRow[j], j);
                        m_dataColumnsByColumnID.Add(j, splitRow[j]);
                    }
                    else if (i == 1)
                    {
                        //Populate choir schedule
                        if (splitRow[j].Equals("11") || splitRow[j].Equals("8") || splitRow[j].Equals("9") || splitRow[j].Equals("8 & 9:30") || splitRow[j].Equals("9:30 & 11"))
                        {
                            m_choirSchedule.Add(j, splitRow[j]);
                        }
                    }
                }

                if (i > 1)
                {
                    m_data.Add(new List<String>(splitRow));
                }
			}
		}
	}

}
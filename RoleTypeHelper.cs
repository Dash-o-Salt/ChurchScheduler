namespace ChurchScheduler
{

	public class RoleTypeHelper
	{

		public enum RoleType
		{
			Acolyte,
			AssistingMinister,
			Greeter,
			Usher
		}

		public static string getRoleTypeString(RoleTypeHelper.RoleType roleType)
		{
			if (roleType == RoleType.Acolyte)
			{
				return "Acolyte";
			}
			else if (roleType == RoleType.AssistingMinister)
			{
				return "Assisting Minister";
			}
			else if (roleType == RoleType.Greeter)
			{
				return "Greeter";
			}
			else if (roleType == RoleType.Usher)
			{
				return "Usher";
			}
			else
			{
				return "Unknown";
			}
		}
	}

}
namespace MARINEYE.Areas.Identity.Data
{
    public static class Constants
    {
        public const string MainAdminRole = "Admin";
        public const string DefaultRole = "Klient";
        public const string UserListAccessRoles = Constants.MainAdminRole + "," + "Bosman";
        public const string EditBoatListAccessRoles = Constants.MainAdminRole + "," + "Bosman";
        public const string EditClubDuesRoles = Constants.MainAdminRole + "," + "Bosman";

        public readonly static List<string> Roles = new List<string> { MainAdminRole, "Bosman", "Członek", DefaultRole };
    }
}

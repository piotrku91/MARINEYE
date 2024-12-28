namespace MARINEYE.Areas.Identity.Data
{
    public static class Constants
    {
        public const string MainAdminRole = "Admin";
        public const string DefaultRole = "Extern";
        public const string UserListAccessRoles = Constants.MainAdminRole + "," + "Boatswain";
        public const string EditBoatListAccessRoles = Constants.MainAdminRole + "," + "Boatswain";
        public readonly static List<string> Roles = new List<string> { MainAdminRole, "Boatswain", "Member", DefaultRole };
    }
}

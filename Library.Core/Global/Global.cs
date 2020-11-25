using System.Collections.Generic;

namespace Library.Core
{
    public static class Global
    {
        public const string FAILURE_KEY = "FailureKey";
        public const string WARNING_KEY = "WarningKey";

        public const string ROLE_ADMINISTRATOR = "Administrator";
        public const string ROLE_RESELLER = "Reseller";

        public const string IMAGE_FOLDER_NAME = "BookImageUploads";

        public static List<string> ImageExtensions = new List<string> {".jpeg", ".png", ".jpg", ".bmp"};

        public static string CLAIM_DISPLAYNAME = "Display_Name";
    }
}

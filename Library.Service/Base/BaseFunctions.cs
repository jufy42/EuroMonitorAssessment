using System.IO;
using Library.Core;

namespace Library.Service
{
    public class BaseFunctions
    {
        internal string CheckBookImageFolder()
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), $@"wwwroot\{Global.IMAGE_FOLDER_NAME}")))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), $@"wwwroot\{Global.IMAGE_FOLDER_NAME}"));

            return Path.Combine(Directory.GetCurrentDirectory(), $@"wwwroot\{Global.IMAGE_FOLDER_NAME}");
        }
    }
}

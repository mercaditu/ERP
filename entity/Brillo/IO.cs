using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace entity.Brillo
{
    public static class IO
    {
        /// <summary>
        /// This function will create a path if it does not previously exist.
        /// </summary>
        /// <param name="path">Full Path</param>
        /// <returns>Full Path but verified</returns>
        public static string CreateIfNotExists(string path)
        {
            string directory = string.Empty;
            int index = path.LastIndexOf(@"\");
            if (index > 0)
                directory = path.Substring(0, index);

            if (!Directory.Exists(directory))
            {
                DirectoryInfo di = Directory.CreateDirectory(directory);
            }

            //DirectoryInfo dInfo = new DirectoryInfo(directory);
            //DirectorySecurity dSecurity = dInfo.GetAccessControl();
            //dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            //dInfo.SetAccessControl(dSecurity);

            return path;
        }
    }
}

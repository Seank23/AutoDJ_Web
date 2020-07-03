using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoDJ_Web.Models
{
    public class SessionUserPermissions
    {
        public bool CanStop { get; set; }
        public bool CanRemove { get; set; }
        public bool CanAddPlaylist { get; set; }
        public bool HidePlayer { get; set; }
        public bool CanClearQueue { get; set; }

        public SessionUserPermissions(bool[] permissions)
        {
            CanStop = permissions[0];
            CanRemove = permissions[1];
            CanAddPlaylist = permissions[2];
            HidePlayer = permissions[3];
            CanClearQueue = false;
        }

        public Dictionary<string, bool> PermissionsToDict()
        {
            return new Dictionary<string, bool>() { { "CanStop", CanStop }, { "CanRemove", CanRemove }, { "CanAddPlaylist", CanAddPlaylist }, { "HidePlayer", HidePlayer }, { "CanClearQueue", CanClearQueue } };
        }
    }
}

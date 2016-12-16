using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FsApi
{
    public class MutliRoomItem
    {
        public string id { get; internal set; }
        public string name { get; internal set; }
        public string ipaddress { get; internal set; }
        public string audiosyncversion { get; internal set; }
        public string groupid { get; internal set; }
        public string groupname { get; internal set; }
        public int grouprole { get; internal set; }
        public int clientnumber { get; internal set; }
    }
}

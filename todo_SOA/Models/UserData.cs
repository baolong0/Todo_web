using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todo_SOA.Models
{
    public class UserData
    {
        public string ServerEndPoin = "";
        public string UserID = "";
        public UserData()
        {
        }
        public UserData(string sv, string id)
        {
            this.ServerEndPoin = sv;
            this.UserID = id;
        }
        public string GetUserID()
        {
            return this.UserID;
        }
    }
}

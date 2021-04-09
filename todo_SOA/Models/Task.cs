using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace todo_SOA.Models
{
    public class Task
    {
        string id;
        string name;
        string description;
        string dueTime;
        string status;
        string tag;

        public Task(string id, string name,string description, string dueTime, string status, string tag)
        {
            this.id = id;
            this.name = name;
            this.dueTime = dueTime;
            this.status = status;
            this.tag = tag;
            this.description = description;
        }

        public string Id { get => Id; set => Id = value; }
        public string Name { get => name; set => name = value; }
        public string DueTime { get => dueTime; set => dueTime = value; }
        public string Status { get => status; set => status = value; }
        public string Tag { get => tag; set => tag = value; }
        public string Description { get => description; set => description = value; }
    }
}

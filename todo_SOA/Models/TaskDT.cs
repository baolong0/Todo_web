namespace todo_SOA.Models
{
    public class TaskDT
    {

        string id;
        string name;
        string dueTime;
        string status;
        string tag;
        public TaskDT()
        {

        }
        public TaskDT(string id,
        string name,
        string dueTime,
        string status,
        string tag)
        {
            this.id = id;
            this.name = name;
            this.dueTime = dueTime;
            this.status = status;
            this.tag = tag;

        }

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string DueTime { get => dueTime; set => dueTime = value; }
        public string Status { get => status; set => status = value; }
        public string Tag { get => tag; set => tag = value; }
    }
}
using System.Collections.Generic;

namespace ClientConsoleApp.Models
{
    public class Group
    {
        public string GroupName { get; set; } = string.Empty;
        public List<User> Users { get; set; } = new List<User>();
    }
}
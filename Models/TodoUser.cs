
using System.Collections.Generic;

namespace TodoApi.Models
{
    public class TodoUser
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public IList<TodoItem> Todos { get; set; }
    
    }
}

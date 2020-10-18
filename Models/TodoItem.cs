
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool isComplete { get; set; }
        
        [ForeignKey("TodoUserId")]
        public long TodoUserId { get; set; }
        public TodoUser User { get; set; }

    }
}

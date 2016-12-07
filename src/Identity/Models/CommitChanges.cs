using System.Collections.Generic;

namespace Identity.Models
{
    public class CommitChanges
    {
        public List<string> NewFiles { get; set; }
        public List<string> DeletedFiles { get; set; }
        public Dictionary<string, string[]> Changes { get; set; }
    }
}
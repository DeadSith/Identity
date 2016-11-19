using System.Collections.Generic;

namespace Identity.Models.HomeViewModels
{
    public class ViewFileViewModel
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public string RepoRootPath { get; set; }
        public List<string> Path { get; set; }
    }
}
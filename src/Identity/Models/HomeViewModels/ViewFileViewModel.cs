using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace Identity.Models.HomeViewModels
{
    public class ViewFileViewModel
    {
        public string FileName { get; set; }
        public List<HtmlString> FileContent { get; set; }
        public string RepoRootPath { get; set; }
        public List<string> Path { get; set; }
    }
}
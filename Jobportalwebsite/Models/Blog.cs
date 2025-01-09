using System;
using System.Collections.Generic;

namespace Jobportalwebsite.Models
{
    public class Blog
    {
        public int Id { get; set; }  
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string> PicturePaths { get; set; } = new List<string>();
        public DateTime DateCreated { get; set; }
        public bool Deleted { get; set; }  
        public string? CreatedBy { get; set; }  

        
        public ICollection<Comment>? Comments { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }  
        public string? CommentText { get; set; }
        public DateTime DateCommented { get; set; }
        public bool IsAnonymous { get; set; }
        public string? CommentedBy { get; set; }  

        
        public int BlogId { get; set; }
        public Blog? Blog { get; set; }  
    }
}


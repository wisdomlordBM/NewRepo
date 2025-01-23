namespace Jobportalwebsite.ViewModels
{
    public class CommentFormModel
    {
        public int BlogId { get; set; }
    }

    public class ReplyFormModel
    {
        public int BlogId { get; set; }
        public int ParentCommentId { get; set; }
    }

   
}



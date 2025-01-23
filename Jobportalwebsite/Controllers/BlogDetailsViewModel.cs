//using Jobportalwebsite.Models;
//using Jobportalwebsite.ViewModels;

//namespace Jobportalwebsite.Controllers
//{
//    internal class BlogDetailsViewModel
//    {
//        public Blog Blog { get; set; }
//        public List<CommentViewModel> Comments { get; set; }
//    }
//}@if (User.Identity.IsAuthenticated)


//{
//    < form method = "post" asp - action = "AddComment" asp - route - blogId = "@Model.Id" >
//        < div class= "form-group" >
//            < textarea class= "form-control" name = "commentText" placeholder = "Write your comment here..." rows = "4" ></ textarea >
//        </ div >
//        < div class= "form-check" >
//            < input type = "checkbox" class= "form-check-input" name = "isAnonymous" />
//            < label class= "form-check-label" for= "isAnonymous" > Post as Anonymous</ label >
//        </ div >
//        < button type = "submit" class= "btn btn-primary mt-2" > Post Comment </ button >
//    </ form >
//}
//else
//{
//    < p >< a href = "@Url.Action("Login", "Account")" > Login </ a > or < a href = "@Url.Action("Register", "Account")" > Register </ a > to post a comment.</ p >
//}
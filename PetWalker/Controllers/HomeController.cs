using PetWalker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace PetWalker.Controllers
{
    public class HomeController : Controller
    {
        PetWalkerDBEntities db = new PetWalkerDBEntities();
        private static Random random = new Random();
        // GET: Walker
        public ActionResult Index()
        {
            using (var datamodel = new PetWalkerDBEntities())
            {
                var j = datamodel.Posts.ToList();
                if (j != null) { return View(j); }
                else
                {
                    return View();
                }

            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ChatPage()
        {
            using (var datamodel = new PetWalkerDBEntities())
            {
                var j = datamodel.Walkers.ToList();
                return View(j);
            }
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        public JsonResult PWReset(string EMail, string Phone, string IdentityNumber)
        {
            Customer User = db.Customers.Where(b => b.EMail.Equals(EMail) && b.Phone.Equals(Phone) && b.IDNumber.Equals(IdentityNumber)).FirstOrDefault();
            if (User != null)
            {
                var NewPassword = RandomString();
                User.Password = NewPassword;
                db.SaveChanges();
                System.Threading.Thread.Sleep(1000);
                return Json(NewPassword);
            }
            else { return Json(false); }

        }

        [HttpPost]
        public bool SendMessage(string Message, int InternalReceiverID)
        {
            Message MyMessage = new Message();
            var ID = Convert.ToInt16(Session["ID"]);
            Customer User = db.Customers.Where(b => b.CustomerID.Equals(InternalReceiverID)).FirstOrDefault();
            if (MyMessage != null)
            {
                if (User==null)
                {
                    Walker WalkerUser = db.Walkers.Where(b => b.WalkerID.Equals(InternalReceiverID)).FirstOrDefault();
                    MyMessage.MessageReceiverNameSurname = (WalkerUser.FName.Trim() + " " + WalkerUser.LName.Trim()).ToString();
                }
                else
                {
                    MyMessage.MessageReceiverNameSurname = (User.FName.Trim() + " " + User.LName.Trim()).ToString();
                }
                if (ID == 0)
                {
                    return false;
                }
                else
                {
                    MyMessage.MessageSenderID = ID;
                    MyMessage.MessageReceiverID = InternalReceiverID;
                    MyMessage.MessageText = Message;
                    MyMessage.MessageDate = DateTime.Now;
                    MyMessage.MessageSenderNameSurname = (Session["FName"] + " " + Session["LName"]).ToString();
                    db.Messages.Add(MyMessage);
                    db.SaveChanges();
                    System.Threading.Thread.Sleep(1000);
                    return true;
                }
            }
            return false;
        }

        public ActionResult UploadPost(string PostText)
        {
            Post PostData = new Post();
            int idForUpdate = Convert.ToInt32(Session["ID"]);
            Customer PostOwner = db.Customers.Where(b => b.CustomerID.Equals(idForUpdate)).FirstOrDefault();
            if (idForUpdate != null)
            {
                PostData.PostOwnerID = idForUpdate;
                PostData.PostText = PostText;
                PostData.PostDate = DateTime.Now.ToString();
                PostData.PostOwnerNameSurname = (PostOwner.FName + PostOwner.LName).Trim();
                PostData.PostOwnerAddress = PostOwner.Address.Trim();
                db.Posts.Add(PostData);
                db.SaveChanges();

            }
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Images/Posts/") + PostData.PostID + ".png");
                System.IO.File.Delete(path);
                file.SaveAs(path);
                db.SaveChanges();
            }

            return Json(true);

        }

        [HttpPost]
        public JsonResult LikePost(int PostID)
        {
            var IsLiked = db.Likes.Where(b => b.LikedPostID == (PostID)).ToList();
            var ID = Convert.ToInt32(Session["ID"]);
            foreach (var b in IsLiked)
            {
                if (b.LikerID == ID) //postu likelamış demektir
                {
                    b.LikerID = null;
                    b.LikedPostID = null;
                    db.SaveChanges();
                    UpdateLikeCommentCount(PostID);
                    return Json("Unliked");
                }
            }
            Like Liked = new Like();
            Liked.LikedPostID = PostID;
            Liked.LikerID = Convert.ToInt32(Session["ID"]);
            db.Likes.Add(Liked);
            db.SaveChanges();
            System.Threading.Thread.Sleep(1000);
            UpdateLikeCommentCount(PostID);
            return Json("Liked");
        }
        [HttpPost]
        public JsonResult GetLikerName(int PostID)
        {
            var IsLiked = db.Likes.Where(b => b.LikedPostID == (PostID)).ToList();
            string LikerString="";
            foreach (var b in IsLiked)
            {
                    LikerString += (b.Customer.FName.Trim() +" "+ b.Customer.LName.Trim()).ToString()+", ";

            }
            return Json(LikerString);
        }
        [HttpPost]
        public JsonResult GetComments(int PostID)
        {
            var Comments = db.Comments.Where(b => b.CommentPostID == (PostID)).ToList();
            var ListOfComments = new List<string>();
            foreach (var b in Comments)
            {
                ListOfComments.Add(b.Customer.FName.Trim() + " " + b.Customer.LName.Trim()+ ": "+ b.CommentText.Trim());
            }
            return Json(ListOfComments);
        }
        public bool UpdateLikeCommentCount(int PostID)
        {
            var LikeData = db.Likes.Where(b => b.LikedPostID == (PostID)).ToList();
            var CommentData = db.Comments.Where(b => b.CommentPostID == (PostID)).ToList();
            var CommentCount = CommentData.Count();
            int LikeCount = LikeData.Count;
            Post UpdatePost = db.Posts.Where(b => b.PostID == (PostID)).FirstOrDefault();
            UpdatePost.PostLikeCount = LikeCount;
            UpdatePost.PostCommentCount = CommentCount;
            db.SaveChanges();
            return true;
        }

        [HttpPost]
        public JsonResult SendComment(string Comment, int PostID)
        {
            Comment NewComment = new Comment();
            var ID = Convert.ToInt16(Session["ID"]);
            if (ID != null)
            {
                if (ID == 0)
                {
                    return Json("Please login!");
                }
                else
                {
                    NewComment.CommentPostID = PostID;
                    NewComment.CommenterID = ID;
                    NewComment.CommentText = Comment;
                    db.Comments.Add(NewComment);
                    db.SaveChanges();
                    System.Threading.Thread.Sleep(1000);
                    return Json("Comment sended.");
                }
            }
            return Json("Please login!");
        }

        public static string RandomString()
        {
            int length = 7;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using PetWalker.Models;
using System.Web.Security;
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Web.DynamicData;
using System.Web.Helpers;
using System.Collections.ObjectModel;

namespace PetWalker.Controllers
{
    public class UserController : Controller
    {
        PetWalkerDBEntities db = new PetWalkerDBEntities();

        // GET: User
        public ActionResult SignUP()
        {
            return View();
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }
        public ActionResult UserProfile()
        {
            return View();
        }
        public ActionResult AddPet()
        {
            return View();
        }
        public ActionResult LogOut()
        {
            return View();
        }
        public ActionResult SearchProfile()
        {
            using (var datamodel = new PetWalkerDBEntities())
            {

                var customers = datamodel.Customers.ToList();
                var walkers = datamodel.Walkers.ToList();

                var combinedViewModel = new CombinedViewModel
                {
                    Customers = customers,
                    Walkers = walkers
                };

                return View(combinedViewModel);

            }
        }
        public ActionResult Chat()
        {
            using (var datamodel = new PetWalkerDBEntities())
            {

                var customers = datamodel.Customers.ToList();
                var walkers= datamodel.Walkers.ToList();
                var UserID = Convert.ToInt32(Session["ID"]);
                var messages = db.Messages.Where(b => b.MessageReceiverID == (UserID) || b.MessageSenderID == (UserID)).ToList();
                var combinedViewModel = new CombinedViewModel
                {
                    Customers = customers,
                    Messages = messages,
                    Walkers = walkers
                };
                return View(combinedViewModel);
            }

        }
        public ActionResult InspectUserProfile()
        {
            return View();
        }
        public class CombinedViewModel
        {
            public List<Customer> Customers { get; set; }
            public List<Walker> Walkers { get; set; }
            public List<Message> Messages { get; set; }
        }
        //User creation
        [HttpPost]
        public bool UserCreater(string FName, string LName, string EMail, string Phone, string Password, string DOB, string Address, string ZipPostal, string IsWalker, string IDNumber)
        {
            if (IsWalker == "on")
            {
                Walker NewWalker = new Walker();
                if (NewWalker != null)
                {
                    NewWalker.FName = FName;
                    NewWalker.LName = LName;
                    NewWalker.EMail = EMail;
                    NewWalker.Phone = Phone;
                    NewWalker.Password = Password;
                    NewWalker.DateOfBirth = DOB;
                    NewWalker.Address = Address;
                    NewWalker.Latitude = "0";
                    NewWalker.Longitude= "0";
                    db.Walkers.Add(NewWalker);
                    db.SaveChanges();
                    System.Threading.Thread.Sleep(1000);
                    return true;

                }
            }
            else
            {
                Customer newCustomer = new Customer();
                if (newCustomer != null)
                {
                    newCustomer.FName = FName;
                    newCustomer.LName = LName;
                    newCustomer.EMail = EMail;
                    newCustomer.Phone = Phone;
                    newCustomer.Password = Password;
                    newCustomer.DOB = DOB;
                    newCustomer.Address = Address;
                    newCustomer.ZipPostal = ZipPostal;
                    newCustomer.IDNumber = IDNumber;
                    db.Customers.Add(newCustomer);
                    db.SaveChanges();
                    System.Threading.Thread.Sleep(1000);
                    return true;
                }
            }
            return false;
        }
        [HttpPost]
        public bool LoginCheck(string EMail, string Password)
        {
            Customer Customer = db.Customers.Where(b => b.EMail.Equals(EMail) && b.Password.Equals(Password)).FirstOrDefault();
            Walker Walker = db.Walkers.Where(b => b.EMail.Equals(EMail) && b.Password.Equals(Password)).FirstOrDefault();
            if (Customer != null)
            {
                Session["FName"] = Customer.FName;
                Session["LName"] = Customer.LName;
                Session["EMail"] = Customer.EMail;
                Session["Phone"] = Customer.Phone;
                Session["Password"] = Customer.Password;
                Session["DOB"] = Customer.DOB;
                Session["Address"] = Customer.Address;
                Session["ZipPostal"] = Customer.ZipPostal;
                Session["ID"] = Customer.CustomerID;
                Session["AboutMe"] = Customer.AboutMe;
                Session["IsWalker"] = "no";
                System.Threading.Thread.Sleep(1000);
                return true;
            }
            else if (Walker != null)
            {
                Session["FName"] = Walker.FName;
                Session["LName"] = Walker.LName;
                Session["EMail"] = Walker.EMail;
                Session["Phone"] = Walker.Phone;
                Session["Password"] = Walker.Password;
                Session["DOB"] = Walker.DateOfBirth;
                Session["Address"] = Walker.Address;
                Session["Rating"] = Walker.Rating;
                Session["Experience"] = Walker.Experience;
                Session["ID"] = Walker.WalkerID;
                Session["IsWalker"] = "yes";
                System.Threading.Thread.Sleep(1000);
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public bool VisitProfile(int ID)
        {
            Session["DesiredID"] = ID;
            Customer Customer = db.Customers.Where(b => b.CustomerID.Equals(ID)).FirstOrDefault();
            Walker Walker = db.Walkers.Where(b => b.WalkerID.Equals(ID)).FirstOrDefault();
            if (Customer != null)
            {
                Session["VisitorFName"] = Customer.FName;
                Session["VisitorLName"] = Customer.LName;
                Session["VisitorEMail"] = Customer.EMail;
                Session["VisitorPhone"] = Customer.Phone;
                Session["VisitorPassword"] = Customer.Password;
                Session["VisitorDOB"] = Customer.DOB;
                Session["VisitorAddress"] = Customer.Address;
                Session["VisitorZipPostal"] = Customer.ZipPostal;
                Session["VisitorID"] = Customer.CustomerID;
                Session["VisitorAboutMe"] = Customer.AboutMe;
                Session["VisitorIsWalker"] = "no";
                System.Threading.Thread.Sleep(1000);
                return true;
            }
            else if (Walker != null)
            {
                Session["VisitorFName"] = Walker.FName;
                Session["VisitorLName"] = Walker.LName;
                Session["VisitorEMail"] = Walker.EMail;
                Session["VisitorPhone"] = Walker.Phone;
                Session["VisitorPassword"] = Walker.Password;
                Session["VisitorDOB"] = Walker.DateOfBirth;
                Session["VisitorAddress"] = Walker.Address;
                Session["VisitorRating"] = Walker.Rating;
                Session["VisitorExperience"] = Walker.Experience;
                Session["VisitorID"] = Walker.WalkerID;
                Session["VisitorIsWalker"] = "yes";
                System.Threading.Thread.Sleep(1000);
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public bool PetCreater(string PetName, string PetWeight, string PetAge, string PetBreed, string PetColor)
        {
            Pet newPet = new Pet();
            if (newPet != null)
            {
                newPet.Name = PetName;
                newPet.Weight = PetWeight;
                newPet.Age = PetAge;
                newPet.Braid = PetBreed;
                newPet.Color = PetColor;
                newPet.OwnerID = Convert.ToInt16(Session["ID"]);
                db.Pets.Add(newPet);
                db.SaveChanges();
                Session["PetID"] = newPet.PetID;
                System.Threading.Thread.Sleep(1000);
                var amk = Session["PetID"];
                return true;

            }


            return false;
        }
        [HttpPost]
        public JsonResult GetPet()
        {
            int DesiredID = Convert.ToInt32(Session["DesiredID"]);
            var ListOfPets = new List<string>();
            if (DesiredID == 0)
            {
                int ID = Convert.ToInt16(Session["ID"]);
                var PetData = db.Pets.Where(b => b.OwnerID == (ID)).ToList();
                foreach (var b in PetData)
                {
                    ListOfPets.Add(b.PetID.ToString());
                    ListOfPets.Add(b.Name.ToString());
                    ListOfPets.Add(b.Weight.ToString());
                    ListOfPets.Add(b.Age.ToString());
                    ListOfPets.Add(b.Braid.ToString());
                    ListOfPets.Add(b.Color.ToString());
                }
            }
            else
            {
                int ID = Convert.ToInt16(Session["DesiredID"]);
                var PetData = db.Pets.Where(b => b.OwnerID == (ID)).ToList();
                foreach (var b in PetData)
                {
                    ListOfPets.Add(b.PetID.ToString());
                    ListOfPets.Add(b.Name.ToString());
                    ListOfPets.Add(b.Weight.ToString());
                    ListOfPets.Add(b.Age.ToString());
                    ListOfPets.Add(b.Braid.ToString());
                    ListOfPets.Add(b.Color.ToString());
                }
            }
            return Json(ListOfPets);
        }

        [HttpPost]
        public bool UpdateProfile(string FName, string LName, string EMail, string Phone, string Password, string DOB, string Address, string ZipPostal, string IDNumber,string AboutMe)
        {
            var ID = Convert.ToInt32(Session["ID"]);
            Customer MyCustomer = db.Customers.Where(b => b.CustomerID.Equals(ID)).FirstOrDefault();
            Walker MyWalker = db.Walkers.Where(b => b.WalkerID.Equals(ID)).FirstOrDefault();
            if (MyWalker == null)
            {
                MyCustomer.FName = IsNullChecker(FName) ? (MyCustomer.FName) : (FName);
                MyCustomer.LName = IsNullChecker(LName) ? (MyCustomer.LName) : (LName);
                MyCustomer.EMail = IsNullChecker(EMail) ? (MyCustomer.EMail) : (EMail);
                MyCustomer.Phone = IsNullChecker(Phone) ? (MyCustomer.Phone) : (Phone);
                MyCustomer.Password = IsNullChecker(Password) ? (MyCustomer.Password) : (Password);
                MyCustomer.DOB = IsNullChecker(DOB) ? (MyCustomer.DOB) : (DOB);
                MyCustomer.Address = IsNullChecker(Address) ? (MyCustomer.Address) : (Address);
                MyCustomer.ZipPostal = IsNullChecker(ZipPostal) ? (MyCustomer.ZipPostal) : (ZipPostal);
                MyCustomer.IDNumber = IsNullChecker(IDNumber) ? (MyCustomer.IDNumber) : (IDNumber);
                MyCustomer.AboutMe = IsNullChecker(AboutMe) ? (MyCustomer.AboutMe) : (AboutMe);
                db.SaveChanges();
                return true;
            }
            else if (MyCustomer == null)
            {
                MyWalker.FName = IsNullChecker(FName) ? (MyWalker.FName) : (FName);
                MyWalker.LName = IsNullChecker(LName) ? (MyWalker.LName) : (LName);
                MyWalker.EMail = IsNullChecker(EMail) ? (MyWalker.EMail) : (EMail);
                MyWalker.Phone = IsNullChecker(Phone) ? (MyWalker.Phone) : (Phone);
                MyWalker.Password = IsNullChecker(Password) ? (MyWalker.Password) : (Password);
                MyWalker.DateOfBirth = IsNullChecker(DOB) ? (MyWalker.DateOfBirth) : (DOB);
                MyWalker.Address = IsNullChecker(Address) ? (MyWalker.Address) : (Address);
                MyWalker.AboutMe = IsNullChecker(AboutMe) ? (MyWalker.AboutMe) : (AboutMe);
                db.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public bool IsNullChecker(string var)
        {
            if ((var == null) || (var == "") || (var == " "))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult ProfilePhotoUpdate()
        {
            var ID = Convert.ToInt32(Session["ID"]);
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Images/ProfilePhotos/") + ID + ".png");
                System.IO.File.Delete(path);
                file.SaveAs(path);
                db.SaveChanges();
            }
            return Json(true);
        }
        public ActionResult PetPhotoUpdate()
        {
            var ID = Session["PetID"];
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/Images/Pets/") + ID + ".png");
                System.IO.File.Delete(path);
                file.SaveAs(path);
                db.SaveChanges();
                Session["NewPetID"] = 0;
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult GetPostCount()
        {
            int PostCount = 0;
            var ID = Convert.ToInt32(Session["VisitorID"]);
            if (ID==0)
            {
                 ID = Convert.ToInt32(Session["ID"]);
            }
            if (ID == 0)
            {
                return Json("");
            }
            var IsLiked = db.Posts.Where(b => b.PostOwnerID == (ID)).ToList();
            foreach (var b in IsLiked)
            {
                PostCount++;

            }
            Customer UpdatePostCount = db.Customers.Where(b => b.CustomerID == (ID)).FirstOrDefault();
            UpdatePostCount.PostCount = PostCount;
            db.SaveChanges();
            return Json(PostCount);
        }
        [HttpPost]
        public JsonResult GetFollowCount()
        {

            int FollowCount = 0;
            var ID = Convert.ToInt32(Session["VisitorID"]);
            if (ID==0)
            {
                return Json("");
            }
            if (ID == 0)
            {
                ID = Convert.ToInt32(Session["ID"]);
            }
            var FollowCheck = db.Follows.Where(b => b.Following == (ID)).ToList();
            foreach (var b in FollowCheck)
            {
                FollowCount++;

            }
            Customer UpdatePostCount = db.Customers.Where(b => b.CustomerID == (ID)).FirstOrDefault();
            UpdatePostCount.FollowingCount = FollowCount;
            db.SaveChanges();
            return Json(FollowCount);
        }
        [HttpPost]
        public JsonResult GetFollowerCount()
        {
            int FollowerCount = 0;
            var ID = Convert.ToInt32(Session["VisitorID"]);
            if (ID == 0)
            {
                ID = Convert.ToInt32(Session["ID"]);
            }
            if (ID == 0)
            {
                return Json("");
            }
            var FollowerCheck = db.Follows.Where(b => b.Follower == (ID)).ToList();
            foreach (var b in FollowerCheck)
            {
                FollowerCount++;

            }
            Customer UpdatePostCount = db.Customers.Where(b => b.CustomerID == (ID)).FirstOrDefault();
            UpdatePostCount.FollowerCount = FollowerCount;
            db.SaveChanges();
            return Json(FollowerCount);
        }
        [HttpPost]
        public JsonResult Follow()
        {
            var VisitorID = Convert.ToInt32(Session["DesiredID"]);
            var ID = Convert.ToInt32(Session["ID"]);

            var IsFollow = db.Follows.Where(b => b.Following == (VisitorID)).FirstOrDefault();
            if (IsFollow == null)//takip etmemiştir
            {
                Follow NewFollower = new Follow();
                NewFollower.Follower = ID;
                NewFollower.Following = VisitorID;
                db.Follows.Add(NewFollower);
                db.SaveChanges();
                return Json("Followed");
            }
            else//takip ediyordur
            {
                IsFollow.Follower = null;
                IsFollow.Following = null;
                db.SaveChanges();
                return Json("Unfollowed");
            }
        }
        public JsonResult GetPosts()
        {
            var ListOfPosts = new List<int>();
            var ID = Convert.ToInt32(Session["VisitorID"]);
            if (ID == 0)
            {
                ID = Convert.ToInt32(Session["ID"]);
            }

            var IsFollow = db.Posts.Where(b => b.PostOwnerID == (ID)).ToList();
            foreach (var b in IsFollow) {
                ListOfPosts.Add(b.PostID);
            }
            return Json(ListOfPosts);
        }
        [HttpPost]
        public JsonResult GetMessagesBatch(int SentID)
        {
            var MessageList = new List<string>();
            var CheckData = new List<Message>();
            var ID = Convert.ToInt32(Session["ID"]);
            var CheckedData = db.Messages.Where(b => b.MessageSenderID == (ID) && b.MessageReceiverID == (SentID)).ToList();
            var CheckedReversedData= db.Messages.Where(b => b.MessageSenderID == (SentID) && b.MessageReceiverID == (ID)).ToList();
            CheckData.AddRange(CheckedData);
            CheckData.AddRange(CheckedReversedData);
            var orderedList = CheckData.OrderBy(m => m.MessageDate).ToList();
            if (orderedList != null)
            {
                foreach (var b in orderedList)
                {
                    if (b.MessageSenderID==ID)
                    {
                        MessageList.Add("Me");
                    }
                    else
                    {
                        MessageList.Add("You");
                    }
                    MessageList.Add(b.MessageDate.ToString());
                    MessageList.Add(b.MessageText.Trim());
                }

            }
            return Json(MessageList);
        }

    }
}
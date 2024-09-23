using PetWalker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;

namespace PetWalker.Controllers
{
    public class WalkerController : Controller
    {
        PetWalkerDBEntities db = new PetWalkerDBEntities();
        // GET: Walker
        public ActionResult Search()
        {
            using (var datamodel = new PetWalkerDBEntities())
            {
                var j = datamodel.Walkers.ToList();
                return View(j);
            }
        }
        public ActionResult WalkerProfile()
        {
            using (var datamodel = new PetWalkerDBEntities())
            {
                var ID= Convert.ToInt16(Session["ID"]);
                var j = datamodel.Walkers.Where(b => b.WalkerID == (ID)).ToList();
                return View(j);
            }
        }
        public ActionResult WalkDetail()
        {
            return View();
        }
        public ActionResult InspectWalkerProfile()
        {
            return View();
        }
        [HttpPost]
        public bool AddWalk(string InternalPetID, DateTime WalkDate, string Length, string Price, string WalkerID)
        {
            Walk NewWalk = new Walk();
            if (NewWalk != null)
            {
                NewWalk.Date = WalkDate;
                NewWalk.Length = Length;
                NewWalk.Price = Price;
                NewWalk.WalkerID = Convert.ToInt16(WalkerID);
                NewWalk.CustomerID = Convert.ToInt16(Session["ID"]);
                NewWalk.PetID = Convert.ToInt16(InternalPetID);
                db.Walks.Add(NewWalk);
                db.SaveChanges();
                System.Threading.Thread.Sleep(1000);
                return true;

            }
            return false;
        }
        [HttpPost]
        public JsonResult GetWalk()
        {
            var IsRated = false;
            int ID = Convert.ToInt16(Session["ID"]);
            var ListOfWalks = new List<string>();
            var WalkData = db.Walks.Where(b => b.CustomerID == (ID)).ToList();
            if (Session["IsWalker"] == "yes")
            {
                WalkData = db.Walks.Where(b => b.WalkerID == (ID)).ToList();
            }
            foreach (var b in WalkData)
            {
                ListOfWalks.Add("@"+b.Walker.Longitude.Trim()+"," + b.Walker.Latitude.Trim().ToString());
                ListOfWalks.Add(b.Pet.Name.ToString());
                ListOfWalks.Add(b.Walker.FName + b.Walker.LName.ToString());
                ListOfWalks.Add(b.Length.ToString());
                ListOfWalks.Add(b.Price.ToString());
                ListOfWalks.Add(b.Date.ToString());
                if (b.Rate != null) { IsRated = true; }
                DateTime? dt = b.Date;
                DateTime dt2 = dt.Value;
                dt = dt2.AddMinutes(Convert.ToInt32(b.Length));
                if (dt > DateTime.Now)
                {
                    ListOfWalks.Add("Not Completed");
                }
                else if ((dt < DateTime.Now) && (IsRated == true))
                {
                    ListOfWalks.Add("Rated");
                }
                else if ((dt < DateTime.Now) && (IsRated == false))
                {
                    ListOfWalks.Add("Completed");
                }
                IsRated = false;
                ListOfWalks.Add(b.WalkID.ToString());
            }
            return Json(ListOfWalks);
        }
        [HttpPost]
        public JsonResult GetRating()
        {
            int ID = Convert.ToInt16(Session["ID"]);
            var ListOfRatings = new List<string>();
            var Rating = db.Ratings.Where(b => b.WalkerID == (ID)).ToList();
            foreach (var b in Rating)
            {
                ListOfRatings.Add(b.Customer.FName + b.Customer.LName.ToString());
                ListOfRatings.Add(b.Pet.Name.ToString());
                ListOfRatings.Add(b.Walk.Date.ToString());
                ListOfRatings.Add(b.Walk.Length.ToString());
                ListOfRatings.Add(b.Walk.Price.ToString());
                ListOfRatings.Add(b.WalkRate.ToString());
                ListOfRatings.Add(b.RateComment.ToString());
            }
            return Json(ListOfRatings);
        }
        [HttpPost]
        public JsonResult GetPetNames()
        {
            int ID = Convert.ToInt16(Session["ID"]);
            var PetNames = new List<string>();
            var PetData = db.Pets.Where(b => b.OwnerID == (ID)).ToList();
            foreach (var b in PetData)
            {
                PetNames.Add(b.PetID.ToString());
                PetNames.Add(b.Name.ToString());
            }
            return Json(PetNames);
        }
        [HttpPost]
        public JsonResult GetAchievements()
        {
            var ID = Convert.ToInt16(Session["ID"]);
            var AchievementList = new List<string>();
            var Achievement = db.Achievements.Where(b => b.WalkerID == (ID)).ToList();
            foreach (var b in Achievement)
            {
                AchievementList.Add(b.AchievementNumber.ToString());
            }
            return Json(AchievementList);
        }
        [HttpPost]
        public bool SendRating(int WalkID, string Comments, string Rate)
        {
            Walk LocateData = db.Walks.Where(b => b.WalkID == (WalkID)).FirstOrDefault();
            Rating NewRating = new Rating();
            if (NewRating != null)
            {
                LocateData.Rate = Convert.ToInt32(Rate);
                NewRating.WalkID = WalkID;
                NewRating.WalkerID = LocateData.WalkerID;
                NewRating.CustomerID = LocateData.CustomerID;
                NewRating.WalkRate = Convert.ToInt32(Rate);
                NewRating.RateComment = Comments;
                NewRating.PetID = LocateData.PetID;
                db.Ratings.Add(NewRating);
                db.SaveChanges();
                System.Threading.Thread.Sleep(1000);
                return true;

            }


            return false;
        }
        [HttpPost]
        public bool WalkerConditioner()
        {
            int Rate = 0;
            int WalkCount = 0;
            int ID = Convert.ToInt32(Session["ID"]);
            bool Ach1=false;
            bool Ach2=false;
            bool Ach3 = false;
            bool Ach4 = false;
            var CheckWalks = db.Walks.Where(b => b.WalkerID == (ID)).ToList();
            foreach (var b in CheckWalks)
            {
                Rate += Convert.ToInt32(b.Rate);
                WalkCount++;
            }
            if (WalkCount != 0)
            {
                Rate = Rate / WalkCount;
                Walker ActiveWalker = db.Walkers.Where(b => b.WalkerID.Equals(ID)).FirstOrDefault();
                ActiveWalker.Rating = Rate.ToString();
                ActiveWalker.Experience = WalkCount.ToString();
                Achievement giveAchievement = new Achievement();
                var checkAchievement = db.Achievements.Where(b => b.WalkerID.Equals(ID)).ToList();
                foreach (var c in checkAchievement)
                {
                    if (c.AchievementNumber == 1) { Ach1 = true; }
                    if (c.AchievementNumber == 2) { Ach2 = true; }
                    if (c.AchievementNumber == 3) { Ach3 = true; }
                    if (c.AchievementNumber == 4) { Ach4 = true; }
                }
                if ((WalkCount > 1) && (Ach1 == false))
                {
                    giveAchievement.WalkerID = ID;
                    giveAchievement.AchievementNumber = 1;
                    db.Achievements.Add(giveAchievement);
                    db.SaveChanges();
                }
                if ((WalkCount > 2) && (Ach2 == false))
                {
                    giveAchievement.WalkerID = ID;
                    giveAchievement.AchievementNumber = 2;
                    db.Achievements.Add(giveAchievement);
                    db.SaveChanges();
                }
                if ((WalkCount > 5) && (Ach3 == false))
                {
                    giveAchievement.WalkerID = ID;
                    giveAchievement.AchievementNumber = 3;
                    db.Achievements.Add(giveAchievement);
                    db.SaveChanges();
                }
                if ((Rate > 3) && (Ach4 == false))
                {
                    giveAchievement.WalkerID = ID;
                    giveAchievement.AchievementNumber = 4;
                    db.Achievements.Add(giveAchievement);
                    db.SaveChanges();
                }
                db.SaveChanges();
            }
           
            Rate = 0;
            return false;
        }
        [HttpPost]
        public bool SendGPS(string latitude, string longitude)
        {
            var ID = Convert.ToInt32(Session["ID"]);    
            Walker UpdateLatLong = db.Walkers.Where(b => b.WalkerID == (ID)).FirstOrDefault();
            if (UpdateLatLong != null)
            {
                UpdateLatLong.Latitude = latitude;
                UpdateLatLong.Longitude = longitude;
                db.SaveChanges();
            }
            return true;
        }
    }
}
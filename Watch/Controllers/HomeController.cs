using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Watch.Models;
using PagedList;
using System.Net.Mail;
using System.Net;

namespace Watch.Controllers
{
    public class HomeController : Controller
    {
        string conn = ConfigurationManager.ConnectionStrings["aspnet_Watch_backConnectionString"].ToString();
        private UsersContext db = new UsersContext();




        public ActionResult Autocomplete(string term)
        {
            

            var model = (new List<string>())
                .Concat(db.Products
                    .Where(u => (u.Name).Contains(term))
                    .Select(u => (u.Name))
                    .Distinct())
                .Concat(db.Products
                    .Where(u => (u.ProductId).Contains(term))
                    .Select(u => (u.ProductId))
                    .Distinct())
                .Concat(db.Products
                    .Where(u => (u.Category).Contains(term))
                    .Select(u => (u.Category))
                    .Distinct())
                .ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Index()
        {
            // ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            string filename = "";
            //using (SqlConnection con = new SqlConnection(conn))
            //{
            //    con.Open();
            //    SqlCommand cmd = new SqlCommand("select FileName FROM FileDetails where Id='" + id + "'", con);
            //    //cmd.ExecuteNonQuery();

            //    SqlDataReader reader = cmd.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        filename = reader["FileName"].ToString();
            //    }

            //    con.Close();
            //}
           
            List<MainImage> product;
            //product = db.MainImage.Where(x => x.ProductId == 6).Take(5).ToList();

            return View(db.MainImage.OrderByDescending(v => v.ProductId).Take(5).ToList());
        }
        

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }


        [HttpPost]
        public ActionResult About(HttpPostedFileBase file)
        {
            
            return RedirectToAction("Index", "Home");
        }

       

        public ActionResult Contact()
        {
            //ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            
            ViewModel vmDemo = new ViewModel();
           

           vmDemo.vMainImage = (from b in db.MainImage
                                    where b.ProductId == id
                                    select b).ToList();

           vmDemo.vSubImages = (from a in db.SubImages
                                where a.ProductId == id
                                select a).ToList();
           vmDemo.vProduct = (from a in db.Products
                              where a.Id == id
                              select a).ToList();
            //product = db..Where(x => x.ProductId == id).Take(5).ToList();

           return View(vmDemo);
        }


        public ActionResult Details(string wid, string name, string email, string content)
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Details()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult ProductDetails(string id)
        {
            
            return Json(new
            {
                redirectUrl = Url.Action("Details"),
                isRedirect = true
            });
            //return RedirectToAction("Details");
        }


        [HttpPost]
        public JsonResult ProductCategory(int id)
        {

            return Json(new { Result = "OK" });
        }




        [HttpGet]
        public ActionResult Product(int page = 1)
        {

            ViewBag.CategoryList = new SelectList(db.Products, "Id", "ProductId");
            return View(db.MainImage.OrderByDescending(v => v.ProductId).ToPagedList(page, 8));
        }

        [HttpPost]
        public ActionResult Product(string SearchTerm, string Value, string CategoryList)
        {
            ViewBag.CategoryList = new SelectList(db.Products, "Id", "ProductId");
            string category = "";
            if (Value == "0")
            {
                category = "Men's";
            }
            else if (Value == "1")
            {
                category = "Lady's";
            }
            else if (Value == "2")
            {
                category = "Chronograph";
            }
            else if (Value == "3")
            {
                category = "Sport";
            }


            List<MainImage> product;
            if (SearchTerm == "" && Value == "" && CategoryList == "")
            {
                product = db.MainImage.ToList();
            }
            else if (SearchTerm == "" && Value != "" && CategoryList == "")
            {
                //product = db.Products.Where(x => x.Name.StartsWith(SearchTerm)).ToList();
                product = db.MainImage.Where(x => x.Product.Category == category).ToList();
            }
            else if (SearchTerm != "" && Value == "" && CategoryList == "")
            {
           //     var model = (new List<string>())
           //    .Concat(db.Products
           //.Where(u => (u.Name).Contains(SearchTerm))
           //.Select(u => (u.Name))
           //.Distinct()).ToList();

                //return Json(model, JsonRequestBehavior.AllowGet);

                string pname = (from s in db.Products
                               where s.Name == SearchTerm
                               select s.Name).FirstOrDefault() ?? string.Empty;

                string pid = (from s in db.Products
                              where s.ProductId == SearchTerm
                              select s.ProductId).FirstOrDefault() ?? string.Empty;

                string pcad = (from s in db.Products
                              where s.Category == SearchTerm
                               select s.Category).FirstOrDefault() ?? string.Empty;


                if (pname != "")
                {
                    product = db.MainImage.Where(x => x.Product.Name.StartsWith(SearchTerm)).ToList();
                }
                else if (pid != "")
                {
                    product = db.MainImage.Where(x => x.Product.ProductId.StartsWith(SearchTerm)).ToList();
                }
                else if (pcad != "")
                {
                    product = db.MainImage.Where(x => x.Product.Category.StartsWith(SearchTerm)).ToList();
                }

                else
                {
                    product = db.MainImage.ToList();
                }

               
            }
            else if (SearchTerm != "" && Value != "" && CategoryList == "")
            {
                product = db.MainImage.Where(x => x.Product.Name.StartsWith(SearchTerm) && x.Product.Category == category).ToList();
            }
            else if (SearchTerm == "" && Value == "" && CategoryList != "")
            {
                int id = Convert.ToInt16(CategoryList);
                //ViewBag.CategoryList = new SelectList(db.Products, "Id", "ProductId");
                product = db.MainImage.Where(x => x.ProductId == id).ToList();
            }

            else
            {
                product = db.MainImage.ToList();
            }
            int page = 1;

            //db.Products.OrderByDescending(v => v.Id).ToPagedList(page, 6

            return View(product.OrderByDescending(v => v.Id).ToPagedList(page, 8));
        }


        [HttpGet]
        public ActionResult Warranty()
        {
            return View();
        }


         [HttpPost]
        public ActionResult Warranty(WarrantyReg warrantyreg)
        {
            if (ModelState.IsValid)
            {
                string name = (from s in db.WarrantyRegs
                               where s.Name == warrantyreg.Name
                               select s.Name).FirstOrDefault() ?? string.Empty;
                string addrerss = (from s in db.WarrantyRegs
                                   where s.Address == warrantyreg.Address
                                   select s.Address).FirstOrDefault() ?? string.Empty;

                string moNo = (from s in db.WarrantyRegs
                               where s.ModelNo == warrantyreg.ModelNo
                               select s.ModelNo).FirstOrDefault() ?? string.Empty;

                string country = (from s in db.WarrantyRegs
                                  where s.Country == warrantyreg.Country
                                  select s.Country).FirstOrDefault() ?? string.Empty;

                string dealer = (from s in db.WarrantyRegs
                                 where s.Dealer == warrantyreg.Dealer
                                 select s.Dealer).FirstOrDefault() ?? string.Empty;

                if (name != "" && addrerss != "" && moNo != "" && country != "" && dealer != "")
                {
                    ViewBag.Error = "You have already Registerd.!!";
                }
                else
                {
                    //sendContactFormDetails(warrantyreg.Name, warrantyreg.Address, warrantyreg.ModelNo, warrantyreg.Dealer, warrantyreg.Country, warrantyreg.Date);
                    db.WarrantyRegs.Add(warrantyreg);
                    db.SaveChanges();
                    //return RedirectToAction("Index");
                    ViewBag.Message = "Ok";
                }
            }
            warrantyreg.Name = string.Empty;
            warrantyreg.Address = string.Empty;
            warrantyreg.Country = string.Empty;
            warrantyreg.Dealer = string.Empty;
            warrantyreg.ModelNo = string.Empty;


            
           

            ModelState.Clear();
            return View(warrantyreg);
        }

      
      

        public bool sendContactFormDetails(string name, string address, string mNo, string dealer, string country, string date)
        {


            string Email = "dinesh.priyantha.uom@gmail.com";
            MailMessage message;
            message = new MailMessage();
            message.To.Add(Email);
            // message.CC.Add("dineshpriyantha488@gmail.com");
            message.Subject = name;
            message.From = new MailAddress("dineshpriyantha488@gmail.com");
            //string path = System.Web.HttpContext.Current.Server.MapPath(@"Images/logo.png");
            //LinkedResource logo = new LinkedResource(path);
            //logo.ContentId = "companylogo";


            message.Body =

                "<br/>" +
                           "Name : " + name +
                           "<br/>Address : " + address + "<br/>" +
                           "<br/>Model No : " + mNo + "<br/>" +
                           "<br/>Dealer : " + dealer + "<br/>" +
                           "<br/>Country : " + country + "<br/>" +
                           "<br/>Date : " + date + "<br/>";
                          

            message.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.EnableSsl = false;

            smtpClient.Host = "Smtp.gmail.com";  //relay-hosting.secureserver.net
            smtpClient.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("dineshpriyantha488@gmail.com", "pass");
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = NetworkCred;
            smtpClient.Port = 587;   //25
            smtpClient.Send(message);
            return true;


        }
    }
}

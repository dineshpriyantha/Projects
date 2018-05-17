using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Watch.Models;
using PagedList;
using System.Configuration;
using System.Data.SqlClient;

namespace Watch.Controllers
{
     [Authorize]
    public class ProductController : Controller
    {
        private UsersContext db = new UsersContext();
        string conn = ConfigurationManager.ConnectionStrings["aspnet_Watch_backConnectionString"].ToString();



        public ViewResult Warranty(int page = 1)
        {
            
            return View(db.WarrantyRegs.OrderByDescending(v => v.Id).ToPagedList(page, 10));
        }


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
        //
        // GET: /Support/

        public ViewResult Index(int page = 1)
        {
            //var products = from s in db.Products
            //               select s;
            //int pageSize = 1;
            //int pageNumber = (page ?? 1);
            //return View(products.ToPagedList(pageNumber, pageSize));
             
            ViewBag.CategoryList = new SelectList(db.Products, "Id", "ProductId");
            return View(db.Products.OrderByDescending(v => v.ProductId).ToPagedList(page, 6));
            //return View(db.Products.ToList());
        }

        [HttpPost]
        public ActionResult Index(string SearchTerm, string Value, string CategoryList, int? page)
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

            
            List<Product> product;
            if (SearchTerm == "" && Value == "" && CategoryList=="")
            {
                product = db.Products.ToList();
            }
            else if (SearchTerm == "" && Value != "" && CategoryList == "")
            {
                //product = db.Products.Where(x => x.Name.StartsWith(SearchTerm)).ToList();
                product = db.Products.Where(x => x.Category == category).ToList();
            }
            else if (SearchTerm != "" && Value == "" && CategoryList == "")
            {
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
                    product = db.Products.Where(x => x.Name.StartsWith(SearchTerm)).ToList();
                }
                else if (pid != "")
                {
                    product = db.Products.Where(x => x.ProductId.StartsWith(SearchTerm)).ToList();
                }
                else if (pcad != "")
                {
                    product = db.Products.Where(x => x.Category.StartsWith(SearchTerm)).ToList();
                }

                else
                {
                    product = db.Products.ToList();
                }
            }
            else if (SearchTerm != "" && Value != "" && CategoryList == "")
            {
                product = db.Products.Where(x => x.Name.StartsWith(SearchTerm) && x.Category == category).ToList();
            }
            else if (SearchTerm == "" && Value == "" && CategoryList != "")
            {
                int id = Convert.ToInt16(CategoryList);
                //ViewBag.CategoryList = new SelectList(db.Products, "Id", "ProductId");
                product = db.Products.Where(x => x.Id == id).ToList();

            }

            else
            {
                product = db.Products.ToList();
            }
            //int page = 1;
            int pageNumber = (page ?? 1);
            //db.Products.OrderByDescending(v => v.Id).ToPagedList(page, 6

            return View(product.OrderByDescending(v => v.Id).ToPagedList(pageNumber, 6));
        }


      
 
       
        //
        // GET: /Support/Details/5

        public ActionResult Details(int id = 0)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        //
        // GET: /Support/Create

        public ActionResult Create()
        {
            var listItems = new SelectListItem[]{
                            new SelectListItem(){Text ="Men's", Value="Men's"},
                            new SelectListItem(){Text = "Lady's", Value = "Lady's"},
                            new SelectListItem(){Text = "Chronograph " , Value = "Chronograph"},
                            new SelectListItem(){Text = "Sport ", Value = "Sport"},
            };
            ViewBag.CategoryName = listItems;

            return View();
        }

        // 
        // POST: /Support/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            string pId = (from tb in db.Products
                                where tb.ProductId == product.ProductId
                                select tb.ProductId).FirstOrDefault() ?? string.Empty;

            if (pId != "")
            {
                TempData["msg"] = "<script>alert('Can't Add Duplicate Watch Id..!!');</script>";
                ViewBag.Message = "Can't Add Duplicate Watch Id..!!";
                var listItems = new SelectListItem[]{
                            new SelectListItem(){Text ="Men's", Value="Men's"},
                            new SelectListItem(){Text = "Lady's", Value = "Lady's"},
                            new SelectListItem(){Text = "Chronograph " , Value = "Chronograph"},
                            new SelectListItem(){Text = "Sport ", Value = "Sport"},
            };
                ViewBag.CategoryName = listItems;
            }
            else
            {

                if (ModelState.IsValid)
                {
                    int count = 1;
                    List<MainImage> mainImages = new List<MainImage>();
                    List<SubImages> subImages = new List<SubImages>();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];

                        if (file != null && file.ContentLength > 0)
                        {
                            //string prepend = "item";
                            var fileName = Path.GetFileName(product.ProductId + count);

                            string fileExtention1 = System.IO.Path.GetExtension(file.FileName);
                            if (fileExtention1.ToLower() != ".jpg" && fileExtention1.ToLower() != ".png")
                            {
                                //TempData["msg"] = "<script>alert('Change succesfully');</script>";
                                ViewData["Name"] = "<script language='javascript' type='text/javascript'>alert('Use Only .jpg and .png Images');</script>";
                                return RedirectToAction("Index");
                                // return Content("<script language='javascript' type='text/javascript'>alert('Use Only .jpg and .png Images');</script>");
                                //return RedirectToAction("Index"); 
                            }

                            else
                            {
                                char last_char = fileName[fileName.Length - 1];

                                if (Convert.ToString(last_char) == "1")
                                {
                                    MainImage mainimages = new MainImage()
                                    {
                                        FileName = fileName,
                                        Extension = Path.GetExtension(file.FileName),

                                        //Id = Guid.NewGuid()

                                    };
                                    mainImages.Add(mainimages);

                                }
                                else if (Convert.ToString(last_char) != "1")
                                {
                                    SubImages subimages = new SubImages()
                                    {
                                        ImageName = fileName,
                                        Extension = Path.GetExtension(file.FileName),
                                        //MainImageId = product.Id,
                                    };
                                    //db.SubImages.Add(subimages);
                                    //db.SaveChanges();
                                    subImages.Add(subimages);

                                    //using (SqlConnection con = new SqlConnection(conn))
                                    //{
                                    //    con.Open();
                                    //    SqlCommand cmd = new SqlCommand("INSERT INTO SubImages VALUES('" + fileName + "','" + fileExtention1 + "','"++"')'", con);
                                    //    cmd.ExecuteNonQuery();
                                    //    con.Close();
                                    //}



                                }


                                var path = Path.Combine(Server.MapPath("~/Images/upload/"), fileName + fileExtention1);
                                file.SaveAs(path);
                                count++;
                            }
                        }

                    }
                    //MainImage mainimage = new MainImage();

                    //mainimage.SubImages = subImages;

                    product.MainImages = mainImages;
                    product.SubImages = subImages;
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(product);

        }

        private bool ValidateExtension(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".gif":
                    return true;
                case ".jpeg":
                    return true;
                default:
                    return false;
            }
        }

        //
        // GET: /Support/Edit/5

        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Include(s => s.MainImages).SingleOrDefault(x => x.Id == id);

           

            if (product == null)
            {
                return HttpNotFound();
            }


            string query = (from s in db.Products
                            where s.Id == id
                            select s.Category).FirstOrDefault() ?? string.Empty;
                        

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Men's", Value = "Men's" });
            items.Add(new SelectListItem { Text = "Lady's", Value = "Lady's" });
            items.Add(new SelectListItem { Text = "Chronograph", Value = "Chronograph" });
            items.Add(new SelectListItem { Text = "Sport", Value = "Sport" });

            ViewBag.SelectedItem = query;
            ViewBag.SearchStatusList = items;


            return View(product);
        }

       

        public FileResult Download(String p, String d)
        {
            return File(Path.Combine(Server.MapPath("~/Images/upload/"), p), System.Net.Mime.MediaTypeNames.Application.Octet, d);
        }

        //
        // POST: /Support/Edit/5

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            string query = (from s in db.MainImage
                            where s.ProductId == product.Id
                            select s.FileName).FirstOrDefault() ?? string.Empty;

            int count = 1;
            if (query == "")
            {
                count = 1;
            }
            else
            {
                count = 2;

            }

            if (ModelState.IsValid)
            {

                //New Files
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(product.ProductId + count);

                        string fileExtention1 = System.IO.Path.GetExtension(file.FileName);
                        //var fileName = Path.GetFileName(file.FileName);
                        if (fileExtention1.ToLower() != ".jpg" && fileExtention1.ToLower() != ".png")
                        {
                            //TempData["msg"] = "<script>alert('Change succesfully');</script>";
                            ViewData["Name"] = "<script language='javascript' type='text/javascript'>alert('Use Only .jpg and .png Images');</script>";
                            return RedirectToAction("Index");
                            // return Content("<script language='javascript' type='text/javascript'>alert('Use Only .jpg and .png Images');</script>");
                            //return RedirectToAction("Index"); 
                        }
                        else
                        {
                            char last_char = fileName[fileName.Length - 1];

                            if (query == "" && Convert.ToString(last_char) == "1")
                            {
                                MainImage mainImage = new MainImage()
                                {
                                    FileName = fileName,
                                    Extension = Path.GetExtension(file.FileName),
                                    //Id = Guid.NewGuid(),
                                    ProductId = product.Id
                                };
                                db.Entry(mainImage).State = EntityState.Added;
                            }
                            else if (Convert.ToString(last_char) != "1")
                            {
                                //count++;
                                SubImages subImages = new SubImages()
                                {
                                    ImageName = fileName,
                                    Extension = Path.GetExtension(file.FileName),
                                    ProductId = product.Id
                                };
                                db.Entry(subImages).State = EntityState.Added;
                            }
                            //var path = Path.Combine(Server.MapPath("~/Images/upload/"), fileName + fileExtention1);
                            var path = Path.Combine(Server.MapPath("~/Images/upload/"), fileName + fileExtention1);
                            file.SaveAs(path);

                           
                            
                            count++;
                        }
                    }
                }

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }




        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {
                

               
                string filename = "";
                string fileExtention = "";
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select FileName,Extension FROM MainImages where Id='" + id + "'", con);
                    //cmd.ExecuteNonQuery();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        filename = reader["FileName"].ToString();
                        fileExtention = reader["Extension"].ToString();
                    }

                    con.Close();
                }

                var path = Path.Combine(Server.MapPath("~/Images/upload/"), filename + fileExtention);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM MainImages where Id='" + id + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }


               
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult DeleteSubFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Result = "Error" });
            }
            try
            {



                string filename = "";
                string fileExtention = "";
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select ImageName,Extension FROM SubImages where Id='" + id + "'", con);
                    //cmd.ExecuteNonQuery();

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        filename = reader["ImageName"].ToString();
                        fileExtention = reader["Extension"].ToString();
                    }

                    con.Close();
                }

                var path = Path.Combine(Server.MapPath("~/Images/upload/"), filename + fileExtention);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM SubImages where Id='" + id + "'", con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }



                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }



        //
        // POST: /Support/Delete/5

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }

                //delete files from the file system

                foreach (var item in product.MainImages)
                {
                    String path = Path.Combine(Server.MapPath("~/Images/upload/"), item.FileName + item.Extension);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }


                foreach (var item in product.SubImages)
                {
                    String path = Path.Combine(Server.MapPath("~/Images/upload/"), item.ImageName + item.Extension);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult DeleteRegister(int id)
        {
            try
            {
                WarrantyReg war = db.WarrantyRegs.Find(id);
                if (war == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Result = "Error" });
                }


                db.WarrantyRegs.Remove(war);
                db.SaveChanges();
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
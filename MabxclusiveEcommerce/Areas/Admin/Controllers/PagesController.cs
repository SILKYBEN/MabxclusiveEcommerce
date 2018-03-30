using MabxclusiveEcommerce.Models.Data;
using MabxclusiveEcommerce.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MabxclusiveEcommerce.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declare list of PageVM
            List<PageVM> pagesList;

            //Initialize the List

            using (Db db = new Db())
            {
                //init the list
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

                //Return View with List
                return View(pagesList);
        }
        // GET: Admin/Pages/addPage
        [HttpGet]
        public ActionResult addPage()
        {
            return View();
        }

        // POST: Admin/Pages/addPage
        [HttpPost]
        public ActionResult addPage(PageVM model)
        {

            // Check Model state

            if ( ! ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {



                // Declare the slug
                string slug;

                // initialize the pageDTO

                PageDTO dto = new PageDTO();

                // DTO title

                dto.Title = model.Title;


                // check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // make sure Title and Slug are unique
                if ( db.Pages.Any (x => x.Title == model.Title) || db.Pages.Any (x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "Title or Slug Already Exists!");
                    return View(model);
                }

                // DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                // save DTO
                db.Pages.Add(dto);
                db.SaveChanges();

            }

            // Set TempData message
            TempData["SuccessMessage"] = "You have added a new Page!";




            // Redirect
            return RedirectToAction("addPage");
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage( int id)
        {
            // Declare pageVM
            PageVM model;

            using (Db db = new Db ())
            {
                // Get the Page
                PageDTO dto = db.Pages.Find(id);

                // Confirm page exists
                if ( dto == null)
                {
                    return Content("This Page Does Not exist");
                }

                // Initialize pageVM
                model = new PageVM ( dto);
            }
            // Return View with model
            return View( model);
        }

        // POST: Admin/Pages/EditPage/id
        public ActionResult EditPage( PageVM model)
        {
            // Check model state
            if ( !ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                
                // Get Page Id
                int id = model.Id;

                // Declare the slug
                string slug = "home";

                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // DTO the title
                dto.Title = model.Title;

                // Check for slug and set it if need be
                if (  model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }
                // Make sure Title and Slug are unique
                if (db.Pages.Where (x => x.Id != id).Any( x => x.Title == model.Title) || 
                    db.Pages.Where (x => x.Id != id).Any( x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Title or Slug Already Exists!");
                    return View(model);
                }

                // DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // Save DTO
                db.SaveChanges();
            }



            // Set TempData message
            TempData["SuccessMessage"] = "You have editted a Page!";

            // Redirect
            return RedirectToAction("EditPage");
            
            
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails( int id)
        {
            //Declare PageVM
            PageVM model;

            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //confirm page exists
                if ( dto == null)
                {
                    return Content("This Page does not exist!");
                }

                // initialize PageVM
                model = new PageVM(dto);



            }
            //return View with Model
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage( int id)
        {
            using (Db db = new Db())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //remove the page
                db.Pages.Remove(dto);
                //save
                db.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        // POST: Admin/Pages/ReorderPages

            [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                // Set initial count
                int count = 1;

                // Declare PageDTO
                PageDTO dto;

                // Set sorting for each page  
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }

        }

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            // Declare the Model
            SidebarVM model;

            using (Db db = new Db())
            {
                // Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                // Initialize the Model
                model = new SidebarVM(dto);
            }



            //Return the view in model
            return View(model);
        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {
                // Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);


                //DTO the body
                dto.Body = model.Body;

                //Save
                db.SaveChanges();

                // Set the tempdata message
                TempData["SuccessMessage"] = "You have editted the Sidebar!";

               

            }

            //Redirect
            // Redirect
            return RedirectToAction("EditSidebar");

            
        }
    }
}
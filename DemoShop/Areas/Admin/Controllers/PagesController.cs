using DemoShop.Models.Data;
using DemoShop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoShop.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageList;
            using (DContext db = new DContext())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            return View(pageList);
        }

        // GET: Admin/AddPage
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (DContext db = new DContext())
            {
                string slug;
                PageDTO dto = new PageDTO();
                dto.Title = model.Title;

                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                if (db.Pages.Any(x=>x.Title == model.Title) || db.Pages.Any(x=>x.Slug == slug))
                {
                    ModelState.AddModelError("", "This title already is taken");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "You have added a new page!";

            return RedirectToAction("AddPage");
        }

        // GET: Admin/EditPage/id 
        public ActionResult EditPage(int id)
        {
            PageVM model;
            using (DContext db = new DContext())
            {
                PageDTO dto = db.Pages.Find(id);
                if (dto == null)
                {
                    return Content("The Page does not exits");
                }
                model = new PageVM(dto);
                
            }
            return View(model);
        }

        // POST: Admin/EditPage/id 
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (DContext db = new DContext())
            {
                int id = model.Id;
                string slug = "home";
                PageDTO dto = db.Pages.Find(id);
                dto.Title = model.Title;

                if (model.Slug != "home")
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

                if (db.Pages.Where(x=>x.Id != id).Any(x=>x.Title == model.Title) || db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Title or slug already is taken");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;

                db.SaveChanges();
            }
            TempData["SM"] = "You have edited the page";

            return RedirectToAction("EditPage");
        }

        // GET: Admin/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            PageVM model;
            using (DContext db = new DContext())
            {
                PageDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("The page does not exist");
                }
                model = new PageVM(dto);
            }
            return View(model);
        }

        // GET: Admin/DeletePage/id (script yazdım indexte)
        public ActionResult DeletePage(int id)
        {
            using (DContext db = new DContext())
            {
                PageDTO dto = db.Pages.Find(id);
                db.Pages.Remove(dto);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        //POST Admin/Pages/ReorderPages (script yazdım indexte)
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (DContext db = new DContext())
            {
                int count = 1;
                PageDTO dto;

                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;
                    db.SaveChanges();

                    count++;
                }
            }
        }

        //GET: Admin/Pages/EditSidebar
        public ActionResult EditSidebar()
        {
            SidebarVM model;
            using (DContext db = new DContext())
            {
                SidebarDTO dto = db.Sidebar.Find(1); //already nows we created

                model = new SidebarVM(dto);
            }
            return View(model);
        }


        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (DContext db = new DContext())
            {
                SidebarDTO dto = db.Sidebar.Find(1);

                dto.Body = model.Body;

                db.SaveChanges();
            }
            TempData["SM"] = "You edited the Sidebar";

            return RedirectToAction("EditSidebar");
        }

    }
}
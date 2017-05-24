using DemoShop.Models.Data;
using DemoShop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoShop.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            if (page == "")
                page = "home";
            
            PageVM model;
            PageDTO dto;
            using (DContext db = new DContext())
            {
                if (!db.Pages.Any(x=>x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            using (DContext db = new DContext())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }
            ViewBag.PageTitle = dto.Title;
            if (dto.HasSideBar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }
            model = new PageVM(dto);


            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            List<PageVM> pageVmList;
            using (DContext db = new DContext())
            {
                pageVmList = db.Pages.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Where(x => x.Slug != "home")
                    .Select(x => new PageVM(x)).ToList();
            }

            return PartialView(pageVmList);
        }

        public ActionResult SidebarPartial()
        {
            SidebarVM model;
            using (DContext db = new DContext())
            {
                SidebarDTO dto = db.Sidebar.Find(1);
                model = new SidebarVM(dto);
            }

            return PartialView(model);
        }
    }
}
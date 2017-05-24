using DemoShop.Models.Data;
using DemoShop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoShop.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop/Index  bu direk ana sayfaya gidiyor.orada da zaten routingde mevcut
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            List<CategoryVM> categoryVmList;

            using (DContext db = new DContext())
            {
                categoryVmList = db.Categories.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x)).ToList();
            }

            return PartialView(categoryVmList);
        }

        //GET Shop/Index/name (sol taraftaki categoryler)
        public ActionResult Category(string name)
        {
            List<ProductVM> productVmList;

            using (DContext db = new DContext())
            {
                CategoryDTO catDto = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = catDto.Id;

                productVmList = db.Products.ToArray()
                    .Where(x => x.CategoryId == catId)
                    .Select(x => new ProductVM(x)).ToList();

                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();
                ViewBag.CategoryName = productCat.CategoryName;
            }

            return View(productVmList);
        }

        //GET Shop/product-petails/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            ProductVM model;
            ProductDTO dto;

            int id = 0;
            using (DContext db = new DContext())
            {
                if (!db.Products.Any(x=>x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                id = dto.Id;

                model = new ProductVM(dto);
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));

            return View("ProductDetails", model);
        }


    }
}
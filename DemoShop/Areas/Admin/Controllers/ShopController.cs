using DemoShop.Models.Data;
using DemoShop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoShop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            List<CategoryVM> categoryVmList;

            using (DContext db = new DContext())
            {
                categoryVmList = db.Categories.ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x)).ToList();
            }

            return View(categoryVmList);
        }

        // POST: Admin/Shop/AddNewCategory (script yazdım Categories.cshtml e)
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using (DContext db = new DContext())
            {
                if (db.Categories.Any(x=>x.Name == catName))
                    return "titletaken";

                CategoryDTO dto = new CategoryDTO();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();
                return id;
                
            }
        }

        //POST Admin/Shop/ReorderCategories (script yazdım Categories.cshtml e)
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (DContext db = new DContext())
            {
                int count = 1;
                CategoryDTO dto;

                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;
                    db.SaveChanges();

                    count++;
                }
            }
        }

        // GET: Admin/Shop/DeleteCategory/id (script yazdım Categories.cshtml e)
        public ActionResult DeleteCategory(int id)
        {
            using (DContext db = new DContext())
            {
                CategoryDTO dto = db.Categories.Find(id);
                db.Categories.Remove(dto);
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }

        //POST Admin/Shop/RenameCategory (script yazdım Categories.cshtml e)
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (DContext db = new DContext())
            {
                if (db.Categories.Any(x=>x.Name == newCatName))
                {
                    return "titletaken";
                }
                CategoryDTO dto = db.Categories.Find(id);

                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                db.SaveChanges();
            }
            return "ok";
        }

        //Get Admin/Shop/AddProduct
        public ActionResult AddProduct()
        {
            ProductVM model = new ProductVM();
            using (DContext db = new DContext())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }
            return View(model);
        }
    }
}
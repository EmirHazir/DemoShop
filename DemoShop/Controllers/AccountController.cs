using DemoShop.Models.Data;
using DemoShop.Models.ViewModels.Account;
using DemoShop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DemoShop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }

        // GET: /account/login
        public ActionResult Login()
        {
            string userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");
            return View();
        }

        // POST: /account/login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValid = false;

            using (DContext db = new DContext())
            {
                if (db.Users.Any(x=>x.UserName.Equals(model.UserName)
                                 && x.Password.Equals(model.Password)))
                {
                    isValid = true;
                }
            }
            if (!isValid)
            {
                ModelState.AddModelError("", "Username or password is wrong!");
                return View(model);
            }
            else
            {
                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe); //cookiye ekledim.
                return Redirect(FormsAuthentication.GetRedirectUrl(model.UserName, model.RememberMe));
            }

        }

        //Get : /account/logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/account/login");
        }

        // GET: /account/create-account
        [ActionName("create-account")]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: /account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            #region Check IsValid
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Passwords uyuşmuyor!");
                return View("CreateAccount", model);
            }
            #endregion

            using (DContext db = new DContext())
            {
                if (db.Users.Any(x=>x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("","Error!"+model.UserName+ " already taken! Tyr another one.");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }

                UserDTO userDto = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    UserName = model.UserName,
                    Password = model.Password
                };
                db.Users.Add(userDto);
                db.SaveChanges();

                int id = userDto.Id;

                UserRolesDTO userRolesDTO = new UserRolesDTO()
                {
                    UserId = id,
                    RoleId = 2 // normal user
                };
                db.UserRoles.Add(userRolesDTO);
                db.SaveChanges();
            }
            TempData["SM"] = "You are now registered and can login!";


            return Redirect("~/account/login");
        }

        //User navbarpartial
        [Authorize]
        public ActionResult UserNavPartial()
        {
            string userName = User.Identity.Name;
            UserNavPartialVM model;
            using (DContext db = new DContext())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            return PartialView(model);
        }

        //Get /account/user-profile
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            string userName = User.Identity.Name;
            UserProfileVM model;

            using (DContext db = new DContext())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                model = new UserProfileVM(dto);
            }
            return View("UserProfile", model);
        }

        //POST /account/user-profile
        [ActionName("user-profile")]
        [HttpPost]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserProfile",model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not match");
                    return View("UserProfile", model);
                }
            }

            using (DContext db = new DContext())
            {
                string userName = User.Identity.Name;
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.UserName == userName))
                {
                    ModelState.AddModelError("", "Username "+model.UserName+" already exist");
                    model.UserName = "";
                    return View("UserProfile", model);
                }

                UserDTO dto = db.Users.Find(model.Id);
                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.UserName = model.UserName;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }
                db.SaveChanges();
            }
            TempData["SM"] = "You have edited your profile! :)";

            return Redirect("~/account/user-profile");
        }

        //Get /account/Orders
        [Authorize(Roles ="User")]
        public ActionResult Orders()
        {
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (DContext db = new DContext())
            {
                UserDTO user = db.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;

                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId).ToArray()
                    .Select(x => new OrderVM(x)).ToList();

                foreach (var order in orders)
                {
                    Dictionary<string, int> productAndQty = new Dictionary<string, int>();

                    decimal total = 0m;

                    List<OrderDetailsDTO> orderDetailsDto = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    foreach (var orderDetails in orderDetailsDto)
                    {
                        ProductDTO product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                        decimal price = product.Price;
                        string productName = product.Name;
                        productAndQty.Add(productName, orderDetails.Quantity);

                        total += orderDetails.Quantity * price;
                    }
                    ordersForUser.Add(new OrdersForUserVM()
                    {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAntQty = productAndQty,
                        CreatedAt = order.CreatedAt

                    });
                }
            }
            return View(ordersForUser);
        }

    }
}
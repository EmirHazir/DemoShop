using DemoShop.Models.Data;
using DemoShop.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace DemoShop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty!";
                return View();
            }
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }
            ViewBag.GrandTotal = total;

            return View(cart);
        }

        //Soldaki kart
        public ActionResult CartPartial()
        {
            CartVM model = new CartVM();

            int qty = 0;
            decimal price = 0m;
            if (Session["cart"] != null)
            {
                var list = (List<CartVM>)Session["cart"];
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                        
                }
                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                model.Quantity = 0;
                model.Price = 0m;
            }


            return PartialView(model);
        }

        //Get sctipt yazdım ProductDetails de
        public ActionResult AddToCartPartial(int id)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            CartVM model = new CartVM();

            using (DContext db = new DContext())
            {
                ProductDTO product = db.Products.Find(id);
                var productCart = cart.FirstOrDefault(x => x.ProductId == id);
                if (productCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    productCart.Quantity++;
                }
            }
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            Session["cart"] = cart;
            return PartialView(model);
        }

        //Get ARTI
        public JsonResult IncrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (DContext db = new DContext())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                model.Quantity++;

                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //EKSİ
        public ActionResult DecrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (DContext db = new DContext())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }
                var result = new { qty = model.Quantity, price = model.Price };

                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        //GET Remove
        public void RemoveProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (DContext db = new DContext())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                cart.Remove(model);
            }
        }

        //GET PaypalPartial
        public ActionResult PaypalPartial()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            return PartialView(cart);
        }

        //POST PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            string userName = User.Identity.Name;
            int orderId = 0;
            using (DContext db = new DContext())
            {
                OrderDTO orderDto = new OrderDTO();

                var _query = db.Users.FirstOrDefault(x => x.UserName == userName);
                int userId = _query.Id;

                orderDto.UserId = userId;
                orderDto.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDto);

                db.SaveChanges();

                orderId = orderDto.OrderId;
                OrderDetailsDTO orderDetailsDto = new OrderDetailsDTO();

                foreach (var item in cart)
                {
                    orderDetailsDto.OrderId = orderId;
                    orderDetailsDto.UserId = userId;
                    orderDetailsDto.ProductId = item.ProductId;
                    orderDetailsDto.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDto);
                    db.SaveChanges();
                }
            }

            //Email to admin
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("2dff05817bb9f1", "67d24470d2dc83"),
                EnableSsl = true
            };
            client.Send("emircanaziz@gmail.com", "emircanaziz@gmail.com", "New order", "You have a new order! Order Number is " + orderId);

            Session["cart"] = null;

        }
    }
}
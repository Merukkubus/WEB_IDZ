using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebApplication1.Models.Entities;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string searchTerm)
        {
            var list = new List<product_category>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                using (var db = new supermarketEntities())
                {
                    list = db.product_category.OrderBy(x => x.category_name).ToList();
                }
                return View(list);
            }
            using (var db = new supermarketEntities())
            {
                list = db.product_category.Where(x => x.category_name.Contains(searchTerm)).ToList();
            }
            return View(list);
        }
        [HttpGet]
        public ActionResult ProductsByCategory(int categoryId, string searchTerm)
        {
            var list = new List<product>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                using (var db = new supermarketEntities())
                {
                    list = db.product.Where(x => x.category_id == categoryId).ToList();
                }
                return View(list);
            }
            using (var db = new supermarketEntities())
            {
                list = db.product.Where(x => x.product_name.Contains(searchTerm) && x.category_id == categoryId).ToList();
            }
            if (list.Count == 0)
            {
                using (var db = new supermarketEntities())
                {
                    list = db.product.Where(x => x.category_id == categoryId).ToList();
                }
                return View(list);
            }
            return View(list);
        }
        [ChildActionOnly]
        public ActionResult ProdPrice(long product_id)
        {
            string message = "";
            using (var db = new supermarketEntities())
            { 
                int pr = db.price.Where(x => x.product_id == product_id && x.date <= DateTime.Now)
                    .OrderByDescending(x => x.date).FirstOrDefault().price1;
                message = $"{pr}";
            }
            return PartialView("ProdPrice", message);
        }
        [HttpGet]
        public ActionResult Register() => View();
        [HttpPost]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                user newuser;
                using (var db = new supermarketEntities())
                {
                    var user = db.user.FirstOrDefault(x => x.login == model.Login);
                    if (user == null)
                    {
                        Guid salt = Guid.NewGuid();
                        string passwordHash = ReturnHashCode(model.Password + salt.ToString().ToUpper());
                        newuser = new user
                        {
                            id = Guid.NewGuid(),
                            login = model.Login,
                            passwordhash = passwordHash,
                            salt = salt,
                            role_id = 1,
                            card_id = 0,
                        };
                        db.user.Add(newuser);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Login", "Такой пользователь уже есть");
                    }
                }
            }
            ViewBag.Error = "Заполните поля";
            return View(model);
        }
        [HttpGet]
        public ActionResult Login() => View();
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserVM webUser)
        {
            if (ModelState.IsValid)
            {
                using (var db = new supermarketEntities())
                {
                    user user = null;
                    user = db.user.Where(u => u.login == webUser.Login).FirstOrDefault();
                    if (user != null)
                    {
                        string passwordHash = ReturnHashCode(webUser.Password + user.salt.ToString().ToUpper());
                        if (passwordHash == user.passwordhash)
                        {
                            string userRole = "";
                            switch (user.role_id)
                            {
                                case 0:
                                    userRole = "Admin";
                                    break;
                                case 1:
                                    userRole = "Participant";
                                    break;
                            }

                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                1,
                                user.login,
                                DateTime.Now,
                                DateTime.Now.AddDays(1),
                                false,
                                userRole
                            );
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            ViewBag.Error = "Пользователь не найден";
            return View(webUser);
        }
        string ReturnHashCode(string str)
        {
            string hash = "";
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }
                hash = sb.ToString().ToUpper();
            }
            return hash;
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
        public List<product_category> GetAllCategories()
        {
            var categoriesList = new List<product_category>();
            using (var db = new supermarketEntities())
            {
                categoriesList = db.product_category.OrderByDescending(x => x.category_id).ToList();
            }
            return categoriesList;
        }
        public List<company> GetAllCompanies()
        {
            var companyList = new List<company>();
            using (var db = new supermarketEntities())
            {
                companyList = db.company.OrderByDescending(x => x.company_id).ToList();
            }
            return companyList;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateProduct()
        {
            ViewBag.product_category = new SelectList(GetAllCategories(), "category_id", "category_name");
            ViewBag.company = new SelectList(GetAllCompanies(), "company_id", "company_name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateProduct(ProdVM newProd)
        {
            if (ModelState.IsValid)
            {
                using (var db = new supermarketEntities())
                {
                    product prod = new product()
                    {
                        product_id = GetProdId(newProd.category_id) + 1000,
                        product_name = newProd.product_name,
                        category_id = newProd.category_id,
                    };
                    db.product.Add(prod);
                    price pr = new price()
                    {
                        date = DateTime.Now,
                        product_id = GetProdId(newProd.category_id) + 1000,
                        price1 = newProd.price1,
                        company_id = newProd.company_id,
                    };
                    db.price.Add(pr);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.product_category = new SelectList(GetAllCategories(), "category_id", "category_name");
            ViewBag.company = new SelectList(GetAllCompanies(), "company_id", "company_name");
            return View(newProd);
        }
        public long GetProdId(int CategoryId)
        {
            var id = new List<long>();
            using (var db = new supermarketEntities())
            {
                foreach (product prod in db.product.Where(x => x.category_id == CategoryId))
                {
                    id.Add(prod.product_id);
                }
            }
            return (id.Max());
        }
        [HttpGet]
        public ActionResult CreateCategory() => View();
        [HttpPost]
        public ActionResult CreateCategory(CategoryVM cat)
        {
            if (ModelState.IsValid)
            {
                using (var db = new supermarketEntities())
                {
                    product_category category = new product_category
                    {
                        category_id = GetCategoryId() + 1,
                        category_name = cat.CategoryName,
                    };
                    db.product_category.Add(category);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
        public int GetCategoryId()
        {
            var id = new List<int>();
            using (var db = new supermarketEntities())
            {
                foreach (product_category cat in db.product_category.OrderBy(x => x.category_id))
                {
                    id.Add(cat.category_id);
                }
            }
            return (id.Max());
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditProduct(long product_id)
        {
            ProdVM model;
            using (var db = new supermarketEntities())
            {
                product prod = db.product.Find(product_id);
                int pr = db.price.Where(x => x.date <= DateTime.Now && x.product_id == product_id).OrderByDescending(x => x.date).FirstOrDefault().price1;
                Guid compId = db.price.Where(x => x.date <= DateTime.Now && x.product_id == product_id).OrderByDescending(x => x.date).FirstOrDefault().company_id;
                model = new ProdVM
                {
                    product_id = prod.product_id,
                    product_name = prod.product_name,
                    category_id = prod.category_id,
                    price1 = pr,
                    company_id = compId,
                };
            }
            ViewBag.product_category = new SelectList(GetAllCategories(), "category_id", "category_name");
            ViewBag.company = new SelectList(GetAllCompanies(), "company_id", "company_name");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult EditProduct(ProdVM model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new supermarketEntities())
                {
                    product editedProduct = new product
                    {
                        category_id = model.category_id,
                        product_id = model.product_id,
                        product_name = model.product_name,
                    };
                    
                    price newprice = new price
                    {
                        date = DateTime.Now,
                        product_id = model.product_id,
                        price1 = model.price1,
                        company_id = model.company_id,
                    };
                    db.product.Attach(editedProduct);
                    db.Entry(editedProduct).State = System.Data.Entity.EntityState.Modified;
                    db.price.Add(newprice);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.product_category = new SelectList(GetAllCategories(), "category_id", "category_name");
            ViewBag.company = new SelectList(GetAllCompanies(), "company_id", "company_name");
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteProduct(long product_id)
        {
            product productToDelete;
            using (var db = new supermarketEntities())
            {
                productToDelete = db.product.Find(product_id);
                ViewBag.product_category = GetAllCategories().First(x => x.category_id == productToDelete.category_id).category_name;
            }
            return View(productToDelete);
        }
        [HttpPost, ActionName("DeleteProduct")]
        public ActionResult DeleteConfirmed(long product_id)
        {
            using (var db = new supermarketEntities())
            {
                product productToDelete = db.product.Find(product_id);
                db.product.Remove(productToDelete);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Users()
        {
            var list = new List<user>();
            using (var db = new supermarketEntities())
            {
                list = db.user.OrderBy(x => x.login).ToList();
            }
            return View(list);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(Guid user_id)
        {
            EditUserVM model;
            using (var db = new supermarketEntities())
            {
                user dbuser = db.user.Find(user_id);
                model = new EditUserVM
                {
                    Id = user_id,
                    login = dbuser.login,
                    card_id = dbuser.card_id,
                    role_id = dbuser.role_id,
                    passwordhash = dbuser.passwordhash,
                    salt = dbuser.salt,
                };
            }
            ViewBag.sale = new SelectList(GetSaleCards(), "card_id", "sale");
            ViewBag.role = new SelectList(GetRoles(), "rolecode", "rolename");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(EditUserVM model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new supermarketEntities())
                {
                    user editeduser = new user
                    {
                        id = model.Id,
                        login = model.login,
                        card_id = model.card_id,
                        role_id = model.role_id,
                        salt = model.salt,
                        passwordhash=model.passwordhash,
                    };
                    db.user.Attach(editeduser);
                    db.Entry(editeduser).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Debug.WriteLine($"Model Error: {error.ErrorMessage}");
                }
            }
            ViewBag.sale = new SelectList(GetSaleCards(), "card_id", "sale");
            ViewBag.role = new SelectList(GetRoles(), "rolecode", "rolename");
            return View(model);
        }
        public List<discount_card> GetSaleCards()
        {
            var list = new List<discount_card>();
            using (var db = new supermarketEntities())
            {
                list = db.discount_card.OrderByDescending(x => x.card_id).ToList();
            }
            return list;
        }
        public List<role> GetRoles()
        {
            var list = new List<role>();
            using (var db = new supermarketEntities())
            {
                list = db.role.OrderByDescending(x => x.rolecode).ToList();
            }
            return list;
        }
        [HttpPost]
        public void AddToBasket(long prodId)
        {
            string currentUserLogin = HttpContext.User.Identity.Name;

            var currentUser = new user();
            var pr = new product();
            var pic = new product_in_check();
            var c = new cashier();
            using (var db = new supermarketEntities())
            {
                currentUser = db.user.Where(x => x.login == currentUserLogin).FirstOrDefault();
                pr = db.product.Find(prodId);

                var thisPurchase = currentUser.card_id;

                var chque = db.check.Where(x => x.datetime == null && thisPurchase == x.card_id).FirstOrDefault();
                if (chque != null)
                {
                    pic = db.product_in_check.FirstOrDefault(x => x.check_id == chque.check_id && x.product_id == pr.product_id);
                    if (pic != null)
                    {
                        pic.quantity += 1;
                        db.product_in_check.Attach(pic);
                        db.Entry(pic).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        pic = new product_in_check
                        {
                            quantity = 1,
                            product_id = pr.product_id,
                            check_id = chque.check_id
                        };

                        db.product_in_check.Add(pic);
                    }
                }
                else
                {
                    //создание покупки без даты
                    chque = new check();
                    chque.cashier_id = db.cashier.Where(x => x.name == "Сайт").FirstOrDefault().cashier_id;
                    chque.cash_reg_id = 2;
                    chque.datetime = null;
                    chque.check_id = Guid.NewGuid();
                    chque.card_id = currentUser.card_id;

                    //Добавление товара в покупку
                    pic.quantity = 1;
                    pic.product_id = pr.product_id;
                    pic.check_id = chque.check_id;

                    db.product_in_check.Add(pic);

                    db.check.Add(chque);
                }
                db.SaveChanges();
            }
        }
        public ActionResult Basket()
        {
            string currentUserLogin = HttpContext.User.Identity.Name;
            var prList = new List<product>();
            var currentUser = new user();
            var model = new Basket(prList, Guid.NewGuid());

            using (var db = new supermarketEntities())
            {
                currentUser = db.user.Where(x => x.login == currentUserLogin).FirstOrDefault();
                var thisPurchase = currentUser.card_id;

                var cheque = db.check.Where(x => x.datetime == null && thisPurchase == x.card_id).FirstOrDefault();
                if (cheque == null)
                {
                    return View(model);
                }
                var pic = db.product_in_check
                            .Where(x => x.check_id == cheque.check_id).ToList();

                foreach (var purItem in pic)
                {
                    var product = db.product.Find(purItem.product_id);
                    prList.Add(product);
                }

                model = new Basket(prList, cheque.check_id);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult ClearBasket(Guid checkId)
        {
            using (var db = new supermarketEntities())
            {
                var picList = db.product_in_check.Where(x => x.check_id == checkId).ToList();
                foreach (var pic in picList)
                {
                    db.product_in_check.Remove(pic);
                }
                var cheque = db.check.FirstOrDefault(x => x.check_id == checkId);
                if (cheque != null)
                {
                    db.check.Remove(cheque);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Basket");
        }
        [HttpPost]
        public ActionResult RemoveFromBasket(Guid checkId, long productId)
        {
            using (var db = new supermarketEntities())
            {
                var productInCheck = db.product_in_check
                                        .FirstOrDefault(x => x.check_id == checkId && x.product_id == productId);

                if (productInCheck != null)
                {
                    db.product_in_check.Remove(productInCheck);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Basket", new { checkId = checkId });
        }
        [HttpGet]
        public ActionResult BuyProducts(Guid guid)
        {
            string UserLogin = "";
            Guid UserID;
            if (User.Identity.IsAuthenticated == true)
            {
                UserLogin = User.Identity.Name;
            }
            double discount = 1;
            double sum = 0;
            using (var db = new supermarketEntities())
            {
                if (User.Identity.IsAuthenticated == true)
                {
                    var us = db.user.Where(x => x.login == UserLogin).FirstOrDefault();
                    UserID = us.id;
                    discount = (double)db.discount_card.Where(x => x.card_id == us.card_id).FirstOrDefault().sale;
                }

                var purItem = db.product_in_check.Where(x => x.check_id == guid).ToList();
                foreach (var pui in purItem)
                {
                    sum += db.price.Where(x => x.date <= DateTime.Now && x.product_id == pui.product_id)
                        .OrderByDescending(x => x.date).FirstOrDefault().price1 * discount;
                }

                var cheque = db.check.Find(guid);
                cheque.datetime = DateTime.Now;

                db.SaveChanges();
            }
            return View(sum);
        }
    }
}
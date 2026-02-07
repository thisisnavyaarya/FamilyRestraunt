using FamilyRestraunt.Data;
using FamilyRestraunt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FamilyRestraunt.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private Repository<Product> _products;
        private Repository<Orders> _orders;
        private readonly UserManager<ApplicationUsers> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUsers> userManager)
        {
            _context = context;
            _products = new Repository<Product>(context);
            _orders = new Repository<Orders>(context);
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet] 
        public async Task<IActionResult> Create()
        {
            // ViewBag.Products = await _products.GetAllAsync();
            //return View();
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel") ?? new OrderViewModel
            {
                OrderItems = new List<OrderItemViewModel>(),
                Products = await _products.GetAllAsync()
            };
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddItem(int prodId, int prodQty)
        {
            var product = await _context.Products.FindAsync(prodId);
            if (product == null)
            {
                return NotFound();
            }
           
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel") ?? new OrderViewModel
            {
                OrderItems = new List<OrderItemViewModel>(),
                Products = await _products.GetAllAsync()
            };
            //Check if product is already in orders 
            var existingItems = model.OrderItems.FirstOrDefault(oi => oi.ProductId == prodId);
            //If product already is in order, increase quantity
            if (existingItems != null)
            {
                existingItems.Quantity += prodQty;
            }
            else
            {
                model.OrderItems.Add(new OrderItemViewModel
                {
                    ProductId = product.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = prodQty
                });
            }
            //Calculate total price
            model.TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Quantity);
            //Save order to session
            HttpContext.Session.Set("OrderViewModel", model);

            return RedirectToAction("Create");
        }
        [HttpGet]
        [Authorize]
        public IActionResult Cart()
        {
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel");
            if (model == null || model.OrderItems.Count == 0)
            {
                return RedirectToAction("Create");
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PlaceOrder()
        {
            var model = HttpContext.Session.Get<OrderViewModel>("OrderViewModel");
            if (model == null || model.OrderItems.Count == 0)
            {
                return RedirectToAction("Create");
            }
            Orders order= new Orders()
            {
                OrderDate= DateTime.Now,
                TotalAmount= model.TotalPrice,
                UserId = _userManager.GetUserId(User),
                OrderItems = new List<OrderItem>() // Ensure OrderItems is initialized
            };
            foreach (var item in model.OrderItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }
            await _orders.AddAsync(order);
            HttpContext.Session.Remove("OrderViewModel");
            return RedirectToAction("ViewOrders");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ViewOrders()
        {
            var userId = _userManager.GetUserId(User);
            var userOrders = await _orders.GetAllByIdAsync(userId, "UserId", new QueryOptions<Orders>
            {
                Includes = "OrderItems.Product"
            });
            return View(userOrders);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

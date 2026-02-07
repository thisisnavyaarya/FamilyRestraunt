using FamilyRestraunt.Data;
using FamilyRestraunt.Data.Migrations;
using FamilyRestraunt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;

namespace FamilyRestraunt.Controllers
{
    public class ProductController : Controller
    {
        private Repository<Product> products;
        private Repository<Ingredient> ingredients;
        private Repository<Category> categories;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            products = new Repository<Product>(context);
            ingredients = new Repository<Ingredient>(context);
            categories = new Repository<Category>(context);
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            return View(await products.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag.Ingredients = await ingredients.GetAllAsync();
            ViewBag.Categories = await categories.GetAllAsync();

            if (id == 0)
            {
                ViewBag.Operation = "Add";
                return View(new Product());
            }
            else
            {
                Product? product = await products.GetByIdAsync(id, new QueryOptions<Product>
                {
                    Includes = "ProductIngredients.Ingredient,Category",
                    where = p => p.ProductId == id
                });

                if (product is null)
                {
                    return NotFound();
                }

                ViewBag.Operation = "Edit";
                return View(product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Product product, int[] IngredientIds, int catId)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = uniqueFileName;
                }
                if (product.ProductId == 0)
                {
                    ViewBag.Ingredients = await ingredients.GetAllAsync();
                    ViewBag.Categories = await categories.GetAllAsync();
                    product.CategoryId = catId;

                    //add Ingredients to product
                    product.ProductIngredients ??= new List<ProductIngredients>();
                    foreach (int Id in IngredientIds)
                    {
                        product.ProductIngredients.Add(new ProductIngredients { IngredientId = Id, ProductId = product.ProductId });
                    }
                    await products.AddAsync(product);
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    var existingProduct = await products.GetByIdAsync(product.ProductId, new QueryOptions<Product>
                    {
                        Includes = "ProductIngredients"
                    });
                    if (existingProduct is null)
                    {
                        ModelState.AddModelError("", "Product not found");
                        ViewBag.Ingredients = await ingredients.GetAllAsync();
                        ViewBag.Categories = await categories.GetAllAsync();
                        return View(product);
                    }
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Stock = product.Stock;
                    existingProduct.CategoryId = catId;
                    //update Ingredients
                    existingProduct.ProductIngredients ??= new List<ProductIngredients>();
                    existingProduct.ProductIngredients.Clear();
                    foreach (int Id in IngredientIds)
                    {
                        existingProduct.ProductIngredients.Add(new ProductIngredients { IngredientId = Id, ProductId = existingProduct.ProductId });
                    }
                    try
                    {
                        await products.UpdateAsync(existingProduct);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error updating product: " + ex.Message);
                        ViewBag.Ingredients = await ingredients.GetAllAsync();
                        ViewBag.Categories = await categories.GetAllAsync();
                        return View(product);
                    }
                }
            }
            return RedirectToAction("Index", "Product");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await products.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error deleting product: " + ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}

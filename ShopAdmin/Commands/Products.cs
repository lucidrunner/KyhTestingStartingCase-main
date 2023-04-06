using Microsoft.Extensions.Logging;
using ShopGeneral.Data;
using ShopGeneral.Services;

namespace ShopAdmin.Commands;

public class Products : ConsoleAppBase
{
    private readonly ILogger<Products> _logger;
    private readonly IProductService _productService;
    private readonly IFileService _fileService;

    public Products(ILogger<Products> logger, IProductService productService, IFileService fileService)
    {
        _logger = logger;
        _productService = productService;
        _fileService = fileService;
    }

    public void ExportJson(string inputTo)
    {
        _logger.LogInformation("ExportJson starting");
        var allProducts = _productService.GetAllProducts().ToList();
        var exportedProducts = new ExportedProducts();
        exportedProducts.total = allProducts.Count;

        foreach (Product product in allProducts)
        {
            var exportedProduct = new ExportedProduct()
            {
                id = product.Id,
                title = product.Name,
                brand = product.Manufacturer.Name,
                category = product.Category.Name,
                price = product.BasePrice,
                images = new List<string>() { product.ImageUrl }
            };

            exportedProducts.products.Add(exportedProduct);
        }

        DateTime today = DateTime.Now;
        string fileName = $"{today:yyyyMMdd}.txt";
        _fileService.SaveJson(inputTo, fileName, exportedProducts);
        _logger.LogInformation("ExportJson ending");
    }
}

public class ExportedProducts
{
    public List<ExportedProduct> products = new();

    public int total { get; set; }
    public int skip { get; set; } = 0;
    public int limit { get; set; } = 0;
}

public class ExportedProduct
{
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; } = string.Empty;
    public int price { get; set; }
    public double discountPercentage { get; set; } = 0;
    public double rating { get; set; } = 0;
    public int stock { get; set; } = 0;

    public string brand { get; set; }
    public string category { get; set; }

    public List<string> images { get; set; } = new();
}
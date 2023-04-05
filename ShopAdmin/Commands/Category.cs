using Microsoft.Extensions.Logging;
using ShopGeneral.Services;

namespace ShopAdmin.Commands;

public class Categorys : ConsoleAppBase
{
    private readonly ILogger<Products> _logger;
    private readonly IProductService _productService;
    private readonly IFileService _fileService;
    private readonly ICategoryService _categoryService;

    public Categorys(ILogger<Products> logger, IProductService productService, IFileService fileService, ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService;
        _fileService = fileService;
        _categoryService = categoryService;
    }

    public void CheckEmpty(string inputTo)
    {
        _logger.LogInformation("exportjson checkempty");

        var missingProducts = _categoryService.GetAllCategories().
                            Where(category => !_productService.GetAllProducts().
                            Select(product => product.Category.Name).
                            Contains(category.Name)).
                            Select(category => category.Name);

        if (missingProducts.Count() > 0)
        {
            DateTime today = DateTime.Now;
            string filename = $"missingproducts-{today:yyyy/MM/dd}.txt";
            _fileService.SaveJson(inputTo, filename, missingProducts);
        }
        _logger.LogInformation("exportjson ending");
    }
}
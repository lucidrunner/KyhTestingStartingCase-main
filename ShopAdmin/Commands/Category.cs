using Microsoft.Extensions.Logging;
using ShopGeneral.Data;
using ShopGeneral.Services;

namespace ShopAdmin.Commands;

public class Category : ConsoleAppBase
{
    private readonly ILogger<Products> _logger;
    private readonly IProductService _productService;
    private readonly IFileService _fileService;
    private readonly ICategoryService _categoryService;

    public Category(ILogger<Products> logger, IProductService productService, IFileService fileService, ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService;
        _fileService = fileService;
        _categoryService = categoryService;
    }

    public void CheckEmpty(string inputTo)
    {
        var temp = _categoryService.GetAllCategories();
        Console.WriteLine(temp);

        //DateTime today = DateTime.Now;
        //string fileName = $"{today:yyyy/MM/dd}.txt";
        //_fileService.SaveJson(inputTo, fileName, exportedProducts);
        //_logger.LogInformation("ExportJson ending");
    }


}
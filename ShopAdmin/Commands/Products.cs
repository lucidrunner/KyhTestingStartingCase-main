﻿using System.Net;
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

    public void VerifyImage(string folder)
    {
        _logger.LogInformation("VerifyImage starting");
        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

        var images = new List<int>();
        var allImages = _productService.GetAllProducts();

        foreach (var image in allImages)

        {
            if (!string.IsNullOrEmpty(image.ImageUrl))
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(image.ImageUrl);
                request.Method = "HEAD";
                try
                {
                    request.GetResponse();
                }
                catch
                {
                    images.Add(image.Id);
                }
            }
            else
            {
                images.Add(image.Id);
            }
        }

        if (images.Count > 0)
        {
            DateTime today = DateTime.Now;
            string fileName = $"missingimages-{today:yyyyMMdd}.txt";
            _fileService.SaveJson(folder, fileName, images);
        }
        


        _logger.LogInformation("VerifyImage ending");
    }


    public void ExportJson(string folder)
    {
        _logger.LogInformation("ExportJson starting");
        if (string.IsNullOrWhiteSpace(folder))
        {
            return;
        }

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
        _fileService.SaveJson(folder, fileName, exportedProducts);
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

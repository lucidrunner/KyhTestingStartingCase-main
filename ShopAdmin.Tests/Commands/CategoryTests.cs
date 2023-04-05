using Microsoft.Extensions.Logging;
using Moq;
using ShopAdmin.Commands;
using ShopGeneral.Data;
using ShopGeneral.Services;

namespace ShopAdmin.Tests.Commands
{
    [TestClass]
    public class CategorysTests
    {
        private Categorys sut;

        private readonly Mock<ILogger<Products>> _logger;
        private readonly Mock<IProductService> _productService;
        private readonly Mock<IFileService> _fileService;
        private readonly Mock<ICategoryService> _categoryService;

        public CategorysTests()
        {
            _logger = new Mock<ILogger<Products>>();
            _productService = new Mock<IProductService>();
            _fileService = new Mock<IFileService>();
            _categoryService = new Mock<ICategoryService>();

            sut = new Categorys(_logger.Object, _productService.Object, _fileService.Object, _categoryService.Object);
        }

        [TestMethod]
        public void When_exporting_empty_category_filename_then_fileservice_is_called()
        {
            //Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Category = categories[0] }
            };

            _categoryService.Setup(x => x.GetAllCategories()).Returns(categories);
            _productService.Setup(x => x.GetAllProducts()).Returns(products);

            //Act
            sut.CheckEmpty("");

            //Assert
            _fileService.Verify(fileService =>
                fileService.SaveJson(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Once);
        }

        [TestMethod]
        public void When_exporting_foldername_then_fileservice_is_called()
        {
            //Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Category = categories[0] }
            };

            _categoryService.Setup(x => x.GetAllCategories()).Returns(categories);
            _productService.Setup(x => x.GetAllProducts()).Returns(products);

            string folder = "categories";

            //Act
            sut.CheckEmpty(folder);

            //Assert
            _fileService.Verify(fileService =>
                fileService.SaveJson(folder, It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [TestMethod]
        public void When_exporting_foldername_and_filename_then_fileservice_is_called()
        {
            //Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Category = categories[0] }
            };

            _categoryService.Setup(x => x.GetAllCategories()).Returns(categories);
            _productService.Setup(x => x.GetAllProducts()).Returns(products);

            string folder = "categories";
            string today = "missingproducts-" + DateTime.Now.ToString("yyyy/MM/dd") + ".txt";

            //Act
            sut.CheckEmpty(folder);

            //Assert
            _fileService.Verify(fileService =>
                fileService.SaveJson(folder, today, It.IsAny<object>()), Times.Once);
        }

        [TestMethod]
        public void When_categories_list_is_empty_dont_call_fileservice()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Category = categories[0] },
                new Product { Id = 2, Name = "Product 2", Category = categories[1] }
            };

            _categoryService.Setup(x => x.GetAllCategories()).Returns(categories);
            _productService.Setup(x => x.GetAllProducts()).Returns(products);

            string inputTo = "categories";

            // Act
            sut.CheckEmpty(inputTo);

            // Assert
            _fileService.Verify(x => x.SaveJson(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Never);
        }

        [TestMethod]
        public void When_categories_list_is_not_empty_call_fileservice()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1" },
                new Category { Id = 2, Name = "Category 2" }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Category = categories[0] },
                new Product { Id = 2, Name = "Product 2", Category = categories[0] }
            };

            _categoryService.Setup(x => x.GetAllCategories()).Returns(categories);
            _productService.Setup(x => x.GetAllProducts()).Returns(products);

            string inputTo = "categories";
            var expectedMissingProducts = new List<string> { "Category 2" };

            // Act
            sut.CheckEmpty(inputTo);

            // Assert
            _fileService.Verify(x => x.SaveJson(It.IsAny<string>(), It.IsAny<string>(), expectedMissingProducts), Times.Once);
        }
    }
}
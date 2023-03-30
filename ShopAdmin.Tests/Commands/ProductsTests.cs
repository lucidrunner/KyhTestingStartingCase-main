using Microsoft.Extensions.Logging;
using Moq;
using ShopAdmin.Commands;
using ShopGeneral.Data;
using ShopGeneral.Infrastructure.Context;
using ShopGeneral.Services;

namespace ShopAdmin.Tests.Commands
{
    [TestClass]
    public class ProductsTests
    {
        private Products sut;

        private readonly Mock<ILogger<Products>> _logger;
        private readonly Mock<IProductService> _productService;
        private readonly Mock<IFileService> _fileService;

        public ProductsTests()
        {
            _logger = new Mock<ILogger<Products>>();
            _productService = new Mock<IProductService>();
            _fileService = new Mock<IFileService>();

            sut = new Products(_logger.Object, _productService.Object, _fileService.Object);
        }

        [TestMethod]
        public void When_exporting_products_then_file_service_is_called()
        {
            //Arrange
            //Act
            sut.ExportJson("");


            //Assert
            _fileService.Verify(fileService => 
                fileService.SaveJson(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [TestMethod]
        public void When_exporting_products_then_correct_folder_is_called()
        {
            //Arrange
            string folder = "pricerunner";

            //Act
            sut.ExportJson(folder);


            //Assert
            _fileService.Verify(fileService =>
                fileService.SaveJson(folder, It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [TestMethod]
        public void When_exporting_products_then_file_is_saved_to_todays_date()
        {
            //Arrange
            string folder = "pricerunner";
            string today = DateTime.Now.ToString("yyyy/MM/dd") + ".txt";
            
            //Act
            sut.ExportJson(folder);


            //Assert
            _fileService.Verify(fileService =>
                fileService.SaveJson(folder, today, It.IsAny<object>()), Times.Once);
        }
    }
}
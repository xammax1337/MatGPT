using Xunit;
using MatGPT.Controllers;
using MatGPT.Models;
using MatGPT.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatGPT.Interfaces;
using MatGPT.Models.ViewModels;

namespace MatGPTTestProject.RepositoryTest
{
    [TestClass]
    public class IngredientTest
    {
        [TestMethod]
        public async Task AddIngredientAsync_ReturnsOkResult()
        {
            // Arrange
            var mockRepository = new Mock<IIngredientRepository>();
            mockRepository.Setup(repo => repo.AddIngredientAsync(It.IsAny<IngredientDto>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(new Ingredient());

            var controller = new IngredientController(null, mockRepository.Object);

            // Act
            var result = await controller.AddIngredientAsync(new IngredientDto(), "TestIngredient", 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task DeleteIngredientAsync_ReturnsOkResult()
        {
            // Arrange
            var mockRepository = new Mock<IIngredientRepository>();
            mockRepository.Setup(repo => repo.DeleteIngredientAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Ingredient());

            var controller = new IngredientController(null, mockRepository.Object);

            // Act
            var result = await controller.DeleteIngredientAsync(1, "TestIngredient");

            // Assert
            Assert.IsNotNull(result);

            
            if (result is OkObjectResult okResult && okResult.Value is string message)
            {
                Assert.AreEqual("TestIngredient deleted", message);
            }
            else
            {
                Assert.Fail("Unexpected result type or value");
            }
        
        }

        [TestMethod]
        public async Task ListIngredientsAsync_ReturnsOkResultWithIngredients()
        {
            // Arrange
            var mockRepository = new Mock<IIngredientRepository>();
            mockRepository.Setup(repo => repo.ListIngredientsFromUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<IngredientViewModel> { new IngredientViewModel { IngredientName = "TestIngredient" } });

            var controller = new IngredientController(null, mockRepository.Object);

            // Act
            var result = await controller.ListUsersIngredientsAsync(1);

            // Assert
            Assert.IsNotNull(result);

            
            if (result is OkObjectResult okResult && okResult.Value is List<IngredientViewModel> ingredients)
            {
                Assert.AreEqual(1, ingredients.Count);
                Assert.AreEqual("TestIngredient", ingredients[0].IngredientName);
            }
            else
            {
                Assert.Fail("Unexpected result type or value");
            }
        }
    }
}


using NSubstitute;
using Practice.Maui.Application.Models;
using Practice.Maui.Application.Services;

namespace Practice.Maui.Tests;

[TestFixture]
[TestOf(typeof(FuelBookSheetModel))]
public class FuelBookSheetModelTest
{

    [SetUp]
    public void Setup()
    {
        _sheetServiceWrapperSubstitute.ClearReceivedCalls();
        _sheetServiceWrapperSubstitute.GetMainSheet().Returns(TestDataProvider.CreateMainSheetWithMultipleRows());
    }
    private readonly ISheetServiceWrapper _sheetServiceWrapperSubstitute = Substitute.For<ISheetServiceWrapper>();

    [Test]
    public async Task LoadColumns_WhenCalled_LoadsData()
    {
        // Arrange
        var fuelBookSheetModel = new FuelBookSheetModel(_sheetServiceWrapperSubstitute);

        // Act
        var result = (await fuelBookSheetModel.LoadColumns()).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
    }
}
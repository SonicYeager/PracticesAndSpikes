using NSubstitute;
using Practice.Maui.Application.Models;
using Practice.Maui.Application.Services;

namespace Practice.Maui.Tests;

public sealed class FuelBookRowModelTests
{
    private readonly ISheetServiceWrapper _sheetServiceWrapperSubstitute = Substitute.For<ISheetServiceWrapper>();

    [SetUp]
    public void Setup()
    {
        _sheetServiceWrapperSubstitute.ClearReceivedCalls();
        _sheetServiceWrapperSubstitute.GetMainSheet().Returns(TestDataProvider.CreateMainSheet(1));
    }

    [Test]
    public async Task Load_WhenCalled_LoadsData()
    {
        // Arrange
        var fuelBookRowModel = new FuelBookRowModel(_sheetServiceWrapperSubstitute);

        // Act
        await fuelBookRowModel.Load(1);

        // Assert
        Assert.That(fuelBookRowModel.RowData, Is.Not.Null);
    }

    [Test]
    public async Task RowData_WhenPropertiesAccessed_ParsesRowData()
    {
        // Arrange
        var fuelBookRowModel = new FuelBookRowModel(_sheetServiceWrapperSubstitute)
        {
            RowData = TestDataProvider.CreateRowData(1, DateTime.FromOADate(1).Date, 2, 3, 4, 5),
        };

        // Act
        var number = fuelBookRowModel.Number;
        var date = fuelBookRowModel.Date;
        var costsInEuro = fuelBookRowModel.CostsInEuro;
        var consumptionInLiters = fuelBookRowModel.ConsumptionInLiters;
        var rangeInKilometers = fuelBookRowModel.RangeInKilometers;
        var averageConsumptionPerHundredKilometers = fuelBookRowModel.AverageConsumptionPerHundredKilometers;

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(number, Is.EqualTo(1));
            Assert.That(date, Is.EqualTo(DateOnly.FromDateTime(DateTime.FromOADate(1).Date)));
            Assert.That(costsInEuro, Is.EqualTo(2));
            Assert.That(consumptionInLiters, Is.EqualTo(3));
            Assert.That(rangeInKilometers, Is.EqualTo(4));
            Assert.That(averageConsumptionPerHundredKilometers, Is.EqualTo(5));
        });
    }
}
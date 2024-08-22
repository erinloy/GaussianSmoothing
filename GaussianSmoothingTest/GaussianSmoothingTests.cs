using System;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;
using Imaging;
using Utils;

namespace Tests;

public class GaussianSmoothingTests
{
    private readonly ITestOutputHelper _output;

    public GaussianSmoothingTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ApplyGaussianSmoothing_ShouldReturnExpectedSmoothedArray()
    {
        // Arrange
        double[,] image = new double[,]
        {
            { 10, 20, 30, 40, 50 },
            { 60, 70, 80, 90, 100 },
            { 110, 120, 130, 140, 150 },
            { 160, 170, 180, 190, 200 },
            { 210, 220, 230, 240, 250 }
        };
        _output.WriteLine($"IMAGE:\n{image.PrettyPrint()}");

        double sigma = 1.0;

        // Act
        double[,] result = GaussianSmoothingExtensions.ApplyGaussianSmoothing(image, sigma);

        _output.WriteLine($"RESULT:\n{result.PrettyPrint()}");

        // Expected result based on Python output
        double[,] expected = new double[,]
        {
            { 35.62,  42.03,  51.35,  60.67,  67.08 },
            { 67.66,  74.07,  83.39,  92.71,  99.12 },
            {114.27, 120.68, 130.00, 139.32, 145.73 },
            {160.88, 167.29, 176.61, 185.93, 192.34 },
            {192.92, 199.33, 208.65, 217.97, 224.38 }
        };

        _output.WriteLine($"EXPECT:\n{expected.PrettyPrint()}");

        // Assert
        result.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.1))
            .WhenTypeIs<double>());
    }

    [Fact]
    public void ApplyGaussianSmoothing_ByteArray_ShouldReturnExpectedSmoothedArray()
    {
        // Arrange
        byte[,] image = new byte[,]
        {
            { 10, 20, 30, 40, 50 },
            { 60, 70, 80, 90, 100 },
            { 110, 120, 130, 140, 150 },
            { 160, 170, 180, 190, 200 },
            { 210, 220, 230, 240, 250 }
        };
        _output.WriteLine($"IMAGE:\n{image.PrettyPrint()}");

        double sigma = 1.0;

        // Act
        byte[,] result = GaussianSmoothingExtensions.ApplyGaussianSmoothing(image, sigma);

        _output.WriteLine($"RESULT:\n{result.PrettyPrint()}");

        // Expected result converted from previous double implementation
        byte[,] expected = new byte[,]
        {
            { 36,  42,  51,  61,  67 },
            { 68,  74,  83,  93,  99 },
            {114, 121, 130, 139, 146 },
            {161, 167, 177, 186, 192 },
            {193, 199, 209, 218, 224 }
        };

        _output.WriteLine($"EXPECT:\n{expected.PrettyPrint()}");

        // Assert
        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Fact]
    public void ApplyGaussianSmoothing_SimplifiedTest()
    {
        // Arrange
        byte[,] image = new byte[,]
        {
            { 10, 20, 30 },
            { 40, 50, 60 },
            { 70, 80, 90 }
        };
        _output.WriteLine($"IMAGE:\n{image.PrettyPrint()}");

        double sigma = 1.0;

        // Act
        byte[,] result = GaussianSmoothingExtensions.ApplyGaussianSmoothing(image, sigma);

        _output.WriteLine($"RESULT:\n{result.PrettyPrint()}");

        // Expected result converted from previous double implementation
        byte[,] expected = new byte[,]
        {
            { 27, 33, 38 },
            { 44, 50, 56 },
            { 62, 67, 73 }
        };

        _output.WriteLine($"EXPECT:\n{expected.PrettyPrint()}");

        // Assert
        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [Fact]
    public void TestPrettyPrint()
    {
        double[] a = new double[]
        { 35.62,  42.03,  51.35,  60.67,  67.08 };
        _output.WriteLine(a.PrettyPrint());
        _output.WriteLine("");

        double[,] b = new double[,]
        {
            { 35.62,  42.03,  51.35,  60.67,  67.08 },
            { 67.66,  74.07,  83.39,  92.71,  99.12 },
            {114.27, 120.68, 130.00, 139.32, 145.73 },
            {160.88, 167.29, 176.61, 185.93, 192.34 },
            {192.92, 199.33, 208.65, 217.97, 224.38 }
        };
        _output.WriteLine(b.PrettyPrint());
        _output.WriteLine("");

        double[,,] c = new double[,,]
        { {
            { 35.62,  42.03,  51.35,  60.67,  67.08 },
            { 67.66,  74.07,  83.39,  92.71,  99.12 },
            {114.27, 120.68, 130.00, 139.32, 145.73 },
            {160.88, 167.29, 176.61, 185.93, 192.34 },
            {192.92, 199.33, 208.65, 217.97, 224.38 }
        },{
            { 35.62,  42.03,  51.35,  60.67,  67.08 },
            { 67.66,  74.07,  83.39,  92.71,  99.12 },
            {114.27, 120.68, 130.00, 139.32, 145.73 },
            {160.88, 167.29, 176.61, 185.93, 192.34 },
            {192.92, 199.33, 208.65, 217.97, 224.38 }
        } };
        _output.WriteLine(c.PrettyPrint());
        _output.WriteLine("");
    }
}

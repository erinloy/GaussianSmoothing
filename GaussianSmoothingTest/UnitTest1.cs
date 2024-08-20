using System;
using Xunit;
using FluentAssertions;

public static class ImageProcessor
{
    public static byte[,] ApplyGaussianSmoothing(byte[,] image, double sigma)
    {
        if (sigma == 0) return (byte[,])image.Clone(); // Return the original image for sigma = 0

        int width = image.GetLength(0);
        int height = image.GetLength(1);
        byte[,] smoothed = new byte[width, height];

        int kernelSize = (int)(6 * sigma + 1);
        if (kernelSize % 2 == 0) kernelSize += 1;
        int kernelRadius = kernelSize / 2;

        // Generate Gaussian kernel
        double[,] kernel = new double[kernelSize, kernelSize];
        double kernelSum = 0;
        for (int i = -kernelRadius; i <= kernelRadius; i++)
        {
            for (int j = -kernelRadius; j <= kernelRadius; j++)
            {
                double value = Math.Exp(-(i * i + j * j) / (2 * sigma * sigma));
                kernel[i + kernelRadius, j + kernelRadius] = value;
                kernelSum += value;
            }
        }

        // Normalize the kernel
        for (int i = 0; i < kernelSize; i++)
        {
            for (int j = 0; j < kernelSize; j++)
            {
                kernel[i, j] /= kernelSum;
            }
        }

        // Apply Gaussian smoothing
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                double sum = 0;
                for (int i = -kernelRadius; i <= kernelRadius; i++)
                {
                    for (int j = -kernelRadius; j <= kernelRadius; j++)
                    {
                        int pixelX = Math.Min(Math.Max(x + i, 0), width - 1);
                        int pixelY = Math.Min(Math.Max(y + j, 0), height - 1);
                        sum += image[pixelX, pixelY] * kernel[i + kernelRadius, j + kernelRadius];
                    }
                }
                smoothed[x, y] = (byte)Math.Round(sum); // Proper rounding
            }
        }

        return smoothed;
    }
}




public class ImageProcessorTests
{
    [Fact]
    public void ApplyGaussianSmoothing_ShouldReturnSmoothedArray()
    {
        // Arrange
        byte[,] image = new byte[,]
        {
        { 10, 20, 30 },
        { 40, 50, 60 },
        { 70, 80, 90 }
        };
        double sigma = 1.0;

        // Act
        byte[,] result = ImageProcessor.ApplyGaussianSmoothing(image, sigma);

        // Assert
        result.Should().NotBeNull();
        result.GetLength(0).Should().Be(image.GetLength(0));
        result.GetLength(1).Should().Be(image.GetLength(1));

        // Check that center pixel is increased, indicating smoothing effect
        result[1, 1].Should().BeGreaterThan(image[1, 1]);

        // Ensure other pixels have changed
        result[0, 0].Should().NotBe(image[0, 0]);
        result[2, 2].Should().NotBe(image[2, 2]);
    }




    [Fact]
    public void ApplyGaussianSmoothing_WithZeroSigma_ShouldReturnOriginalArray()
    {
        // Arrange
        byte[,] image = new byte[,]
        {
            { 10, 20, 30 },
            { 40, 50, 60 },
            { 70, 80, 90 }
        };
        double sigma = 0.0;

        // Act
        byte[,] result = ImageProcessor.ApplyGaussianSmoothing(image, sigma);

        // Assert
        result.Should().BeEquivalentTo(image); // Ensure the original image is returned
    }

    [Fact]
    public void ApplyGaussianSmoothing_WithUniformImage_ShouldReturnAlmostUniformArray()
    {
        // Arrange
        byte[,] image = new byte[,]
        {
            { 50, 50, 50 },
            { 50, 50, 50 },
            { 50, 50, 50 }
        };
        double sigma = 1.0;

        // Act
        byte[,] result = ImageProcessor.ApplyGaussianSmoothing(image, sigma);

        // Assert
        result.Should().NotBeNull();
        result.GetLength(0).Should().Be(image.GetLength(0));
        result.GetLength(1).Should().Be(image.GetLength(1));

        // Allow slight deviations due to floating-point arithmetic
        foreach (var pixel in result)
        {
            pixel.Should().BeInRange(49, 51); // Uniform image should stay nearly uniform
        }
    }

    [Fact]
    public void ApplyGaussianSmoothing_ExactValueCheck()
    {
        // Arrange
        byte[,] image = new byte[,]
        {
        { 0, 0, 0 },
        { 0, 100, 0 },
        { 0, 0, 0 }
        };
        double sigma = 1.0;

        // Act
        byte[,] result = ImageProcessor.ApplyGaussianSmoothing(image, sigma);

        // Assert
        byte[,] expected = new byte[,]
        {
        { 6, 10, 6 },
        { 10, 16, 10 },
        { 6, 10, 6 }
        };

        result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }



}



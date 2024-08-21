using System;
using Xunit;
using FluentAssertions;
using Xunit.Abstractions;

public static class ImageProcessor
{
    public static double[,] ApplyGaussianSmoothing(double[,] image, double sigma, double truncate = 4.0)
    {
        int width = image.GetLength(0);
        int height = image.GetLength(1);
        double[,] smoothed = new double[width, height];

        // Calculate the radius of the kernel
        int radius = (int)(truncate * sigma + 0.5);
        int kernelSize = 2 * radius + 1;

        // Generate Gaussian kernel
        double[] kernel = new double[kernelSize];
        double kernelSum = 0;
        for (int i = -radius; i <= radius; i++)
        {
            double value = Math.Exp(-(i * i) / (2 * sigma * sigma));
            kernel[i + radius] = value;
            kernelSum += value;
        }

        // Normalize the kernel
        for (int i = 0; i < kernelSize; i++)
        {
            kernel[i] /= kernelSum;
        }

        // Apply Gaussian smoothing (1D convolution along rows)
        double[,] temp = new double[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double sum = 0;
                for (int i = -radius; i <= radius; i++)
                {
                    int pixelX = Reflect(x + i, width);  // Reflect mode
                    sum += image[pixelX, y] * kernel[i + radius];
                }
                temp[x, y] = sum;
            }
        }

        // Apply Gaussian smoothing (1D convolution along columns)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                double sum = 0;
                for (int i = -radius; i <= radius; i++)
                {
                    int pixelY = Reflect(y + i, height);  // Reflect mode
                    sum += temp[x, pixelY] * kernel[i + radius];
                }
                smoothed[x, y] = sum;
            }
        }

        return smoothed;
    }

    public static byte[,] ApplyGaussianSmoothing(byte[,] image, double sigma, double truncate = 4.0)
    {
        // Convert byte array to double array
        int width = image.GetLength(0);
        int height = image.GetLength(1);
        double[,] doubleImage = new double[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                doubleImage[x, y] = image[x, y];
            }
        }

        // Perform Gaussian smoothing using the existing method
        double[,] smoothedDouble = ApplyGaussianSmoothing(doubleImage, sigma, truncate);

        // Convert the double array back to a byte array
        byte[,] smoothedByte = new byte[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                smoothedByte[x, y] = (byte)Math.Clamp(Math.Round(smoothedDouble[x, y]), 0, 255);
            }
        }

        return smoothedByte;
    }

    // Reflect helper function to handle boundary conditions
    private static int Reflect(int x, int length)
    {
        if (x < 0) return -x - 1;
        if (x >= length) return 2 * length - x - 1;
        return x;
    }
}


public class ImageProcessorTests
{
    private readonly ITestOutputHelper _output;

    public ImageProcessorTests(ITestOutputHelper output)
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
        double sigma = 1.0;

        // Act
        double[,] result = ImageProcessor.ApplyGaussianSmoothing(image, sigma);

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
        double sigma = 1.0;

        // Act
        byte[,] result = ImageProcessor.ApplyGaussianSmoothing(image, sigma);

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

public static class Utils
{
    public static string PrettyPrint(this Array array)
    {
        int maxLength = GetMaxLength(array);
        var sb = new System.Text.StringBuilder();
        PrettyPrintRecursive(array, sb, 0, new int[array.Rank], maxLength);
        return sb.ToString();
    }

    private static int GetMaxLength(Array array)
    {
        int maxLength = 0;
        foreach (var item in array)
        {
            int length = string.Format("{0:F2}", item).Length;
            if (length > maxLength)
            {
                maxLength = length;
            }
        }
        return maxLength;
    }

    private static void PrettyPrintRecursive(Array array, System.Text.StringBuilder sb, int dimension, int[] indices, int maxLength)
    {
        int length = array.GetLength(dimension);

        sb.Append("{");
        for (int i = 0; i < length; i++)
        {
            indices[dimension] = i;
            if (dimension < array.Rank - 1)
            {
                PrettyPrintRecursive(array, sb, dimension + 1, indices, maxLength);
                if (i < length - 1)
                {
                    sb.AppendLine(",");
                    for (int j = 0; j < dimension + 1; j++)
                        sb.Append(" ");
                }
            }
            else
            {
                sb.AppendFormat("{0," + maxLength + ":F2}", array.GetValue(indices));
                if (i < length - 1)
                    sb.Append(", ");
            }
        }
        sb.Append("}");
    }
}

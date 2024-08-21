namespace Imaging;

public static class GaussianSmoothingExtensions
{
    public static double[,] ApplyGaussianSmoothing(this double[,] image, double sigma, double truncate = 4.0)
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

    public static byte[,] ApplyGaussianSmoothing(this byte[,] image, double sigma, double truncate = 4.0)
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

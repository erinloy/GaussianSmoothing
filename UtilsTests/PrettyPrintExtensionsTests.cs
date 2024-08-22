using Microsoft.VisualStudio.TestPlatform.Utilities;
using PrettyPrint;
using Xunit.Abstractions;

namespace UtilsTests;

public class PrettyPrintExtensionsTests(ITestOutputHelper _output)
{
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
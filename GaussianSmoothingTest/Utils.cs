namespace Utils;

public static class PrettyPrintExtensions
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

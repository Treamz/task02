public static class Base64Convertor
{
    public static string ReturnInBase64(string value)
    {
        var clientBytes = System.Text.Encoding.UTF8.GetBytes(value);
        return System.Convert.ToBase64String(clientBytes);
    }

    public static string ReturnFromBase64(string value)
    {
        byte[] buffer = System.Convert.FromBase64String(value);
        return System.Text.Encoding.ASCII.GetString(buffer);
    }
}



public static class Validator
{
    public static void ValidateAddresses(List<string> addresses)
    {
        if (addresses == null)
        {
            throw new ArgumentNullException(nameof(addresses), "Addresses list cannot be null.");
        }
        if (addresses.Count < 2)
        {
            throw new ArgumentException("At least two addresses are required.", nameof(addresses));
        }
    }

    public static void ValidateRequest(AddressRequestFormat addressRequest)
    {
        if (addressRequest == null)
        {
            throw new ArgumentNullException(nameof(addressRequest), "Request cannot be null.");
        }
    }

    public static void ValidateJsonNullOrEmpty(string json)
    {
       if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentException("The JSON string is null or empty.", nameof(json));
        }
    }
}
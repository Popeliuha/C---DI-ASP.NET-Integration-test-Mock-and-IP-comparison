// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
string ip1 = "192.168.1.6ejdid";
string ip2 = "192.168.1.6a";

string result = Task.CompareIPVersions(ip1, ip2);

Console.WriteLine($"The greater IP version is: {result}");
Console.ReadLine();

public static class Task
{
    public static string CompareIPVersions(string ip1, string ip2)
    {
        // Split the strings into arrays of integers
        string[] ipArray1 = ip1.Split('.');
        string[] ipArray2 = ip2.Split('.');

        for (int i = 0; i < Math.Min(ipArray1.Length, ipArray2.Length); i++)
        {
            int part1=0, part2=0;

            if (int.TryParse(ipArray1[i], out part1) && int.TryParse(ipArray2[i], out part2))
            {
                // If the parts are not equal, return the greater version
                if (part1 != part2)
                {
                    return part1 > part2 ? "IP1" : "IP2";
                }
            }
            else
            {
                // Handle the case where parsing fails (e.g., a letter is present)
                // Consider the part as 0 for comparison purposes
                return part1 > 0 ? "IP1" : (part2 > 0 ? "IP2" : "Equal");
            }
        }

        // If all parts are equal, compare the lengths to determine the greater version
        return ipArray1.Length > ipArray2.Length ? "IP1" : (ipArray1.Length < ipArray2.Length ? "IP2" : "Equal");
    }
} 

using BCrypt.Net;

Console.WriteLine("Enter password:");

var password = Console.ReadLine();

if(string.IsNullOrEmpty(password))
{
    Console.WriteLine("Password required");
    return;
}


var hash = BCrypt.Net.BCrypt.HashPassword(password);


Console.WriteLine();
Console.WriteLine("BCrypt Hash:");
Console.WriteLine(hash);
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static string filePath = "users.csv";
    static string autoLoginFile = "autologin.txt";

    static void Main(string[] args)
    {
        if (File.Exists(autoLoginFile))
        {
            string autoLoginUser = File.ReadAllText(autoLoginFile);
            Console.WriteLine($"Login automático para o usuário: {autoLoginUser}");
            return;
        }

        while (true)
        {
            Console.WriteLine("1. Registrar");
            Console.WriteLine("2. Login");
            Console.Write("Escolha uma opção: ");
            string option = Console.ReadLine();

            if (option == "1")
            {
                Register();
            }
            else if (option == "2")
            {
                Login();
            }
            else
            {
                Console.WriteLine("Opção inválida. Tente novamente.");
            }
        }
    }

    static void Register()
    {
        Console.Write("Digite o nome de usuário: ");
        string username = Console.ReadLine();
        Console.Write("Digite a senha: ");
        string password = Console.ReadLine();

        if (UserExists(username))
        {
            Console.WriteLine("Usuário já existe. Tente outro nome de usuário.");
            return;
        }

        string hashedPassword = HashPassword(password);
        string userEntry = $"{username},{hashedPassword}";

        File.AppendAllText(filePath, userEntry + Environment.NewLine);
        Console.WriteLine("Usuário registrado com sucesso!");
    }

    static void Login()
    {
        Console.Write("Digite o nome de usuário: ");
        string username = Console.ReadLine();
        Console.Write("Digite a senha: ");
        string password = Console.ReadLine();

        if (ValidateLogin(username, password))
        {
            Console.WriteLine("Login bem-sucedido!");
            File.WriteAllText(autoLoginFile, username);
        }
        else
        {
            Console.WriteLine("Login falhou. Nome de usuário ou senha incorretos.");
        }
    }

    static bool UserExists(string username)
    {
        if (!File.Exists(filePath))
            return false;

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            if (data[0] == username)
            {
                return true;
            }
        }
        return false;
    }

    static bool ValidateLogin(string username, string password)
    {
        if (!File.Exists(filePath))
            return false;

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            if (data[0] == username && data[1] == HashPassword(password))
            {
                return true;
            }
        }
        return false;
    }

    static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}

using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace SMTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please, enter your email:");
            string inputLogin = Console.ReadLine();
            var login = Convert.ToBase64String(Encoding.UTF8.GetBytes(inputLogin));

            Console.WriteLine("Please, enter your password:");
            var password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Console.ReadLine()));

            using var client = new TcpClient();
            client.Connect("smtp.gmail.com", 465);

            using var stream = client.GetStream();
            using var sslStream = new SslStream(stream);
            sslStream.ReadTimeout = 2000;
            sslStream.AuthenticateAsClient("smtp.gmail.com");

            using var writer = new StreamWriter(sslStream);
            using var reader = new StreamReader(sslStream);

            SendCommand(writer, reader, "EHLO " + "server");
            SendCommand(writer, reader, $"AUTH LOGIN {login}");
            SendCommand(writer, reader, password);
            SendCommand(writer, reader, $"MAIL FROM: <" + inputLogin + ">");
            SendCommand(writer, reader, $"RCPT TO: <" + inputLogin + ">");
            SendCommand(writer, reader, "DATA");
            SendCommand(writer, reader, "From: Sender", false);
            SendCommand(writer, reader, "To: Receiver", false);
            SendCommand(writer, reader, "Subject: Some subject", false);
            SendCommand(writer, reader, "Some text.", false);
            SendCommand(writer, reader, "Second line!", false);
            SendCommand(writer, reader, ".");
            SendCommand(writer, reader, "QUIT");
        }

        static void SendCommand(StreamWriter writer, StreamReader reader, string command, bool read = true)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                Console.WriteLine($">>> {command}");
                writer.WriteLine(command);
                writer.Flush();
            }

            if (read)
            {
                try
                {
                    string text;
                    while ((text = reader.ReadLine()!) != null)
                        Console.WriteLine(text);
                }
                catch { }
            }
        }
    }
}
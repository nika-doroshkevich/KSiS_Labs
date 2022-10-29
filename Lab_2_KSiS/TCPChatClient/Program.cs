using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace TCPChatClient
{
    class Program
    {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            client = new TcpClient(); //Класс TcpClient предназначен для создания клиентской программы
            try
            {
                client.Connect(host, port); //подключение клиента к серсеру TCP, передаем название хоста и порт
                stream = client.GetStream(); // получаем поток //возвращает объект NetworkStream, через него можно общаться с сервером

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length); //отправка данных

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                Console.WriteLine("Добро пожаловать, {0}", userName);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        // отправка сообщений
        static void SendMessage()
        {
            Console.WriteLine("Введите сообщение: ");

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message); //ожидаем, что строка будет в кодировке юникод
                stream.Write(data, 0, data.Length);
            }
        }
        // получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder(); //определяем этот объект для создания строки
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length); //используется для чтения
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes)); //ожидаем, что строка будет в кодировке юникод
                    }
                    while (stream.DataAvailable); //проверка наличия данных в потоке

                    string message = builder.ToString();
                    Console.WriteLine(message);//вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}

using System;

namespace RpcClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var rpcClient = new RpcClient();

            while (true)
            {
                Console.WriteLine("Please press n for cancel!");
                var key = Console.ReadLine();

                if (string.Equals(key, "n", StringComparison.OrdinalIgnoreCase))
                {
                    rpcClient.Close();
                    break;
                }

                Console.WriteLine("Please in put number(int)");
                key = Console.ReadLine();
                int number;
                if (int.TryParse(key, out number))
                {
                    Console.WriteLine($" [x] Requesting fib({number})");
                    var response = rpcClient.Call($"{number}");
                    Console.WriteLine(" [.] Got '{0}'", response);
                    continue;
                }
                Console.WriteLine("inrrect integer.");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaravelNETConnector;
using LaravelNETConnector.Models.Auth;

namespace ExampleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config()
            {
                BaseURL = "https://127.0.0.1:8000/api",
                AuthenticationMode = Enums.AuthenticationType.JWT,
                AutoRefresh = true,
                Out = Console.Out,
                ThrowAuthenticationExceptions = true
            };

            Host.Initialize(config);

            User user = new User("info@adamwoodhead.co.uk", "secretpassword");

            Task.Run(async() => {
                await user.AuthenticateAsync(new EmailPassword(user.Email, user.Password));

                while (true)
                {
                    List<Invoice> invoices = await Invoice.GetAsync();
                }
            });



            Console.ReadLine();
        }
    }
}

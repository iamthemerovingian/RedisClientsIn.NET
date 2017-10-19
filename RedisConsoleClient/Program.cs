using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisConsoleClient
{
    public class Phone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public Person Owner { get; set; }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Profession { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string host = "localhost";
            string elementKey = "testKeyRedis";

            using (RedisClient redisClient = new RedisClient(host))
            {
                if (string.IsNullOrWhiteSpace(redisClient.Get<string>(elementKey)))
                {
                    // adding delay to see the difference
                    Thread.Sleep(5000);
                    // save value in cache
                    redisClient.Set(elementKey, "some cached value");
                }
                // get value from the cache by key
                var message = "Item value is: " + redisClient.Get<string>(elementKey);

                Console.WriteLine(message);
            }

            using (RedisClient redisClient = new RedisClient(host))
            {
                IRedisTypedClient<Phone> phones = redisClient.As<Phone>();
                Phone phoneFive = phones.GetValue("5");
                if (phoneFive == null)
                {
                    // make a small delay
                    Thread.Sleep(5000);
                    // creating a new Phone entry
                    phoneFive = new Phone
                    {
                        Id = 5,
                        Manufacturer = "Motorolla",
                        Model = "xxxxx",
                        Owner = new Person
                        {
                            Id = 1,
                            Age = 90,
                            Name = "OldOne",
                            Profession = "sportsmen",
                            Surname = "OldManSurname"
                        }
                    };
                    // adding Entry to the typed entity set
                    phones.SetEntry(phoneFive.Id.ToString(), phoneFive);
                }
                var message = "Phone model is " + phoneFive.Manufacturer;
                Console.WriteLine(message);
                message += "Phone Owner Name is: " + phoneFive.Owner.Name;
                Console.WriteLine(message);
            }

#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}

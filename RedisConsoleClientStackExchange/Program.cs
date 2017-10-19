using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisConsoleClientStackExchange
{
    class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public bool IsMinor { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            // for multiple redis servers 
            //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("server1:6379,server2:6379");

            IDatabase db = redis.GetDatabase();

            //Loading and getting a string.
            db.StringSet("some key", "Some Value");
            string gottenFromRedisString = db.StringGet("some key");


            //Loding and getting a byte array from redis.
            byte[] key = { 0, 1 };
            byte[] value = { 2, 3 };
            db.StringSet(key, value);
            byte[] gottenfromRedisByteArray = db.StringGet(key);

            //Loading and getting an Object from Redis using my own serialization methods.
            var someFellow = new Person
            {
                Id = 1,
                Name = "Milinda",
                Age = 26,
                IsMinor = false
            };
            db.StringSet(JsonConvert.SerializeObject(someFellow.Id), JsonConvert.SerializeObject(someFellow));
            string personJsonString = db.StringGet("1");
            Person gottenfromRedisPersonCustom = JsonConvert.DeserializeObject<Person>(personJsonString);

            //setting and getting an object from redis using StackExchange.Redis.Extensions
            ISerializer serializer = new NewtonsoftSerializer();
            ICacheClient cacheClient = new StackExchangeRedisCacheClient(redis,serializer);
            bool success = cacheClient.Add(someFellow.Id.ToString(), someFellow);
            Person gottenfromRedisPerson = cacheClient.Get<Person>("1");


            //Writing out the stuff we got back from redis to the console.
            Console.WriteLine($"Gotten from String: {gottenFromRedisString}");
            Console.WriteLine($"Gotten from Array: {gottenfromRedisByteArray}");
            Console.WriteLine($"Gotten from Custom Serializer: {gottenfromRedisPersonCustom.Name}");
            Console.WriteLine($"Gotten from StackExchange.Redis.Extensions: {gottenfromRedisPerson.Name}");


#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}

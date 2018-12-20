using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Neo4JLogistics
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "bolt://localhost:7687/";
            var client = new BoltGraphClient(uri, "neo4j", "nosql");
            client.Connect();
            //1
            Console.WriteLine("Enter the name of the product");
            string productName = Console.ReadLine();
            var product = client.Cypher
                .Match($"(prod:Product{{name:'{productName}'}})")
                .Return(prod => prod.As<Product>())
                .Results;
            //2
            Console.WriteLine("Enter the name of the city you want to get warehouses of");
            string cityName = Console.ReadLine();
            var warehouses = client.Cypher
                .Match($"(city:City{{name:'{cityName}'}})-[:HAS]->(warehouse)")
                .Return(warehouse => warehouse.As<Warehouse>())
                .Results;
            //3
            Console.WriteLine("Enter the name of the city you want to get products of");
            string cityName2 = Console.ReadLine();
            var products = client.Cypher
                .Match($"(city:City{{name:'{cityName2}'}})-[:HAS]-(warehouse)-[:STORES]-(category)-[:BELONGS]->(prods)")
                .Return(prods=> prods.CollectAsDistinct<Product>())
                .Results;
            
            //4
            string cityFrom = "Vilnius";
            string cityTo = "Klaipėda";
            var shortestPath = client.Cypher
                .Match($"(from:City{{name:'{cityFrom}'}}), (to:City{{name:'{cityTo}'}}), p = shortestPath((from)-[:ROAD*..5]->(to))")
                .Return(() => Return.As<IEnumerable<City>>("nodes(p)"))
                .Results;
            //5
            var shortestPathDistance = client.Cypher
                .Match($"(from:City{{name:'{cityFrom}'}}), (to:City{{name:'{cityTo}'}}), p = shortestPath((from)-[:ROAD*..5]->(to))")
                .With("p,reduce(s = 0, r IN rels(p) | s + r.distance) AS dist")
                .Return(dist => dist.As<int>())
                .Results;
        }
    }
    public class Product
    {
        public int quantity { get; set; }
        public string brand { get; set; }
        public string name { get; set; }
        public int price { get; set; }  
    }
    public class Warehouse
    {
        public string name { get; set; }
    }
    public class City
    {
        public string name { get; set; }
    }
    
}

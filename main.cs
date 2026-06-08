using System;
using System.Collections.Generic;

namespace CacheSimulator
{
    class MemoryElement
    {
        public int Id { get; set; }

        public MemoryElement(int id)
        {
            Id = id;
        }
    }

    interface IReplacementAlgorithm
    {
        void ElementAdded(MemoryElement element);
        MemoryElement ReplaceElement(MemoryElement newElement);
    }

    class FifoAlgorithm : IReplacementAlgorithm
    {
        private Queue<MemoryElement> queue = new Queue<MemoryElement>();
        private int capacity;

        public FifoAlgorithm(int capacity)
        {
            this.capacity = capacity;
        }

        public void ElementAdded(MemoryElement element)
        {
            if (queue.Count < capacity)
            {
                queue.Enqueue(element);
            }
        }

        public MemoryElement ReplaceElement(MemoryElement newElement)
        {
            MemoryElement removed = queue.Dequeue();
            queue.Enqueue(newElement);
            return removed;
        }
    }

    class MemoryLevel
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        public Dictionary<int, MemoryElement> Elements { get; set; }
        private IReplacementAlgorithm algorithm;

        public MemoryLevel(string name, int capacity, IReplacementAlgorithm algo)
        {
            Name = name;
            Capacity = capacity;
            Elements = new Dictionary<int, MemoryElement>();
            algorithm = algo;
        }

        public bool Access(int elementId)
        {
            return Elements.ContainsKey(elementId);
        }

        public void AddToCache(MemoryElement element)
        {
            if (Elements.Count >= Capacity)
            {
                MemoryElement removedElement = algorithm.ReplaceElement(element);
                Elements.Remove(removedElement.Id);
            }
            else
            {
                algorithm.ElementAdded(element);
            }
            Elements.Add(element.Id, element);
        }
    }

    class AccessGenerator
    {
        private Random random = new Random();

        public List<int> Generate(int count, int maxId)
        {
            List<int> sequence = new List<int>();
            for (int i = 0; i < count; i++)
            {
                sequence.Add(random.Next(1, maxId + 1));
            }
            return sequence;
        }
    }

    class Simulator
    {
        public void Run(int cacheCapacity, int totalAccesses, int maxElementId)
        {
            IReplacementAlgorithm fifo = new FifoAlgorithm(cacheCapacity);
            MemoryLevel cache = new MemoryLevel("Cache", cacheCapacity, fifo);
            AccessGenerator generator = new AccessGenerator();

            List<int> sequence = generator.Generate(totalAccesses, maxElementId);

            int hits = 0;
            int misses = 0;

            foreach (int id in sequence)
            {
                if (cache.Access(id))
                {
                    hits++;
                }
                else
                {
                    misses++;
                    MemoryElement newElement = new MemoryElement(id);
                    cache.AddToCache(newElement);
                }
            }

            Console.WriteLine($"Config: Cache Size = {cacheCapacity}, Accesses = {totalAccesses}, Max ID = {maxElementId}");
            Console.WriteLine($"Hits: {hits} | Misses: {misses}");
            Console.WriteLine($"Hit Rate: {(double)hits / totalAccesses * 100:F2}%");
            Console.WriteLine(new string('-', 45));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Simulator sim = new Simulator();
            
            sim.Run(5, 100, 20);
            sim.Run(10, 100, 20);
            sim.Run(20, 100, 20);
        }
    }
}

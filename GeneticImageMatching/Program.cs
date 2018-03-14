using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GeneticImageMatching
{
    class Program
    {
        public static void printPerson(int[,] person)
        {// Print single person
            int x = person.GetLength(0);
            int y = person.GetLength(1);
            Console.WriteLine();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Console.Write(person[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static int[,] getGoal(string filePath, int x, int y)
        {// Convert text file to integer 2d array
            string[] data = File.ReadAllLines(filePath);
            int[,] goal = new int[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    goal[i, j] = int.Parse(data[i][j].ToString());
                }
            }            
            return goal;
        }

        public static List<int[,]> generatePopulation(int size, int x, int y)
        {// Create a population based on the goals dimensions
            List<int[,]> population = new List<int[,]>();
            Random random = new Random();
            for (int i = 0; i < size; i++)
            {
                int[,] person = new int[x, y];
                for (int j = 0; j < x; j++)
                {
                    for (int k = 0; k < y; k++)
                    {
                        person[j, k] = random.Next(0, 2);
                    }
                }
                population.Add(person);
            }
            return population;
        }

        public static int calculateFitness(int[,] person, int[,] goal)
        {// Calculate hamming distance between person and goal
            int fitness = 0;
            int x = goal.GetLength(0);
            int y = goal.GetLength(1);
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (person[i, j] != goal[i, j])
                    {
                        fitness++;
                    }
                }
            }
            return fitness;
        }

        public static int[,] crossover(int[,] person1, int[,] person2)
        {// Create child via randomised crossover of parents
            int x = person1.GetLength(0);
            int y = person1.GetLength(1);
            int[,] child = new int[x, y];
            Random random = new Random();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if(random.Next(0, 2) == 0)
                    {
                        child[i, j] = person1[i, j];
                    }
                    else
                    {
                        child[i, j] = person2[i, j];
                    }
                }
            }
            return child;
        }

        public static int[,] mutate(int[,] person, int impact)
        {// Mutate person with variable impact (number of characters flipped)
            int x = person.GetLength(0);
            int y = person.GetLength(1);
            Random random = new Random();
            for (int i = 0; i < impact; i++)
            {
                int randX = random.Next(0, x);
                int randY = random.Next(0, y);

                if (person[randX, randY] == 0)
                {
                    person[randX, randY] = 1;
                }
                else
                {
                    person[randX, randY] = 0;
                }
            }
            return person;
        }

        public static int[,] makeChild(int[,] person1, int[,] person2, int mutationImpact)
        {// Create a child from two parents (Wrapper, implements mutation)
            int[,] child = crossover(person1, person2);
            Random random = new Random();
            if(random.Next(1, 101) > 80)
            {
                child = mutate(child, mutationImpact);
            }
            return child;
        }

        public static List<int[,]> evaluate(List<int[,]> population, int[,] goal)
        {// Sort population by fitness
            Dictionary<int, int> populationDictionary = new Dictionary<int, int>();
            int key = 0;
            foreach (int[,] person in population)
            {
                populationDictionary.Add(key, calculateFitness(person, goal));
                key++;
            }
            var orderedDictionary = populationDictionary.OrderBy(x => x.Value);
            orderedDictionary.ToList();
            List<int[,]> evalutatedPopulation = new List<int[,]>();
            foreach (KeyValuePair<int, int> entry in orderedDictionary)
            {
                evalutatedPopulation.Add(population[entry.Key]);
            }
            return evalutatedPopulation;
        }

        public static List<int[,]> evolve(List<int[,]> population, int size, int pressure, int mutationImpact)
        {// Evolve generation one stage
            List<int[,]> parents = new List<int[,]>();
            for (int i = 0; i < pressure; i++)
            {
                parents.Add(population[i]);
            }
            Random random = new Random();
            List<int[,]> newGeneration = new List<int[,]>();
            for (int i = 0; i < size; i++)
            {
                newGeneration.Add(makeChild(population[random.Next(0, pressure)], population[random.Next(0, pressure)], mutationImpact));
            }
            return newGeneration;
        }

        static void Main(string[] args)
        {
            string projectPath = "D:\\Work\\Fun\\GeneticImageMatching\\GeneticImageMatching\\";
            string filePath = "res\\one.txt";
            int dimX = File.ReadLines(projectPath + filePath).Count(); // # Lines in file
            int dimY = File.ReadLines(projectPath + filePath).First().Count(); // # Characters in line
            int goalFitness = ((dimX * dimY)/100); // 1 %
            int generationLimit = 300;
            int mutationImpact = ((dimX * dimY)/10); // 10 %
            int populationSize = 1000;

            Console.WriteLine("Image dimensions: " + dimX + " x " + dimY + ", size: " + (dimX * dimY));
            Console.WriteLine("Population size: " + populationSize + ", Goal fitness: " + goalFitness + ", Generation limit: " + generationLimit + ", Mutation Impact: " + mutationImpact);

            int[,] goal = getGoal(projectPath + filePath, dimX, dimY);
            List<int[,]> population = generatePopulation(populationSize, dimX, dimY);
            population = evaluate(population, goal);

            int currentFitness = calculateFitness(population[0], goal);
            int generation = 0;

            while (generation < generationLimit && currentFitness > goalFitness)
            {
                population = evolve(population, populationSize, populationSize/4, mutationImpact);
                population = evaluate(population, goal);
                currentFitness = calculateFitness(population[0], goal);
                Console.WriteLine("Generation: " + generation + ", fitness: " + currentFitness);
                generation++;
            }
            printPerson(population[0]);
            Console.ReadLine();
        }
    }
}

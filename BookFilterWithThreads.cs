using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace BookFilterWithThreads
{

    class Program
    {
        static void Main(string[] args)
        {
            
            string filePath = @"C:\\Users\\Acer\\OneDrive\\Работен плот\\MyCode\\text.txt";

            string input;
            try
            {
                input = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return;
            }

            string[] words = ExtractWords(input);

            if (words.Length == 0)
            {
                Console.WriteLine("No valid words found.");
                return;
            }

            int wordCount = 0;
            string shortestWord = string.Empty;
            string longestWord = string.Empty;
            double averageWordLength = 0.0;
            List<string> mostCommonWords = new List<string>();
            List<string> leastCommonWords = new List<string>();

            Thread countThread = new Thread(() => wordCount = words.Length);
            Thread shortestThread = new Thread(() => shortestWord = FindShortestWord(words));
            Thread longestThread = new Thread(() => longestWord = FindLongestWord(words));
            Thread averageThread = new Thread(() => averageWordLength = CalculateAverageWordLength(words));
            Thread mostCommonThread = new Thread(() => mostCommonWords = FindMostCommonWords(words, 5));
            Thread leastCommonThread = new Thread(() => leastCommonWords = FindLeastCommonWords(words, 5));

            countThread.Start();
            shortestThread.Start();
            longestThread.Start();
            averageThread.Start();
            mostCommonThread.Start();
            leastCommonThread.Start();

            countThread.Join();
            shortestThread.Join();
            longestThread.Join();
            averageThread.Join();
            mostCommonThread.Join();
            leastCommonThread.Join();

            Console.WriteLine($"Number of words: {wordCount}");
            Console.WriteLine($"Shortest word: {shortestWord}");
            Console.WriteLine($"Longest word: {longestWord}");
            Console.WriteLine($"Average word length: {averageWordLength:F2}");
            Console.WriteLine("Five most common words:");
            foreach (string word in mostCommonWords)
            {
                Console.WriteLine(word);
            }
            Console.WriteLine("Five least common words:");
            foreach (string word in leastCommonWords)
            {
                Console.WriteLine(word);
            }
        }

        static string[] ExtractWords(string input)
        {
            List<string> validWords = new List<string>();
            string[] potentialWords = input.Split(' ', '\n', '\t');

            foreach (string word in potentialWords)
            {
                string cleanedWord = RemovePunctuation(word);
                if (cleanedWord.Length >= 3)
                {
                    validWords.Add(cleanedWord.ToLower());
                }
            }

            return validWords.ToArray();
        }

        static string RemovePunctuation(string word)
        {
            char[] result = word.Where(c => char.IsLetterOrDigit(c)).ToArray();
            return new string(result);
        }

        static string FindShortestWord(string[] words)
        {
            string shortest = words[0];

            foreach (string word in words)
            {
                if (word.Length < shortest.Length)
                {
                    shortest = word;
                }
            }

            return shortest;
        }

        static string FindLongestWord(string[] words)
        {
            string longest = words[0];

            foreach (string word in words)
            {
                if (word.Length > longest.Length)
                {
                    longest = word;
                }
            }

            return longest;
        }

        static double CalculateAverageWordLength(string[] words)
        {
            int totalLength = 0;

            foreach (string word in words)
            {
                totalLength += word.Length;
            }

            return (double)totalLength / words.Length;
        }

        static List<string> FindMostCommonWords(string[] words, int count)
        {
            Dictionary<string, int> wordFrequency = new Dictionary<string, int>();

            foreach (string word in words)
            {
                if (wordFrequency.ContainsKey(word))
                {
                    wordFrequency[word]++;
                }
                else
                {
                    wordFrequency[word] = 1;
                }
            }

            var sortedWords = wordFrequency.OrderByDescending(kvp => kvp.Value).ThenBy(kvp => kvp.Key);
            return sortedWords.Take(count).Select(kvp => kvp.Key).ToList();
        }

        static List<string> FindLeastCommonWords(string[] words, int count)
        {
            Dictionary<string, int> wordFrequency = new Dictionary<string, int>();

            foreach (string word in words)
            {
                if (wordFrequency.ContainsKey(word))
                {
                    wordFrequency[word]++;
                }
                else
                {
                    wordFrequency[word] = 1;
                }
            }

            var filteredWords = wordFrequency
                .Where(kvp => kvp.Key.Length >= 3 && kvp.Key.All(char.IsLetter))
                .OrderBy(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key);

            return filteredWords.Take(count).Select(kvp => kvp.Key).ToList();
        }

    }

}

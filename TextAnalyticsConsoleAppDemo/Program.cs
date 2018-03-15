namespace TextAnalyticsConsoleAppDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

    class Program
    {
        private static void DetectLanguage(ITextAnalyticsAPI client)
        {
            // Extracting language
            Console.WriteLine("===== LANGUAGE EXTRACTION ======");

            var inputs1 = new List<Input>()
            {
                new Input("1", "This is a document written in English."),
                new Input("2", "C'est un fichier écrit en français."),
                new Input("3", "这是一个用中文写的文件")
            };

            LanguageBatchResult result = client.DetectLanguage(new BatchInput(inputs1));

            // Printing language results.

            foreach (var document in result.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} , Language: {document.DetectedLanguages[0].Name} , Text: {inputs1.FirstOrDefault(i => i.Id == document.Id).Text}");
            }
        }

        private static void KeyPhrases(ITextAnalyticsAPI client)
        {
            // Getting key-phrases
            Console.WriteLine("\n\n===== KEY-PHRASE EXTRACTION ======");

            var inputs2 = new List<MultiLanguageInput>()
            {
                new MultiLanguageInput("zh", "1", "我的兔子花花好可爱."),
                new MultiLanguageInput("de", "2", "Fahrt nach Stuttgart und dann zum Hotel zu Fu."),
                new MultiLanguageInput("en", "3", "My cat is stiff as a rock."),
                new MultiLanguageInput("es", "4", "A mi me encanta el fútbol!")
            };

            KeyPhraseBatchResult result2 = client.KeyPhrases(new MultiLanguageBatchInput(inputs2));

            // Printing keyphrases
            foreach (var document in result2.Documents)
            {
                Console.WriteLine("Document ID: {0} ", document.Id);

                Console.WriteLine("\t Key phrases:");

                foreach (string keyphrase in document.KeyPhrases)
                {
                    Console.WriteLine("\t\t" + keyphrase);
                }

                Console.WriteLine($"Text: {inputs2.FirstOrDefault(i => i.Id == document.Id).Text}");
                Console.WriteLine();
            }
        }

        private static void Sentiment(ITextAnalyticsAPI client)
        {
            // Extracting sentiment
            Console.WriteLine("\n\n===== SENTIMENT ANALYSIS ======");

            var inputs3 = new List<MultiLanguageInput>()
            {
                new MultiLanguageInput("en", "0", "I had the best day of my life."),
                new MultiLanguageInput("en", "1", "This was a waste of my time. The speaker put me to sleep."),
                new MultiLanguageInput("fr", "2", "lol, eh qu'on a pas fini de passer à la caisse!"),
                new MultiLanguageInput("es", "3", "No tengo dinero ni nada que dar..."),
                new MultiLanguageInput("it", "4", "L'hotel veneziano era meraviglioso. È un bellissimo pezzo di architettura."),
            };

            SentimentBatchResult result3 = client.Sentiment(new MultiLanguageBatchInput(inputs3));

            // Printing sentiment results
            foreach (var document in result3.Documents)
            {
                Console.WriteLine($"Document ID: {document.Id} , Sentiment Score: {document.Score:0.00} , Text: {inputs3.FirstOrDefault(i => i.Id == document.Id).Text}");
            }
        }

        private static void SentimentConsoleInput(ITextAnalyticsAPI client)
        {
            // Extracting sentiment
            Console.WriteLine("\n\n===== SENTIMENT ANALYSIS CONSOLE INPUT ======");

            var canBreak = false;

            for (; ; )
            {
                Console.WriteLine("Please type your comment and press 'Enter', 'Q' or 'q' to exit.");
                var input = Console.ReadLine();
                canBreak = input.Equals("q", StringComparison.OrdinalIgnoreCase);

                if (canBreak)
                {
                    Console.WriteLine("Exit.");
                    break;
                }

                SentimentBatchResult result = client.Sentiment(new MultiLanguageBatchInput(new List<MultiLanguageInput> { new MultiLanguageInput()
                {
                    Id = DateTimeOffset.UtcNow.ToFileTime().ToString(),
                    Text = input
                }}));

                foreach (var document in result.Documents)
                {
                    Console.WriteLine($"Document ID: {document.Id} , Sentiment Score: {document.Score:0.00} , Text: {input}");
                }

                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Start...");

            // Create a client.
            ITextAnalyticsAPI client = new TextAnalyticsAPI();
            client.AzureRegion = AzureRegions.Eastus2;
            client.SubscriptionKey = "YOUR KEY";

            DetectLanguage(client);
            KeyPhrases(client);
            Sentiment(client);
            SentimentConsoleInput(client);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}

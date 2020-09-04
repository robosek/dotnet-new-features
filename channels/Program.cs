using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Diagnostics;

namespace dotnet_new_features
{
    internal class HardWorker
    {
        internal async Task<string> HardAndLongTextProcessing(string someText)
        {
            Console.WriteLine($"Start processing text: {someText}");
            await Task.Delay(5000);
            var processedText = someText.ToUpper();
            Console.WriteLine($"Uff it's done. Result is: {processedText}");
            
            return processedText;
        }
    }

    internal class OldGoodProcessor
    {
        private readonly string[] _texts;
        private readonly HardWorker _worker;

        internal OldGoodProcessor(string[] texts, HardWorker worker)
        {
            _texts = texts;
            _worker = worker;
        }

        internal async IAsyncEnumerable<string> ProcessOneByOneAsync()
        {
            foreach (var text in _texts)
            {
                yield return await _worker.HardAndLongTextProcessing(text);
            }
        }
    }

    internal class Producer
    {
        private readonly ChannelWriter<string> _writer;

        public Producer(ChannelWriter<string> writer)
        {
            _writer = writer;
        }

        public async Task WriteMessage(string message)
        {
            await _writer.WriteAsync(message);
        }
    }

    internal class Consumer
    {
        private readonly ChannelReader<string> _reader;
        private readonly HardWorker _worker;

        public Consumer(ChannelReader<string> reader, HardWorker worker)
        {
            _reader = reader;
            _worker = worker;
        }

        public async Task ProcessMessage()
        {
            while (await _reader.WaitToReadAsync())
            {
                var result = _reader.TryRead(out var text);
                if (result)
                {
                    await _worker.HardAndLongTextProcessing(text);
                }
                else
                {    
                    Console.WriteLine("Cannot process data!");
                }
            }
        }
    }
    
    

    static class Program
    {
        static async Task SimpleOneByOneProcessing(Stopwatch stopWatch, HardWorker worker, string[] textsToProcess)
        {
            Console.WriteLine($"Let's start with {nameof(OldGoodProcessor)}");
            var oldGoodProcessor = new OldGoodProcessor(textsToProcess, worker);
            await foreach (var processedText in oldGoodProcessor.ProcessOneByOneAsync()){}
            stopWatch.Stop();
            Console.WriteLine($"Processing is done for {nameof(oldGoodProcessor)} {stopWatch.Elapsed.Minutes}:{stopWatch.Elapsed.Seconds}:{stopWatch.Elapsed.Milliseconds}");

        }
        
        static async Task ManyProducersOneConsumer(Stopwatch stopWatch, HardWorker worker, string[] textsToProcess)
        {
            stopWatch.Start();
            Console.WriteLine($"Let's start with N:1 {nameof(Producer)} and {nameof(Consumer)}");
            var channel = Channel.CreateUnbounded<string>();
            var producer1 = new Producer(channel.Writer);
            var producer2 = new Producer(channel.Writer);
            var producer3 = new Producer(channel.Writer);
            var producer4 = new Producer(channel.Writer);
            var producer5 = new Producer(channel.Writer);
            
            var consumer = new Consumer(channel.Reader, worker);
            var producerTask1 = producer1.WriteMessage(textsToProcess[0]);
            var producerTask2 = producer2.WriteMessage(textsToProcess[1]);
            var producerTask3 = producer3.WriteMessage(textsToProcess[2]);
            var producerTask4 = producer4.WriteMessage(textsToProcess[3]);
            var producerTask5 = producer5.WriteMessage(textsToProcess[4]);
            
            var consumerTask = consumer.ProcessMessage();
            await Task.WhenAll(producerTask1, producerTask2, producerTask3, producerTask4, producerTask5).ContinueWith(_ => channel.Writer.Complete());
            await consumerTask;
            stopWatch.Stop();
            Console.WriteLine($"Processing is done for N:1 {nameof(Producer)} and {nameof(Consumer)}. Time: {stopWatch.Elapsed.Minutes}:{stopWatch.Elapsed.Seconds}:{stopWatch.Elapsed.Milliseconds}");

        }

        static async Task ManyProducersManyConsumers(Stopwatch stopWatch, HardWorker worker, string[] textsToProcess)
        {
            stopWatch.Start();
            Console.WriteLine($"Let's start with N:N {nameof(Producer)} and {nameof(Consumer)}");
            var channel = Channel.CreateUnbounded<string>();
            var producer1 = new Producer(channel.Writer);
            var producer2 = new Producer(channel.Writer);
            var producer3 = new Producer(channel.Writer);
            var producer4 = new Producer(channel.Writer);
            var producer5 = new Producer(channel.Writer);
            
            var consumer1 = new Consumer(channel.Reader, worker);
            var consumer2 = new Consumer(channel.Reader, worker);
            var consumer3 = new Consumer(channel.Reader, worker);

            var producerTask1 = producer1.WriteMessage(textsToProcess[0]);
            var producerTask2 = producer2.WriteMessage(textsToProcess[1]);
            var producerTask3 = producer3.WriteMessage(textsToProcess[2]);
            var producerTask4 = producer4.WriteMessage(textsToProcess[3]);
            var producerTask5 = producer5.WriteMessage(textsToProcess[4]);
            
            var consumerTask1 = consumer1.ProcessMessage();
            var consumerTask2 = consumer2.ProcessMessage();
            var consumerTask3 = consumer3.ProcessMessage();

            await Task.WhenAll(producerTask1, producerTask2, producerTask3, producerTask4, producerTask5).ContinueWith(_ => channel.Writer.Complete());
            await Task.WhenAll(consumerTask1, consumerTask2, consumerTask3);
            stopWatch.Stop();
            Console.WriteLine($"Processing is done for N:N {nameof(Producer)} and {nameof(Consumer)}. Time: {stopWatch.Elapsed.Minutes}:{stopWatch.Elapsed.Seconds}:{stopWatch.Elapsed.Milliseconds}");
        }
        
        static async Task Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var textsToProcess = new string [] {"some_text_1", "some_text2", "some_text3", "some_text4", "some_text5"};
            var worker = new HardWorker();
            
            await SimpleOneByOneProcessing(stopWatch, worker, textsToProcess);
            
            stopWatch.Restart();
            await ManyProducersOneConsumer(stopWatch, worker, textsToProcess);
            
            stopWatch.Restart();
            await ManyProducersManyConsumers(stopWatch, worker, textsToProcess);
        }
    }
}

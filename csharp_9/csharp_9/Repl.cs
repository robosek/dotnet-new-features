using System;

var hello = new HelloWorld("hello world!");

hello.SayHello();

public record HelloWorld(string helloMessage)
{
    public void SayHello()
    {
        Console.WriteLine(helloMessage);
    }
}
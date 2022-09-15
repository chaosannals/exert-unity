﻿using System;
using ClientDemoCommon;

Console.WriteLine("Client Auto !");

// 因为 demo 是和服务器一起启动，所以先等待服务器初始化完成。
Thread.Sleep(4000);

var clients = Enumerable.Range(1, 4000)
    //.AsParallel()
    .ToDictionary(i => i, i => 
    {
        var r = new GameClient(i);
        r.Connect();
        Thread.Sleep(100);
        return r;
    });

// TODO 需要连接完成才能发送，这个等待要换成事件。
Thread.Sleep(2000);

while (true)
{
    try
    {
        foreach (var client in clients)
        {
            var d = DateTime.Now;
            client.Value.Send($"client {client.Key} say: {d}");
        }
        var n = DateTime.Now;
        Console.WriteLine($"[{n}] clients say final.");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
    Thread.Sleep(1000);
}

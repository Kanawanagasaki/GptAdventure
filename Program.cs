using GptAdventure;

Console.WriteLine("Loading GPT model...");
var gpt = new GptJ();
gpt.Start();
while (!gpt.IsReady)
    Thread.Sleep(1000);

Console.WriteLine("Warming up GPT model...");

gpt.Generate("Hello world");
while (!gpt.IsWaitingForPromt)
    Thread.Sleep(1000);

Console.WriteLine("Loading assets...");

var game = new MyGame(gpt);
game.Run();

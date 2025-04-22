namespace LAB1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PaintApp app = new PaintApp(20, 10); // Холст 20x10 символов
            app.Run();
        }
    }
}

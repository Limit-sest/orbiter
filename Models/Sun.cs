namespace orbiter.Models;

public class Sun
{
    private readonly string[] texture = {
        "⣲⣵⣵⣅",
        "⢽⣿⣿⢃",
        "⡞⡻⠏⠳"
    };
    public void ProcessTick()
    {
        Console.Write($"\x1b[38;5;11m");
        for (int i = 0; i < texture.Length; i++)

        {
            Console.SetCursorPosition(Console.BufferWidth / 2 - texture[0].Length / 2, Console.BufferHeight / 2 - texture.Length / 2 + i);
            Console.Write(texture[i]);
        }
        Console.Write("\x1b[0m");

    }
}

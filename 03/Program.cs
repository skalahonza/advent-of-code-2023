using System.Collections.Frozen;

Solve1("sample.txt");
Solve1("input.txt");
Console.WriteLine("-----");
Solve2("sample.txt");
Solve2("input.txt");

void Solve1(string path)
{
    Console.WriteLine(path);
    var lines = File.ReadAllLines(path);
    var matrix = LoadMatrix(lines);
    Console.WriteLine(lines
        .SelectMany(ParseNumbers)
        .Where(x => x.IsPartNumber(matrix))
        .Select(x => x.Value)
        .Sum());
}

void Solve2(string path)
{
}

char[,] LoadMatrix(string[] lines)
{
    var matrix = new char[lines.Length, lines[0].Length];
    for (var y = 0; y < lines.Length; y++)
    {
        for (var x = 0; x < lines[y].Length; x++)
        {
            matrix[y, x] = lines[y][x];
        }
    }
    return matrix;
}

IEnumerable<Number> ParseNumbers(string line, int y)
{
    int? number = null;
    var boxes = new List<NumberBox>();
    for (var x = 0; x < line.Length; x++)
    {
        var c = line[x];
        if (char.IsDigit(c))
        {
            number ??= 0;
            number = number * 10 + (c - '0');
            boxes.Add(new NumberBox(x, y));
        }
        else if(number.HasValue)
        {
            yield return new Number(number.Value, boxes.ToFrozenSet());
            number = null;
            boxes.Clear();
        }
    }
    
    if(number.HasValue)
    {
        yield return new Number(number.Value, boxes.ToFrozenSet());
    }
}

public record NumberBox(int X, int Y)
{
    public bool IsAdjacentToSymbol(char[,] matrix)
    {
        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <= 1; j++)
            {
                if(i == 0 && j == 0)
                {
                    continue;
                }
                
                var x = X + i;
                var y = Y + j;
                
                if(x < 0 || x >= matrix.GetLength(1) || y < 0 || y >= matrix.GetLength(0))
                {
                    continue;
                }
                
                var symbol = matrix[y, x];
                if (!char.IsDigit(symbol) && symbol != '.')
                {
                    return true;
                }
            }
        }

        return false;
    }
}

public record Number(int Value, FrozenSet<NumberBox> Boxes)
{
    public bool IsPartNumber(char[,] matrix) => Boxes.Any(x => x.IsAdjacentToSymbol(matrix));
}
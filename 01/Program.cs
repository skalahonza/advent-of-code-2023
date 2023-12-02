using System.Net.Mime;

SolvePart1("sample.txt");
SolvePart1("input.txt");
SolvePart2("sample2.txt");
SolvePart2("input2.txt");

// part 1 -----------------------------------------------

static void SolvePart1(string path)
{
    Console.WriteLine(path);
    var lines = File.ReadAllLines(path);
    var sum = lines.Select(ExtractNumber).Sum();
    Console.WriteLine(sum);
}

static int ExtractNumber(string line)
{
    if (!line.All(char.IsDigit))
    {
        // error i guess
    }

    var firstDigit = line.First(char.IsDigit);
    var lastDigit = line.Last(char.IsDigit);

    return int.Parse($"{firstDigit}{lastDigit}");
}

// part 2 ----------------------------------------------
static void SolvePart2(string path)
{
    Console.WriteLine(path);
    var lines = File.ReadAllLines(path);
    var sum = lines.Select(ExtractNumber2).Sum();
    Console.WriteLine(sum);
}

static int ExtractNumber2(string line)
{
    var digits = ExtractDigits(line).ToList();
    var firstDigit = digits.First();
    var lastDigit = digits.Last();
    return int.Parse($"{firstDigit}{lastDigit}");
}

static IEnumerable<int> ExtractDigits(string line)
{
    var buffer = new List<char>();
    foreach (var c in line)
    {
        buffer.Add(c);
        var digit = RecognizeDigit(buffer);
        if (digit.HasValue)
        {
            yield return digit.Value;
        }
    }
}

static int? RecognizeDigit(IReadOnlyCollection<char> buffer)
{
    var lastChar = buffer.Last();
    if (char.IsDigit(lastChar))
    {
        return int.Parse($"{lastChar}");
    }

    var input = new string(buffer.ToArray());
    foreach (var number in Enum.GetValues<Number>())
    {
        if (input.EndsWith(number.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return (int) number;
        }
    }

    return null;
}

enum Number
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9
}

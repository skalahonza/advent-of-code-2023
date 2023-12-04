using System.Collections.Frozen;

Solve1("sample.txt");
Solve1("input.txt");
Console.WriteLine("-----");
Solve2("sample.txt");
Solve2("input.txt");

void Solve1(string path)
{
    Console.WriteLine($"Solving {path}");
    var lines = File.ReadAllLines(path);
    var cards = lines.Select(Card.Parse).ToList();
    var points = cards.Select(x => x.GetPoints()).ToList();
    Console.WriteLine(points.Sum());
}

void Solve2(string path)
{
    Console.WriteLine($"Solving {path}");
    var lines = File.ReadAllLines(path);
    var originalCards = lines.Select(Card.Parse).ToDictionary(x => x.Id);
    var cards = lines.Select(Card.Parse).ToList();
    var queue = new Queue<Card>(cards);
    while (queue.Count > 0)
    {
        var card = queue.Dequeue();
        var numbersThatWon = card.GetMatchingNumbers();
        for (int i = 0; i < numbersThatWon; i++)
        {
            if (originalCards.TryGetValue(card.Id + i + 1, out var wonCard))
            {
                queue.Enqueue(wonCard);
                cards.Add(wonCard);
            }
        }
    }

    Console.WriteLine(cards.Count);
}

public class Card
{
    public required int Id { get; init; }
    public required FrozenSet<int> WinningNumbers { get; init; }
    public required FrozenSet<int> Numbers { get; init; }

    public int GetMatchingNumbers() => Numbers.Intersect(WinningNumbers).Count();

    public int GetPoints()
    {
        var numbersThatWon = GetMatchingNumbers();
        return numbersThatWon > 0
            ? (int) Math.Pow(2, numbersThatWon - 1)
            : 0;
    }

    public static Card Parse(string line)
    {
        var parts = line.Split(':');
        var header = parts[0];
        var id = ParseId(header);

        var numbers = parts[1];
        var winningNumbers = ParseNumbers(numbers.Split('|')[0]).ToFrozenSet();
        var givenNumbers = ParseNumbers(numbers.Split('|')[1]).ToFrozenSet();

        return new Card
        {
            Id = id,
            WinningNumbers = winningNumbers,
            Numbers = givenNumbers
        };
    }

    public override string ToString() =>
        $"{Id}: {string.Join(" ", WinningNumbers)} | {string.Join(" ", Numbers)}";

    private static IEnumerable<int> ParseNumbers(string input) =>
        input.Split(' ').Select(x => x.Trim()).Where(x => x.Any(char.IsDigit)).Select(int.Parse);

    private static int ParseId(string header) =>
        int.Parse(new string(header.Where(char.IsDigit).ToArray()));
}
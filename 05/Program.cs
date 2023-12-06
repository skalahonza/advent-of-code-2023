using System.Text;

Solve1("sample.txt");
Solve1("input.txt"); // 214922730
Solve2("sample.txt");
Solve2("input.txt");
return;

void Solve1(string path)
{
    Console.WriteLine(path);

    var lines = File.ReadAllLines(path);
    var seeds = ParseSeeds(lines);
    var seedToSoil = ParseMap(lines, "seed-to-soil");
    var soilToFertilizer = ParseMap(lines, "soil-to-fertilizer");
    var fertilizerToWater = ParseMap(lines, "fertilizer-to-water");
    var waterToLight = ParseMap(lines, "water-to-light");
    var lightToTemperature = ParseMap(lines, "light-to-temperature");
    var temperatureToHumidity = ParseMap(lines, "temperature-to-humidity");
    var humidityToLocation = ParseMap(lines, "humidity-to-location");

    var locations = new List<long>();
    foreach (var seed in seeds)
    {
        var soil = seedToSoil.Get(seed);
        var fertilizer = soilToFertilizer.Get(soil);
        var water = fertilizerToWater.Get(fertilizer);
        var light = waterToLight.Get(water);
        var temperature = lightToTemperature.Get(light);
        var humidity = temperatureToHumidity.Get(temperature);
        var location = humidityToLocation.Get(humidity);
        locations.Add(location);
    }

    Console.WriteLine("Result: " + locations.Min());
}

void Solve2(string path)
{
    Console.WriteLine(path);

    var lines = File.ReadAllLines(path);
    var seeds = ParseSeeds(lines);
    var chunks = seeds.Chunk(2);
    
    var seedToSoil = ParseMap(lines, "seed-to-soil");
    var soilToFertilizer = ParseMap(lines, "soil-to-fertilizer");
    var fertilizerToWater = ParseMap(lines, "fertilizer-to-water");
    var waterToLight = ParseMap(lines, "water-to-light");
    var lightToTemperature = ParseMap(lines, "light-to-temperature");
    var temperatureToHumidity = ParseMap(lines, "temperature-to-humidity");
    var humidityToLocation = ParseMap(lines, "humidity-to-location");
    
    var locations = new List<long>();
    foreach (var chunk in chunks)
    {
        var start = chunk.First();
        var length = chunk.Last();
        for (var seed = start; seed < start + length; seed++)
        {
            var soil = seedToSoil.Get(seed);
            var fertilizer = soilToFertilizer.Get(soil);
            var water = fertilizerToWater.Get(fertilizer);
            var light = waterToLight.Get(water);
            var temperature = lightToTemperature.Get(light);
            var humidity = temperatureToHumidity.Get(temperature);
            var location = humidityToLocation.Get(humidity);
            locations.Add(location);
        }
    }

    Console.WriteLine("Result: " + locations.Min());
}

long[] ParseSeeds(string[] lines)
{
    var line = lines.Single(x => x.Contains("seeds:"));
    return line.Split(' ')
        .Where(x => x.Any(char.IsDigit))
        .Select(x => x.Trim())
        .Select(long.Parse)
        .ToArray();
}

Map ParseMap(string[] lines, string name)
{
    lines.Single(x => x.Contains(name)); // assertion
    var index = Array.FindIndex(lines, x => x.Contains(name));
    var ranges = new List<Range>();
    foreach (var x in lines.Skip(index + 1).TakeWhile(x => x.Any(char.IsDigit)))
    {
        var parts = x.Split(' ').Select(x => x.Trim()).ToList();
        var numbers = parts.Select(long.Parse).ToList();
        if (numbers is not [var destinationRangeStart, var sourceRangeStart, var rangeLength])
        {
            throw new Exception();
        }

        ranges.Add(new Range(destinationRangeStart, sourceRangeStart, rangeLength));
    }

    var map = new Map(name, ranges);
    return map;
}

public record Range(long DestinationRangeStart, long SourceRangeStart, long RangeLength)
{
    public bool TryGet(long source, out long destination)
    {
        checked
        {
            destination = source - SourceRangeStart + DestinationRangeStart;
            return source >= SourceRangeStart
                   && source < SourceRangeStart + RangeLength
                   && destination >= DestinationRangeStart
                   && destination < DestinationRangeStart + RangeLength;
        }
    }

    public override string ToString() => 
        $"{DestinationRangeStart} {SourceRangeStart} {RangeLength}";
}

public record Map(string Name, IReadOnlyList<Range> Ranges)
{
    public long Get(long source)
    {
        foreach (var range in Ranges)
        {
            if (range.TryGet(source, out var destination))
            {
                return destination;
            }
        }

        return source;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine(Name);
        foreach (var range in Ranges)
        {
            builder.AppendLine(range.ToString());
        }
        return builder.ToString();
    }
}
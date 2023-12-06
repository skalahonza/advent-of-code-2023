using System.Text;

Solve1("sample.txt"); // 35
Solve1("input.txt"); // 214922730
//Solve2("sample.txt"); // 46
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
    var seedToSoil = ParseMap(lines, "seed-to-soil");
    var soilToFertilizer = ParseMap(lines, "soil-to-fertilizer");
    var fertilizerToWater = ParseMap(lines, "fertilizer-to-water");
    var waterToLight = ParseMap(lines, "water-to-light");
    var lightToTemperature = ParseMap(lines, "light-to-temperature");
    var temperatureToHumidity = ParseMap(lines, "temperature-to-humidity");
    var humidityToLocation = ParseMap(lines, "humidity-to-location");

    var locations = new List<long>();
    var ranges = ParseSeedRanges(lines).OrderBy(x => x.Start).ToList();
    foreach (var range in ranges)
    {
        foreach (var seed in range.GetSeeds())
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
        
        if (path.Contains("input"))
        {
            var index = ranges.IndexOf(range);
            Console.WriteLine($"Progress: {index + 1}/{ranges.Count}");
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

IEnumerable<SeedRange> ParseSeedRanges(string[] lines)
{
    var seeds = ParseSeeds(lines);
    return seeds.Chunk(2)
        .Select(chunk =>
        {
            var start = chunk.First();
            var length = chunk.Last();
            return new SeedRange(start, length);
        });
}

OptimizedMap ParseMap(string[] lines, string name)
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
    return map.Optimize();
}

public record SeedRange(long Start, long Length)
{
    public IEnumerable<long> GetSeeds()
    {
        for (var i = Start; i < Start + Length; i++)
        {
            yield return i;
        }
    }
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
    
    public OptimizedMap Optimize() => new(this);
}

/// <summary>
/// This optimized map assumes that the keys will be looked up in order.
/// So caching the last range will speed up the lookup.
/// </summary>
public class OptimizedMap
{
    private readonly Map _map;
    private Range? _currentRange;

    public OptimizedMap(Map map) => 
        _map = map with {Ranges = map.Ranges.OrderBy(x => x.SourceRangeStart).ToList()};

    public long Get(long source)
    {
        var destination = source;
        // range not cached or not found in cached range
        if (_currentRange is null || !_currentRange.TryGet(source, out destination))
        {
            // find the range that contains the source
            _currentRange = _map.Ranges.FirstOrDefault(x => x.TryGet(source, out destination));
        }
        
        // if no range contains the source, return the source
        if (_currentRange is null)
        {
            return source;
        }
        
        return destination;
    }
}
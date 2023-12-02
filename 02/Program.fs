﻿open System.IO

type Configuration = {
    Red: int
    Green: int
    Blue: int
}

type Game = {
    Id: int
    Configurations: Configuration list
}

let isValidConfiguration config r g b =
    config.Red <= r && config.Green <= g && config.Blue <= b
    
let isValidGame game r g b =
    game.Configurations |> List.forall (fun config -> isValidConfiguration config r g b)
    
/// Parse color from input like:
/// 3 blue or 4 red
let parseColor (input: string) =
    let parts = input.Trim().Split(' ')
    let count = int parts[0]
    let color = parts[1].ToLower()
    match color with
    | "red" -> { Red = count; Green = 0; Blue = 0 }
    | "green" -> { Red = 0; Green = count; Blue = 0 }
    | "blue" -> { Red = 0; Green = 0; Blue = count }
    | _ -> failwith "Invalid color"

/// Merge two configurations by summing their values    
let mergeConfigurations a b = {
    Red = a.Red + b.Red
    Green = a.Green + b.Green
    Blue = a.Blue + b.Blue
}

/// Parse game configuration from string like:
///  3 blue, 4 red
let parseConfiguration (input: string) =
    input.Split(',') |> Array.map parseColor |> Array.reduce mergeConfigurations
    
/// Parse configurations separated by semicolon
/// 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
let parseConfigurations (line: string) =
    line.Split(';') |> Array.map parseConfiguration |> Array.toList
    
/// Parse game from string like:
/// Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
let parseGame (line: string) =
    let parts = line.Split(':')
    let game = parts[0]
    let configs = parts[1]
    let id = int game.[4..]
    let configurations = parseConfigurations configs
    { Id = id; Configurations = configurations }
    
printfn "sample.txt"
let games = File.ReadAllLines("sample.txt") |> Array.map parseGame
games |> Array.filter (fun game -> isValidGame game 12 13 14)
|> Array.map (fun game -> game.Id)
|> Array.sum
|> printfn "%d"
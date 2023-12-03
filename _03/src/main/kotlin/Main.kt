import java.io.File

fun main(args: Array<String>) {
    //solve1("C:\\Users\\jskal\\source\\repos\\advent-of-code-2023\\_03\\src\\main\\resources\\sample.txt")
    solve1("C:\\Users\\jskal\\source\\repos\\advent-of-code-2023\\_03\\src\\main\\resources\\input.txt")
}

data class NumberBox(val x: Int, val y: Int) {
    fun isAdjacentToSymbol(matrix: List<CharArray>): Boolean {
        for (i in -1..1) {
            for (j in -1..1) {
                if (i == 0 && j == 0) continue
                val x = this.x + i
                val y = this.y + j
                if (x < 0 || y < 0 || x >= matrix[0].size || y >= matrix.size) continue
                val c = matrix[y][x]
                if (isSymbol(c)) {
                    return true
                }
            }
        }
        return false
    }
}

data class Number(val value: Int, val boxes: List<NumberBox>) {
    fun isPartNumber(matrix: List<CharArray>): Boolean {
        return boxes.any { it.isAdjacentToSymbol(matrix) }
    }
}

fun parseNumbers(y: Int, line: String): List<Number> {
    val boxes = mutableListOf<NumberBox>()
    val numbers = mutableListOf<Number>()
    var value = 0
    line.forEachIndexed { x, char ->
        if (char.isDigit()) {
            value *= 10
            value += char.digitToInt()
            boxes.add(NumberBox(x, y))
        } else if (boxes.any()) {
            numbers.add(Number(value, boxes.toList()))
            value = 0
            boxes.clear()
        }
    }
    return numbers
}

fun isSymbol(char: Char): Boolean = !(char == '.' || char.isDigit())

fun solve1(path: String) {
    val lines = File(path).readLines()
    val matrix = lines.map { it.toCharArray() }
    val numbers = lines.mapIndexed(::parseNumbers).flatten()
    val partNumbers = numbers.filter { it.isPartNumber(matrix) }
    println(path)
    println(partNumbers.sumOf { it.value })
}
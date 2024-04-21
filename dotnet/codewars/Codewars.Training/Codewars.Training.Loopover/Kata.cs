using System.Collections.Generic;

namespace Codewars.Training.Loopover;

public static class Kata
{
    public static List<string>? Solve(char[][] mixedUpBoard, char[][] solvedBoard)
    {
        var solver = new AStarAlgorithm(solvedBoard);
        return solver.Solve(mixedUpBoard);
    }
}
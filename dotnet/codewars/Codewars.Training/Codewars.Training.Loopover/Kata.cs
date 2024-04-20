using System.Collections.Generic;

namespace Codewars.Training.Loopover;

public static class Kata
{
    private static List<BoardNode> GetNeighbors(BoardNode BoardNode)
    {
        // TODO: Implement this function to generate all possible next states
        // from the current state by applying all possible moves to the current
        // board configuration.
        return null;
    }

    public static IEnumerable<string> Solve(char[][] mixedUpBoard, char[][] solvedBoard)
    {
        var startNode = new BoardNode
        {
            Board = mixedUpBoard, Moves = new(), Parent = null
        };
        var endNode = new BoardNode
        {
            Board = solvedBoard, Moves = new(), Parent = null
        };

        var queue = new Queue<BoardNode>();
        var visited = new HashSet<string>();

        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            // TODO: Implement a function to compare two boards
            if (AreBoardsEqual(currentNode.Board, endNode.Board))
            {
                return currentNode.Moves;
            }

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                // TODO: Implement a function to convert a board to a string
                var boardString = BoardToString(neighbor.Board);

                if (!visited.Contains(boardString))
                {
                    visited.Add(boardString);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return null;
    }
}
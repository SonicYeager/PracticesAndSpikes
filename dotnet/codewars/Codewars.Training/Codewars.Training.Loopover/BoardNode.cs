using System.Collections.Generic;

namespace Codewars.Training.Loopover;

public sealed class BoardNode
{
    public char[][] Board { get; set; } = null!;
    public List<Move> Moves { get; set; } = null!;
    public BoardNode? Parent { get; set; }
}
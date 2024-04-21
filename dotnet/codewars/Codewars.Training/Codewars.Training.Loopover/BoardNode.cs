using System.Collections.Generic;

namespace Codewars.Training.Loopover;

public sealed record BoardNode
{
    public char[][] Board { get; init; } = null!;
    public List<Move> Moves { get; init; } = null!;
    public BoardNode? Parent { get; set; }
}
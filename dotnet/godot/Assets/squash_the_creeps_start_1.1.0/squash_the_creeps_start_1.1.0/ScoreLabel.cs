using Godot;
using System;

public partial class ScoreLabel : Label
{
    private int _score;

    public void OnMobSquashed()
    {
        Text = $"Score: {++_score}";
    }
}
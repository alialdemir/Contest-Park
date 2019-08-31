namespace ContestPark.Duel.API.Services.ScoreCalculator
{
    public interface IScoreCalculator
    {
        byte Calculator(int round, byte time);
    }
}